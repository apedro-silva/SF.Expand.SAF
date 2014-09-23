using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Utils;
using System;
namespace SF.Expand.SAF.Core
{
	public static class DBConnectionString
	{
		private const string NEW_LINE = "\r\n";
		private const string cBASE_NAME = "SF.Expand.SAF.Core.DBConnectionString";
		public static string ExpandSAFCore()
		{
			string result;
			try
			{
				string cipherString = SAFConfiguration.readConnectionStringCoreEncrypted();
				string text = CryptorEngineTripleDES.Decrypt(cipherString, SAFSecurityKeys.getSecurityInfoFromWConfig(), true);
				result = text;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.DBConnectionString::ExpandSAFCore[]\r\n" + ex.Message, null);
				result = null;
			}
			return result;
		}
	}
}
