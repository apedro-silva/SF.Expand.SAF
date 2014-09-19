using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using System;
using System.Reflection;
namespace SF.Expand.Secure.Business
{
	public static class DBConnectionString
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/";
		public static string ExpandSecureBusiness()
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
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
	}
}
