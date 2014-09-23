using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
using System.Text;
namespace SF.Expand.SAF.Core
{
	public static class BaseImportExportTokens
	{
		private static string cFIELD_SEPARATOR_PARSER = ";";
		public static TokenCryptoData ParseFileInputLine(string tokenData, TokenTypeBaseParams tokenTypeBaseParams)
		{
			TokenCryptoData result;
			try
			{
				string[] array = tokenData.Split(new char[]
				{
					'\a'
				});
				if (array.Length != 5)
				{
					result = default(TokenCryptoData);
				}
				else
				{
					if (array[1].Length < 1 || array[2].Length < 1 || array[3].Length < 1)
					{
						result = default(TokenCryptoData);
					}
					else
					{
						if (tokenTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber && (array[0].Length < 1 || array[4].Length < 1))
						{
							result = default(TokenCryptoData);
						}
						else
						{
							result = new TokenCryptoData(null, array[0], new CryptoData(long.Parse(array[2]), array[3], array[1], array[4]), tokenTypeBaseParams);
						}
					}
				}
			}
			catch
			{
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
					string text;
					result = new TokensDAO().createToken(tokenCryptoData.TokenBaseParams.TokenTypeBaseParamsID, tokenCryptoData.CryptoData.MovingFactor, tokenExpirationDate, tokenCryptoData.CryptoData.CryptoKey, tokenCryptoData.SupplierSerialNumber, tokenCryptoData.CryptoData.InternalSerialNumber, tokenCreationLotID, tokenCryptoData.CryptoData.SupportCryptoData, out text);
				}
			}
			catch
			{
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
			StringBuilder stringBuilder = new StringBuilder(900);
			string result;
			try
			{
				if (tokenData.SupplierSerialNumber == null || tokenData.CryptoData.CryptoKey == null || tokenData.CryptoData.InternalSerialNumber == null)
				{
					result = null;
				}
				else
				{
					stringBuilder.Append(loteID);
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append(tokenData.TokenBaseParams.TokenTypeBaseParamsID.ToString());
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append(tokenData.SupplierSerialNumber);
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append(tokenData.CryptoData.InternalSerialNumber);
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append(tokenData.CryptoData.MovingFactor.ToString().Trim());
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append(tokenData.CryptoData.CryptoKey);
					stringBuilder.Append(BaseImportExportTokens.cFIELD_SEPARATOR_PARSER);
					stringBuilder.Append((tokenData.CryptoData.SupportCryptoData.Length < 1) ? "NULL" : tokenData.CryptoData.SupportCryptoData);
					result = stringBuilder.ToString();
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}
	}
}
