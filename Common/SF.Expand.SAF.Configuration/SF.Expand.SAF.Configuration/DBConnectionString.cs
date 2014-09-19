using SF.Expand.LOG;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Configuration
{
	public static class DBConnectionString
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusinessConfig.dbConnectionString.softfinanca.com/";
		public static string ExpandSAFBusinessConfiguration()
		{
			string result;
			try
			{
				result = SAFConfiguration.readConnectionStringBusiness();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusinessConfig.dbConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
	}
}
