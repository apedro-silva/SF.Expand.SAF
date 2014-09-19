using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
using System.Reflection;
using System.Text;
namespace SF.Expand.SAF.Core
{
	public static class BaseImportExportTokens
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.PREProcessorTokens.softfinanca.com/";
		private static string cFIELD_SEPARATOR_PARSER = ";";
		public static TokenCryptoData ParseFileInputLine(string tokenData, TokenTypeBaseParams tokenTypeBaseParams)
		{
			TokenCryptoData result;
			try
			{
				string[] _flds = tokenData.Split(new char[]
				{
					'\a'
				});
				if (_flds.Length != 5)
				{
					result = default(TokenCryptoData);
				}
				else
				{
					if (_flds[1].Length < 1 || _flds[2].Length < 1 || _flds[3].Length < 1)
					{
						result = default(TokenCryptoData);
					}
					else
					{
						if (tokenTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber && (_flds[0].Length < 1 || _flds[4].Length < 1))
						{
							result = default(TokenCryptoData);
						}
						else
						{
							result = new TokenCryptoData(null, _flds[0], new CryptoData(long.Parse(_flds[2]), _flds[3], _flds[1], _flds[4]), tokenTypeBaseParams);
						}
					}
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
				result = default(TokenCryptoData);
			}
			return result;
		}
		public static OperationResult Import(TokenCryptoData tokenCryptoData, string tokenCreationLotID, DateTime tokenExpirationDate)
		{
			OperationResult result;
			try
			{
				if (tokenCryptoData.CryptoData.MovingFactor == 0L || tokenCryptoData.CryptoData.InternalSerialNumber == null || tokenCryptoData.CryptoData.CryptoKey == null)
				{
					result = OperationResult.Error;
				}
				else
				{
					string tkID;
					result = new TokensDAO().createToken(tokenCryptoData.TokenBaseParams.TokenTypeBaseParamsID, tokenCryptoData.CryptoData.MovingFactor, tokenExpirationDate, tokenCryptoData.CryptoData.CryptoKey, tokenCryptoData.SupplierSerialNumber, tokenCryptoData.CryptoData.InternalSerialNumber, tokenCreationLotID, tokenCryptoData.CryptoData.SupportCryptoData, out tkID);
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
		public static long tokensSeedsBulkInsert(string pathFileName, TokenStatus tokenStatus, DateTime tokenExpiration)
		{
			return new TokensDAO().tokensSeedsBulkInsert(pathFileName, tokenStatus, tokenExpiration);
		}
		public static string Export(TokenCryptoData tokenData, string loteID)
		{
			StringBuilder strBuild = new StringBuilder(900);
			string result;
			try
			{
				if (tokenData.SupplierSerialNumber == null || tokenData.CryptoData.CryptoKey == null || tokenData.CryptoData.InternalSerialNumber == null)
				{
					result = null;
				}
				else
				{
					strBuild.Append(loteID);
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append(tokenData.TokenBaseParams.TokenTypeBaseParamsID.ToString());
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append(tokenData.SupplierSerialNumber);
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append(tokenData.CryptoData.InternalSerialNumber);
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append(tokenData.CryptoData.MovingFactor.ToString().Trim());
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append(tokenData.CryptoData.CryptoKey);
					strBuild.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					strBuild.Append((tokenData.CryptoData.SupportCryptoData.Length < 1) ? "NULL" : tokenData.CryptoData.SupportCryptoData);
					result = strBuild.ToString();
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
				result = null;
			}
			return result;
		}
	}
}
