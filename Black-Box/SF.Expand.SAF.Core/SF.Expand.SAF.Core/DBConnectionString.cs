using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CryptoEngine;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public static class DBConnectionString
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.dbConnectionString.softfinanca.com/";
		public static string ExpandSAFCore()
		{
			string result;
			try
			{
				result = CryptorEngineTripleDES.Decrypt(SAFConfiguration.readConnectionStringCoreEncrypted(), new SecurityInfo(SAFConfiguration.readMasterKey(), SAFConfiguration.readInfoKey(), SAFConfiguration.readInfoIV()), true);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.dbConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
	}
}
