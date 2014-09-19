using SF.Expand.LOG;
using SF.Expand.SAF.Core.Factory;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public class PREProcessorTokens : ITokens, ITokenValidator
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/";
		public OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore)
		{
			tokenInfoCore = new TokenInfoCore();
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenParamsID(tokenVendorID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					tokenInfoCore = new TokenInfoCore();
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.Create(tokenVendorID, expirationDate, supplierSerialNumber, creationLotID, pin, out tokenInfoCore);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				tokenInfoCore = new TokenInfoCore();
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult UndoCreate(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.UndoCreate(tokenInternalID);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult InhibitedUse(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.InhibitedUse(tokenInternalID);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult AllowedUse(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.AllowedUse(tokenInternalID);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult Cancel(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.Cancel(tokenInternalID);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens _tokens = TokensFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokens]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					tokenStatus = TokenStatus.Undefined;
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.CheckStatus(tokenInternalID, out tokenStatus);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				tokenStatus = TokenStatus.Undefined;
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult ChallengeRequest(string tokenInternalID, string dataEntropy, out string newChallange)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator _tokens = TokenValidatorFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokenValidator]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					newChallange = null;
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.ChallengeRequest(tokenInternalID, dataEntropy, out newChallange);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				newChallange = null;
				result = OperationResult.Error;
			}
			return result;
		}
		public AutenticationStatus Autenticate(string tokenInternalID, string password, string dataentropy, out string Challenge)
		{
			AutenticationStatus result;
			try
			{
				string assemb = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator _tokens = TokenValidatorFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokenValidator]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					Challenge = null;
					result = AutenticationStatus.AutenticationProcessFail;
				}
				else
				{
					result = _tokens.Autenticate(tokenInternalID, password, dataentropy, out Challenge);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				Challenge = null;
				result = AutenticationStatus.AutenticationProcessFail;
			}
			return result;
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, string dataEntropy, out string newPwd)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator _tokens = TokenValidatorFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokenValidator]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					newPwd = null;
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.StartServerAuthentication(tokenInternalID, dataEntropy, out newPwd);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				newPwd = null;
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, long movingFactorDrift, string dataEntropy, out string newPwd)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator _tokens = TokenValidatorFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokenValidator]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					newPwd = null;
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.StartServerAuthentication(tokenInternalID, movingFactorDrift, dataEntropy, out newPwd);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				newPwd = null;
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult Synchronize(string tokenInternalID, string firstPwd, string secondPwd)
		{
			OperationResult result;
			try
			{
				string assemb = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator _tokens = TokenValidatorFactory.LoadAssembly(assemb);
				if (_tokens == null)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
						"[ITokenValidator]::" + assemb.Trim(),
						"Invalid or null typename!"
					});
					result = OperationResult.Error;
				}
				else
				{
					result = _tokens.Synchronize(tokenInternalID, firstPwd, secondPwd);
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult ResetChallengeRequest(string tokenInternalID)
		{
			return new TokensChallengeRequestDAO().resetChallengeRequest(tokenInternalID);
		}
		public OperationResult ResetMovingFactor(string tokenInternalID, long movingFactorValue)
		{
			return new TokensDAO().updateMovingFactor(tokenInternalID, movingFactorValue);
		}
		public TokenInfoCore loadTokenInfoCore(string tokenInternalID)
		{
			return new TokensDAO().loadTokenInfoCore(tokenInternalID);
		}
		public int TRIPLEDESEncrypt(string deviceKey, string messageData, out string outMessage)
		{
			outMessage = null;
			int result;
			if (deviceKey == null || (deviceKey ?? "").Length < 1 || messageData == null || (messageData ?? "").Length < 1)
			{
				result = 100;
			}
			else
			{
				outMessage = CryptorEngineTripleDES.Encrypt(messageData, new SecurityInfo(deviceKey, deviceKey, deviceKey), true);
				result = ((outMessage == null || (outMessage ?? "").Length < 1) ? 109 : 0);
			}
			return result;
		}
		public int TRIPLEDESDecrypt(string deviceKey, string messageData, out string outMessage)
		{
			outMessage = null;
			int result;
			if (deviceKey == null || (deviceKey ?? "").Length < 1 || messageData == null || (messageData ?? "").Length < 1)
			{
				result = 100;
			}
			else
			{
				outMessage = CryptorEngineTripleDES.Decrypt(messageData, new SecurityInfo(deviceKey, deviceKey, deviceKey), true);
				result = ((outMessage == null || (outMessage ?? "").Length < 1) ? 109 : 0);
			}
			return result;
		}
		public int TRIPLEDESDecryptOTPBase(string deviceKey, string messageData, string deviceID, string dataEntropy, out string outMessage)
		{
			outMessage = null;
			int result;
			if (deviceID == null || (deviceID ?? "").Length < 1 || deviceKey == null || (deviceKey ?? "").Length < 1 || messageData == null || (messageData ?? "").Length < 1)
			{
				result = 100;
			}
			else
			{
				TokenStatus tokenStatus;
				if (OperationResult.Success != this.CheckStatus(deviceID, out tokenStatus))
				{
					result = 101;
				}
				else
				{
					if (tokenStatus != TokenStatus.Enabled)
					{
						result = 102;
					}
					else
					{
						string newPWD = null;
						if (OperationResult.Success != this.StartServerAuthentication(deviceID, long.Parse(deviceKey), dataEntropy, out newPWD))
						{
							result = 103;
						}
						else
						{
							string msgData = CryptorEngineTripleDES.Decrypt(messageData, new SecurityInfo(deviceKey, newPWD, deviceKey), true);
							result = ((outMessage == null || (outMessage ?? "").Length < 1) ? 109 : 0);
						}
					}
				}
			}
			return result;
		}
	}
}
