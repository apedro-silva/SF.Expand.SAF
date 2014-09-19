using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public static class TokensBaseFunctions
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/";
		public static OperationResult tokenTANFetchSupplierSerialNumber(LoteType loteType, string lotID, string TokenVendorID)
		{
			DataTable _dt = null;
			TokenTypeBaseParams _tkBaseParams = default(TokenTypeBaseParams);
			string _exportFilePath = SAFConfiguration.readParameterExternal("ExportFilePath");
			SAFLOGGERInMEMORY logger = SAFLOGGERInMEMORY.GetLogString(_exportFilePath + "\\" + lotID.Trim() + "SerialNumbers.TXT", false);
			logger.Clear();
			OperationResult result;
			try
			{
				_tkBaseParams = new TokenParamsDAO().loadTokenBaseParams(TokenVendorID);
				if (_tkBaseParams.TokenTypeBaseParamsID == null || _tkBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber)
				{
					result = OperationResult.Error;
				}
				else
				{
					if (OperationResult.Error == new TokensDAO().tokenSupplierSerialNumbersByLot(loteType, lotID, out _dt))
					{
						result = OperationResult.Error;
					}
					else
					{
						foreach (DataRow row in _dt.Rows)
						{
							logger.Add(row[0].ToString().Trim());
						}
						logger.Persist();
						result = OperationResult.Success;
					}
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				_dt = null;
				logger = null;
				_tkBaseParams = default(TokenTypeBaseParams);
			}
			return result;
		}
		public static OperationResult tokenTANFetchMatrixValues(LoteType loteType, string lotID, string TokenVendorID)
		{
			DataTable _dt = null;
			TokenTypeBaseParams _tkBaseParams = default(TokenTypeBaseParams);
			string _masterKey = SAFConfiguration.readMasterKey();
			string _exportFilePath = SAFConfiguration.readParameterExternal("ExportFilePath");
			SAFLOGGERInMEMORY logger = SAFLOGGERInMEMORY.GetLogString(_exportFilePath + "\\" + lotID.Trim() + ".DAT", false);
			logger.Clear();
			OperationResult result;
			try
			{
				_tkBaseParams = new TokenParamsDAO().loadTokenBaseParams(TokenVendorID);
				if (_tkBaseParams.TokenTypeBaseParamsID == null || _tkBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber)
				{
					result = OperationResult.Error;
				}
				else
				{
					if (OperationResult.Error == new TokensDAO().loadTableWithTokensLot(loteType, lotID, TokenVendorID, TokenMovingFactorType.TransactionAuthenticationNumber, out _dt))
					{
						result = OperationResult.Error;
					}
					else
					{
						foreach (DataRow row in _dt.Rows)
						{
							TokenCryptoData _tkCryptoData = new TokenCryptoData(row[5].ToString(), row[0].ToString(), new CryptoData((long)row[1], row[2].ToString().Trim(), row[3].ToString().Trim(), (row[6] != null) ? row[6].ToString().Trim() : string.Empty), _tkBaseParams);
							logger.Add(_tkCryptoData.SupplierSerialNumber + ";" + string.Join(";", TokensBaseFunctions.tokenTANMatrixArrayFetch(_tkCryptoData, _masterKey, null)));
						}
						logger.Persist();
						result = OperationResult.Success;
					}
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				_dt = null;
				//TokenCryptoData _tkCryptoData = default(TokenCryptoData);
				_tkBaseParams = default(TokenTypeBaseParams);
			}
			return result;
		}
		public static OperationResult tokenTANMatrixIntegrityCheck(TokenCryptoData tokenCryptoData, byte[] tkSeed, byte[] entropy, out string SupportCriptoData)
		{
			string _TanMatrixChallenge = string.Empty;
			OperationResult result;
			try
			{
				string[] _matriz = new string[tokenCryptoData.TokenBaseParams.OTPValidationWindow];
				for (int _idx = 0; _idx < tokenCryptoData.TokenBaseParams.OTPValidationWindow; _idx++)
				{
					bool _flag;
					do
					{
						_flag = false;
						tokenCryptoData.ResetMovingFactor(tokenCryptoData.CryptoData.MovingFactor + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift);
						_matriz[_idx] = HOTPPwdGenerator.generate(tokenCryptoData, tkSeed, new byte[0]);
						for (int _idx2 = 0; _idx2 < _idx; _idx2++)
						{
							if (_matriz[_idx] == _matriz[_idx2])
							{
								_flag = true;
								break;
							}
						}
					}
					while (_flag);
					_TanMatrixChallenge = _TanMatrixChallenge + _matriz[_idx] + ";";
				}
				SupportCriptoData = BaseFunctions.EncodeTo64(_TanMatrixChallenge.Substring(0, _TanMatrixChallenge.Length - 1));
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				SupportCriptoData = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
			}
			return result;
		}
		public static string[] tokenTANMatrixArrayFetch(TokenCryptoData tokenCryptoData, string masterKey, string dataEntropy)
		{
			string[] result;
			try
			{
				string[] _TanChallenge = BaseFunctions.DecodeFrom64(tokenCryptoData.CryptoData.SupportCryptoData).Split(new char[]
				{
					';'
				});
				if (tokenCryptoData.TokenBaseParams.OTPValidationWindow != _TanChallenge.Length)
				{
					result = null;
				}
				else
				{
					byte[] _seedOpen = tokenCryptoData.GetTokenSeed(masterKey);
					string[] _matriz = new string[tokenCryptoData.TokenBaseParams.OTPValidationWindow];
					for (int _index = 0; _index < tokenCryptoData.TokenBaseParams.OTPValidationWindow; _index++)
					{
						tokenCryptoData.ResetMovingFactor((long)int.Parse(_TanChallenge[_index]));
						_matriz[_index] = HOTPPwdGenerator.generate(tokenCryptoData, _seedOpen, (dataEntropy == null || dataEntropy.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy));
					}
					result = _matriz;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
			}
			return result;
		}
		public static OperationResult TokensImportNew(TokenTypeBaseParams tkTypeBaseParams, string masterKey, string vendorSerialNumber, string externalSeed, string pin, long movingFactor, out TokenCryptoData TokenCryptoData)
		{
			TokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
			OperationResult result;
			try
			{
				if (tkTypeBaseParams.SeedType != TokenSeedType.Dynamic)
				{
					throw new Exception("Invalid Seed type!");
				}
				if (tkTypeBaseParams.MovingFactorType != TokenMovingFactorType.EventBase || movingFactor < 1L)
				{
					throw new Exception("Invalid MovingFactorType!");
				}
				byte[] tkserial = HOTPCipherInitialize.createSerialNumber((pin == null || pin.Length < 1) ? HOTPCipherInitialize.Generate4DigitsPin() : pin);
				byte[] tkseed = HOTPCipher.encryptData(BaseFunctions.HexDecoder(externalSeed), HOTPCipherInitialize.createCryptKey(tkserial, (masterKey == null || masterKey.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(masterKey)));
				TokenCryptoData = new TokenCryptoData(null, vendorSerialNumber, new CryptoData(movingFactor, BaseFunctions.HexEncoder(tkseed), BaseFunctions.HexEncoder(tkserial), ""), tkTypeBaseParams);
				TokenCryptoData.ResetMovingFactor(movingFactor);
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				TokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
			}
			return result;
		}
		public static OperationResult TokensCreateNew(TokenTypeBaseParams tkTypeBaseParams, string masterKey, string vendorSerialNumber, string dataEntropy, out TokenCryptoData tokenCryptoData)
		{
			OperationResult result;
			try
			{
				byte[] tkseed;
				byte[] tkserial;
				long tkmovFactor;
				if (OperationResult.Error == HOTPCryptoData.Generate(masterKey, null, tkTypeBaseParams, out tkseed, out tkserial, out tkmovFactor))
				{
					tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
					result = OperationResult.Error;
				}
				else
				{
					TokenCryptoData _tkCryptoData = new TokenCryptoData(null, vendorSerialNumber, new CryptoData(tkmovFactor, BaseFunctions.HexEncoder(tkseed), BaseFunctions.HexEncoder(tkserial), ""), tkTypeBaseParams);
					_tkCryptoData.ResetMovingFactor(HOTPCipherInitialize.createSequenceNumber());
					if (tkTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
					{
						string supportCryptoData;
						if (OperationResult.Error == TokensBaseFunctions.tokenTANMatrixIntegrityCheck(_tkCryptoData, _tkCryptoData.GetTokenSeed(masterKey), (dataEntropy == null || dataEntropy.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy), out supportCryptoData))
						{
							tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
							result = OperationResult.Error;
							return result;
						}
						_tkCryptoData.ResetSupportCryptoData(supportCryptoData);
					}
					tokenCryptoData = _tkCryptoData;
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensBaseFunctions.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				// byte[] tkseed = null;
				// byte[] tkserial = null;
			}
			return result;
		}
	}
}
