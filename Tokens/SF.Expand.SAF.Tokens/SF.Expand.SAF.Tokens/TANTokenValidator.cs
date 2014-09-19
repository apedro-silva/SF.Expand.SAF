using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
namespace SF.Expand.SAF.Tokens
{
	public class TANTokenValidator : ITokenValidator
	{
		private string _masterKey = SAFConfiguration.readMasterKey();
		private int columnCount = int.Parse(SAFConfiguration.readParameterExternal("TANMatrixColumnsCount"));
		private int rowCount = int.Parse(SAFConfiguration.readParameterExternal("TANMatrixRowsCount"));
		private string formatChallenge(string thisChallenge)
		{
			int _baseASCII = 64;
			string newChallengeformat = null;
			string result;
			try
			{
				string[] arrChallenge = thisChallenge.Split(new char[]
				{
					'|'
				});
				for (int index = 0; index < arrChallenge.Length; index++)
				{
					int _arrPos = int.Parse(arrChallenge[index].Split(new char[]
					{
						';'
					})[0]);
					int iCalc = Convert.ToInt32((double)(_arrPos / this.columnCount) + 0.99);
					int iCalc2 = _arrPos - (iCalc - 1) * this.columnCount;
					newChallengeformat = newChallengeformat + ((char)(_baseASCII + iCalc)).ToString() + (iCalc2 + 1).ToString() + (int.Parse(arrChallenge[index].Split(new char[]
					{
						';'
					})[1]) + 1).ToString();
				}
				result = newChallengeformat;
			}
			catch
			{
				result = null;
			}
			return result;
		}
		public AutenticationStatus Autenticate(string tokenInternalID, string password, string dataEntropy, out string newChallenge)
		{
			newChallenge = null;
			TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
			AutenticationStatus result;
			if (_tkCryptoData.ID == null)
			{
				result = AutenticationStatus.TokenNotFoundOrCanceled;
			}
			else
			{
				if (_tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber)
				{
					throw new Exception("Function not implemented for this type of token!!");
				}
				string currentChallenge = (string)new TokensChallengeRequestDAO().loadChallengeRequest(tokenInternalID);
				if (currentChallenge == null)
				{
					result = AutenticationStatus.InvalidDataOnPasswordValidation;
				}
				else
				{
					int iRequest = int.Parse(SAFConfiguration.readParameterExternal("TANRequestPositions"));
					int iDigitsByPos = int.Parse(SAFConfiguration.readParameterExternal("TANDigitsByPosition"));
					int iFixPosOnFaill = int.Parse(SAFConfiguration.readParameterExternal("TANFixedPosOnFail"));
					string _otp = string.Empty;
					byte[] _tkSeedOpen = _tkCryptoData.GetTokenSeed("");
					byte[] _dataEntropy = (dataEntropy == null || dataEntropy.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy);
					string[] _arrayPosValid = currentChallenge.Split(new char[]
					{
						'|'
					});
					string[] _arrayChallenge = BaseFunctions.DecodeFrom64(_tkCryptoData.CryptoData.SupportCryptoData.Trim()).Split(new char[]
					{
						';'
					});
					for (int idx = 0; idx < _arrayPosValid.Length; idx++)
					{
						string[] _temp = _arrayPosValid[idx].Trim().Split(new char[]
						{
							';'
						});
						_tkCryptoData.ResetMovingFactor(long.Parse(_arrayChallenge[(int)checked((IntPtr)long.Parse(_temp[0]))]));
						_otp += HOTPPwdGenerator.generate(_tkCryptoData, _tkSeedOpen, _dataEntropy).Substring(int.Parse(_temp[1]), 1);
					}
					if (password.Trim() == _otp)
					{
						if (OperationResult.Success == new TokensChallengeRequestDAO().resetChallengeRequest(tokenInternalID))
						{
							result = AutenticationStatus.Success;
							return result;
						}
					}
					result = AutenticationStatus.TokenOrPasswordInvalid;
				}
			}
			return result;
		}
		public OperationResult ChallengeRequest(string tokenInternalID, string dataEntropy, out string newChallenge)
		{
			TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
			OperationResult result;
			if (_tkCryptoData.ID == null)
			{
				newChallenge = null;
				result = OperationResult.Error;
			}
			else
			{
				if (_tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber && _tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.EventBase && _tkCryptoData.TokenBaseParams.SeedType == TokenSeedType.ActivactionKey)
				{
					throw new Exception("Function not implemented for this type of token!!");
				}
				string currentChallenge = (string)new TokensChallengeRequestDAO().loadChallengeRequest(tokenInternalID);
				if (currentChallenge != null)
				{
					newChallenge = this.formatChallenge(currentChallenge.Trim());
					result = OperationResult.Success;
				}
				else
				{
					int _idx = 0;
					int iRequest = int.Parse(SAFConfiguration.readParameterExternal("TANRequestPositions"));
					int iDigitsByPos = int.Parse(SAFConfiguration.readParameterExternal("TANDigitsByPosition"));
					int[] _array = new int[iRequest];
					string _lastRequest = string.Empty;
					DateTime _lastRequestValidThru = (_tkCryptoData.TokenBaseParams.ChallengeRequestValidUntil > 0) ? DateTime.Now.AddSeconds((double)_tkCryptoData.TokenBaseParams.ChallengeRequestValidUntil) : DateTime.MaxValue;
					while (_idx != iRequest)
					{
						bool _flag;
						do
						{
							_flag = false;
							Random rndArray = new Random();
							_array[_idx] = rndArray.Next(0, _tkCryptoData.TokenBaseParams.OTPValidationWindow);
							for (int _idx2 = 0; _idx2 < _idx; _idx2++)
							{
								if (_array[_idx] == _array[_idx2])
								{
									_flag = true;
									break;
								}
							}
						}
						while (_flag);
						Random rndPos = new Random();
						string text = _lastRequest;
						_lastRequest = string.Concat(new string[]
						{
							text,
							_array[_idx].ToString().Trim(),
							";",
							rndPos.Next(0, _tkCryptoData.TokenBaseParams.OTPTotalDigits).ToString().Trim(),
							"|"
						});
						_idx++;
					}
					newChallenge = this.formatChallenge(_lastRequest.Substring(0, _lastRequest.Length - 1).Trim());
					result = new TokensChallengeRequestDAO().persistChallengeRequest(tokenInternalID, _lastRequest.Substring(0, _lastRequest.Length - 1).Trim(), _lastRequestValidThru);
				}
			}
			return result;
		}
		public OperationResult ResetChallengeRequest(string tokenInternalID)
		{
			return new TokensChallengeRequestDAO().resetChallengeRequest(tokenInternalID);
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, long movingFactorDrift, string dataEntropy, out string newPwd)
		{
			throw new NotImplementedException();
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, string dataEntropy, out string newPwd)
		{
			throw new NotImplementedException();
		}
		public OperationResult Synchronize(string tokenInternalID, string firstPwd, string secondPwd)
		{
			throw new NotImplementedException();
		}
		public OperationResult ResetMovingFactor(string tokenInternalID, long movingFactorValue)
		{
			throw new NotImplementedException();
		}
	}
}
