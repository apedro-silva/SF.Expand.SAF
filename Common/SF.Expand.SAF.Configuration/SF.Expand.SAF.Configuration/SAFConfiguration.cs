using SF.Expand.LOG;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
namespace SF.Expand.SAF.Configuration
{
	public static class SAFConfiguration
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusinessConfig.SAFConfiguration.softfinanca.com/";
		private const int cDefaultCacheSecs = 5;
		private const string cSAFCACHEID = "SAFConfigurationDomainCache";
		private const string _cSAFDBaseConnection = "SAFDBaseConnection";
		private const string _cSAFDBaseBusinessConnection = "SAFDBaseBusinessConnection";
		private const string _cSAFMasterKey = "SAFMasterKey";
		private const string _cSAFSecurityKey = "SecurityKey";
		private const string _cSAFSecurityIV = "SecurityIV";
		private const string _cSAFServerID = "ServerID";
		private const string _expandSAFConfSection = "SF.Expand/Expand.SAF";
		private const string _expandSAFConfSectionExternal = "SF.Expand/Expand.SAF.External";
		public const string cSAFAPPEventHandler = "SAFAPPEventHandler";
		public const string cSAFClientBusinessRules = "SAFClientBusinessRules";
		public const string cSAFSMSAssemblyProcessor = "SAFSMSAssemblyProcessor";
		public const string cSAFSMSDefaultGateway = "SAFSMSDefaultGateway";
		public const string cSAFSMSDefaultGatewayTimeout = "SAFSMSDefaultGatewayTimeout";
		public const string cSAFEMAILAssemblyProcessor = "SAFEMAILAssemblyProcessor";
		public const string cSAFEMAILDefaultGateway = "SAFEMAILDefaultGateway";
		public const string cSAFEMAILDefaultGatewayTimeout = "SAFEMAILDefaultGatewayTimeout";
		private static byte _serverID = SAFConfiguration.__serverID();
		private static SAFConfigurationParametersMap _CheckCache(int cacheSecs)
		{
			AppDomain domainCurr = AppDomain.CurrentDomain;
			bool needsUpdate = false;
			SAFConfigurationParametersMap result;
			try
			{
				SAFConfigurationParametersMap cacheData = (SAFConfigurationParametersMap)domainCurr.GetData("SAFConfigurationDomainCache");
				if (cacheData == null)
				{
					needsUpdate = true;
				}
				else
				{
					double totSecs = (DateTime.UtcNow - cacheData.LastUpdate).TotalSeconds;
					if (totSecs > (double)cacheSecs || totSecs < 0.0)
					{
						needsUpdate = true;
					}
				}
				if (needsUpdate)
				{
					cacheData = new SAFConfigurationDAO().GetAllParametersFromDB();
					if (cacheData == null)
					{
						result = null;
						return result;
					}
					cacheData.LastUpdate = DateTime.UtcNow;
					domainCurr.SetData("SAFConfigurationDomainCache", cacheData);
				}
				result = cacheData;
			}
			catch
			{
				result = null;
			}
			finally
			{
			}
			return result;
		}
		private static string readParameterAppConfigSection(string section, string parameter)
		{
			string result;
			try
			{
				result = ((NameValueCollection)ConfigurationManager.GetSection(section)).Get(parameter);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusinessConfig.SAFConfiguration.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					string.Concat(new string[]
					{
						"failed::[",
						section,
						"/",
						parameter,
						"]"
					}),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
		private static string _readParameterBusiness(string parameterFullName, int cacheSecs)
		{
			string result;
			try
			{
				SAFConfigurationParametersMap cacheData = SAFConfiguration._CheckCache(cacheSecs);
				if (cacheData == null || !cacheData.ContainsKey(parameterFullName))
				{
					result = null;
				}
				else
				{
					result = cacheData[parameterFullName].value;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusinessConfig.SAFConfiguration.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"failed::[" + parameterFullName + "]",
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
		public static string readMasterKey()
		{
			return SAFConfiguration.readParameterAppConfigBase("SAFMasterKey");
		}
		public static string readInfoKey()
		{
			return SAFConfiguration.readParameterAppConfigBase("SecurityKey");
		}
		public static string readInfoIV()
		{
			return SAFConfiguration.readParameterAppConfigBase("SecurityIV");
		}
		private static byte __serverID()
		{
			string sServerID = SAFConfiguration.readParameterAppConfigBase("ServerID");
			byte res;
			byte result;
			if (!byte.TryParse(sServerID, out res))
			{
				result = 0;
			}
			else
			{
				result = res;
			}
			return result;
		}
		public static byte readServerID()
		{
			return SAFConfiguration._serverID;
		}
		public static string readConnectionStringCoreEncrypted()
		{
			return SAFConfiguration.readParameterAppConfigBase("SAFDBaseConnection");
		}
		public static string readConnectionStringBusiness()
		{
			return SAFConfiguration.readParameterAppConfigBase("SAFDBaseBusinessConnection");
		}
		public static SAFConfigurationParametersMap readParameters()
		{
			return SAFConfiguration._CheckCache(0);
		}
		public static SAFConfigurationParametersMap readParameters(int cacheSecs)
		{
			return SAFConfiguration._CheckCache(cacheSecs);
		}
		public static string readParameterBusinessSection(string parameter)
		{
			return SAFConfiguration._readParameterBusiness("SF.Expand/Expand.SAF" + '@' + parameter, 5);
		}
		public static string readParameterBusinessExternalSection(string parameter)
		{
			return SAFConfiguration._readParameterBusiness("SF.Expand/Expand.SAF.External" + '@' + parameter, 5);
		}
		public static int updateParameterBusiness(string parameterName, string parameterValue)
		{
			return new SAFConfigurationDAO().UpdateParameterBusiness(parameterName, parameterValue);
		}
		public static string readParameterAppConfigBase(string parameter)
		{
			return SAFConfiguration.readParameterAppConfigSection("SF.Expand/Expand.SAF", parameter);
		}
		public static string readParameterAppConfigExternal(string parameter)
		{
			return SAFConfiguration.readParameterAppConfigSection("SF.Expand/Expand.SAF.External", parameter);
		}
		public static string readParameter(string parameter)
		{
			return SAFConfiguration.readParameterBusinessSection(parameter);
		}
		public static string readParameterExternal(string parameter)
		{
			return SAFConfiguration.readParameterBusinessExternalSection(parameter);
		}
	}
}
