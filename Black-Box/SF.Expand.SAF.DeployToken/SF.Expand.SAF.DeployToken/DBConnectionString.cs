using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using System;
using System.Reflection;
namespace SF.Expand.SAF.DeployToken
{
	public static class DBConnectionString
	{
		private const string cMODULE_NAME = "SAFBUSINESSDEPLOY";
		private const string cBASE_NAME = "http://sfexpand.SAFDeploy.DBConnectionString.softfinanca.com/";
		public static string ExpandSecureDeployTokens()
		{
			string result;
			try
			{
				result = SAFConfiguration.readConnectionStringBusiness();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
	}
}
