using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using System;
using System.Data;
using System.Text;
namespace SF.Expand.SAF.Core
{
	public static class TokensBaseFunctions
	{
		public static OperationResult tokenTANFetchSupplierSerialNumber(LoteType loteType, string lotID, string TokenVendorID)
		{
			DataTable dataTable = null;
			TokenTypeBaseParams tokenTypeBaseParams = default(TokenTypeBaseParams);
			InMemoryLogging logString = InMemoryLogging.GetLogString(lotID.Trim() + "SerialNumbers.TXT", false);
			OperationResult result;
			try
			{
				tokenTypeBaseParams = new TokenParamsDAO().loadTokenBaseParams(TokenVendorID);
				if (tokenTypeBaseParams.TokenTypeBaseParamsID == null || tokenTypeBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber)
				{
					result = OperationResult.Error;
				}
				else
				{
					if (OperationResult.Error == new TokensDAO().tokenSupplierSerialNumbersByLot(loteType, lotID, out dataTable))
					{
						result = OperationResult.Error;
					}
					else
					{
						foreach (DataRow dataRow in dataTable.Rows)
						{
							logString.Add(dataRow[0].ToString().Trim());
						}
						logString.Persist();
						result = OperationResult.Success;
					}
				}
			}
			catch
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::tokenTANFetchSupplierSerialNumber[]", null);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenTANFetchMatrixValues(LoteType loteType, string lotID, string TokenVendorID)
		{
			DataTable dataTable = null;
			TokenTypeBaseParams tokenTypeBaseParams = default(TokenTypeBaseParams);
			string str = SAFConfiguration.readParameterExternal("ExportFilePath");
			InMemoryLogging logString = InMemoryLogging.GetLogString(str + "\\" + lotID.TrimEnd(new char[]
			{
				' '
			}) + ".DAT", false);
			string masterKey = SAFConfiguration.readMasterKey();
			OperationResult result;
			try
			{
				tokenTypeBaseParams = new TokenParamsDAO().loadTokenBaseParams(TokenVendorID);
				if (tokenTypeBaseParams.TokenTypeBaseParamsID == null || tokenTypeBaseParams.MovingFactorType != TokenMovingFactorType.TransactionAuthenticationNumber)
				{
					result = OperationResult.Error;
				}
				else
				{
					if (OperationResult.Error == new TokensDAO().loadTableWithTokensLot(loteType, lotID, TokenVendorID, TokenMovingFactorType.TransactionAuthenticationNumber, out dataTable))
					{
						result = OperationResult.Error;
					}
					else
					{
						foreach (DataRow dataRow in dataTable.Rows)
						{
							TokenCryptoData tokenCryptoData = new TokenCryptoData(dataRow[5].ToString(), dataRow[0].ToString(), new CryptoData((long)dataRow[1], dataRow[2].ToString().Trim(), dataRow[3].ToString().Trim(), (dataRow[6] != null) ? dataRow[6].ToString().Trim() : string.Empty), tokenTypeBaseParams);
							logString.Add(tokenCryptoData.SupplierSerialNumber + ";" + string.Join(";", TokensBaseFunctions.tokenTANMatrixArrayFetch(tokenCryptoData, masterKey, null)));
						}
						logString.Persist();
						result = OperationResult.Success;
					}
				}
			}
			catch (Exception logObject)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::tokenTANFetchMatcrixValues[]", logObject);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenTANMatrixIntegrityCheck(TokenCryptoData tokenCryptoData, byte[] tkSeed, byte[] entropy, out string SupportCriptoData)
		{
			OperationResult result;
			try
			{
				string text = string.Empty;
				string[] array = new string[tokenCryptoData.TokenBaseParams.OTPValidationWindow];
				for (int i = 0; i < tokenCryptoData.TokenBaseParams.OTPValidationWindow; i++)
				{
					bool flag;
					do
					{
						flag = false;
						tokenCryptoData.ResetMovingFactor(tokenCryptoData.CryptoData.MovingFactor + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift);
						array[i] = HOTPPwdGenerator.generate(tokenCryptoData, tkSeed, new byte[0]);
						for (int j = 0; j < i; j++)
						{
							if (array[i] == array[j])
							{
								flag = true;
								break;
							}
						}
					}
					while (flag);
					text = text + array[i] + ";";
				}
				SupportCriptoData = BaseFunctions.EncodeTo64(text.Substring(0, text.Length - 1));
				result = OperationResult.Success;
			}
			catch
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::TANMatrixIntegrityCheck[]", null);
				SupportCriptoData = null;
				result = OperationResult.Error;
			}
			return result;
		}
		public static string[] tokenTANMatrixArrayFetch(TokenCryptoData tokenCryptoData, string masterKey, string dataEntropy)
		{
			string[] result;
			try
			{
				string[] array = BaseFunctions.DecodeFrom64(tokenCryptoData.CryptoData.SupportCryptoData).Split(new char[]
				{
					';'
				});
				if (tokenCryptoData.TokenBaseParams.OTPValidationWindow != array.Length)
				{
					result = null;
				}
				else
				{
					byte[] tokenSeed = tokenCryptoData.GetTokenSeed(masterKey);
					string[] array2 = new string[tokenCryptoData.TokenBaseParams.OTPValidationWindow];
					for (int i = 0; i < tokenCryptoData.TokenBaseParams.OTPValidationWindow; i++)
					{
						tokenCryptoData.ResetMovingFactor((long)int.Parse(array[i]));
						array2[i] = HOTPPwdGenerator.generate(tokenCryptoData, tokenSeed, (dataEntropy == null || dataEntropy.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy));
					}
					result = array2;
				}
			}
			catch
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::TANMatrixArrayFetch[]", null);
				result = null;
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
				byte[] data;
				byte[] data2;
				long movingFactor;
				if (OperationResult.Error == HOTPCryptoData.Generate(masterKey, null, tkTypeBaseParams, out data, out data2, out movingFactor))
				{
					tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
					result = OperationResult.Error;
				}
				else
				{
					TokenCryptoData tokenCryptoData2 = new TokenCryptoData(null, vendorSerialNumber, new CryptoData(movingFactor, BaseFunctions.HexEncoder(data), BaseFunctions.HexEncoder(data2), ""), tkTypeBaseParams);
					tokenCryptoData2.ResetMovingFactor(HOTPCipherInitialize.createSequenceNumber());

                    /*--------------------------*/
                    byte[] tokenSeed = tokenCryptoData2.GetTokenSeed(masterKey);
                    string x = Encoding.ASCII.GetString(tokenSeed);
                    Base32Encoder enc = new Base32Encoder();
                    string y = enc.Encode(tokenSeed);


                    /*--------------------------*/


					if (tkTypeBaseParams.MovingFactorType == TokenMovingFactorType.TransactionAuthenticationNumber)
					{
						string value;
						if (OperationResult.Error == TokensBaseFunctions.tokenTANMatrixIntegrityCheck(tokenCryptoData2, tokenCryptoData2.GetTokenSeed(masterKey), (dataEntropy == null || dataEntropy.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy), out value))
						{
							tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
							result = OperationResult.Error;
							return result;
						}
						tokenCryptoData2.ResetSupportCryptoData(value);
					}
					tokenCryptoData = tokenCryptoData2;
					result = OperationResult.Success;
				}
			}
			catch
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::TokensCreateNew[]", null);
				tokenCryptoData = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
				result = OperationResult.Error;
			}
			return result;
		}
	}
}
