using SF.Expand.SAF.Utils;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace SF.Expand.SAF.Configuration
{
	public static class SAFConfiguration
	{
		private const int cDefaultCacheSecs = 5;
		private const string _cSAFDBaseConnection = "SAFDBaseConnection";
		private const string _cSAFDBaseBusinessConnection = "SAFDBaseBusinessConnection";
		private const string _cSAFMasterKey = "SAFMasterKey";
		private const string _cSAFSecurityKey = "SecurityKey";
		private const string _cSAFSecurityIV = "SecurityIV";
		public const string cSAFSmsGatewayService = "SAFSmsGatewayService";
		public const string cSAFSmsAssemblyProcessor = "SAFSmsAssemblyProcessor";
		public const string cSAFConfigurationKeyParsPath = "SAFKeyParsFilePath";
		public const string cSAFSMSDefaultGateway = "SAFSMSDefaultGateway";
		public const string cSAFSMTPDefaultGateway = "SAFSMTPDefaultGateway";
		public const string cSAFClientBusinessRules = "SAFClientBusinessRules";
		private const string _expandSAFConfSection = "SF.Expand/Expand.SAF";
		private const string _expandSAFConfSectionExternal = "SF.Expand/Expand.SAF.External";
		public static string readMasterKey()
		{
			return SAFConfiguration.readParameterAppConfigBase("SAFMasterKey").Trim();
		}
		public static string readInfoKey()
		{
			return SAFConfiguration.readParameterAppConfigBase("SecurityKey").Trim();
		}
		public static string readInfoIV()
		{
			return SAFConfiguration.readParameterAppConfigBase("SecurityIV").Trim();
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
		public static int updateParameterBusiness(string userName, string parameterName, string parameterValue)
		{
			return SAFConfiguration._updateParameterBusiness(userName, parameterName, parameterValue);
		}
		private static int _updateParameterBusiness(string userName, string parameterName, string parameterValue)
		{
			int result = 0;
			try
			{
				string connectionString = SAFConfiguration.readConnectionStringBusiness();
				using (IDbConnection dbConnection = new SqlConnection(connectionString))
				{
					dbConnection.Open();
					using (IDbCommand dbCommand = dbConnection.CreateCommand())
					{
						dbCommand.CommandType = CommandType.StoredProcedure;
						dbCommand.CommandText = "SetConfigurationParameter";
						dbCommand.Parameters.Add(new SqlParameter("@UserName", userName));
						dbCommand.Parameters.Add(new SqlParameter("@ParameterName", parameterName));
						dbCommand.Parameters.Add(new SqlParameter("@ParameterValue", parameterValue));
						result = (int)dbCommand.ExecuteScalar();
					}
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::_updateParameterBusiness failed. Err:" + ex.Message, ex);
			}
			SAFConfiguration._CheckCache(0);
			return result;
		}
		private static SAFConfigurationParametersMap _CheckCache(int cacheSecs)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			SAFConfigurationParametersMap sAFConfigurationParametersMap = currentDomain.GetData("ExpandSAFConfigurationDomainCache") as SAFConfigurationParametersMap;
			bool flag = false;
			if (sAFConfigurationParametersMap == null)
			{
				flag = true;
			}
			else
			{
				double totalSeconds = (DateTime.UtcNow - sAFConfigurationParametersMap.LastUpdate).TotalSeconds;
				if (totalSeconds > (double)cacheSecs || totalSeconds < 0.0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				sAFConfigurationParametersMap = SAFConfiguration.GetAllParametersFromDB();
				if (sAFConfigurationParametersMap == null)
				{
					return null;
				}
				sAFConfigurationParametersMap.LastUpdate = DateTime.UtcNow;
				currentDomain.SetData("ExpandSAFConfigurationDomainCache", sAFConfigurationParametersMap);
			}
			return sAFConfigurationParametersMap;
		}
		private static string _readParameterBusiness(string parameterFullName, int cacheSecs)
		{
			SAFConfigurationParametersMap sAFConfigurationParametersMap = SAFConfiguration._CheckCache(cacheSecs);
			if (sAFConfigurationParametersMap == null)
			{
				return null;
			}
			string result;
			try
			{
				result = sAFConfigurationParametersMap[parameterFullName].value;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::readParameterBusinessChangeable error getting parameter[" + parameterFullName + "]. Err:" + ex.Message, ex);
				result = null;
			}
			return result;
		}
		private static SAFConfigurationParametersMap GetAllParametersFromDB()
		{
			SAFConfigurationParametersMap sAFConfigurationParametersMap = new SAFConfigurationParametersMap();
			SAFConfigurationParametersMap result;
			try
			{
				string connectionString = SAFConfiguration.readConnectionStringBusiness();
				using (IDbConnection dbConnection = new SqlConnection(connectionString))
				{
					dbConnection.Open();
					using (IDbCommand dbCommand = dbConnection.CreateCommand())
					{
						dbCommand.CommandType = CommandType.StoredProcedure;
						dbCommand.CommandText = "GetAllConfiguration";
						IDataReader dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
						while (dataReader.Read())
						{
							SAFConfigurationParameter sAFConfigurationParameter = new SAFConfigurationParameter();
							sAFConfigurationParameter.section = dataReader.GetString(0);
							sAFConfigurationParameter.name = dataReader.GetString(1);
							sAFConfigurationParameter.value = (dataReader.IsDBNull(2) ? null : dataReader.GetString(2));
							sAFConfigurationParameter.lastUTCupdate = dataReader.GetDateTime(3);
							sAFConfigurationParameter.frozen = dataReader.GetBoolean(4);
							sAFConfigurationParameter.hidden = dataReader.GetBoolean(5);
							string key = sAFConfigurationParameter.section + "@" + sAFConfigurationParameter.name;
							sAFConfigurationParametersMap.Add(key, sAFConfigurationParameter);
						}
					}
				}
				result = sAFConfigurationParametersMap;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::GetAllParameters failed. Err:" + ex.Message, ex);
				result = null;
			}
			return result;
		}
		public static string readParameter(string parameter)
		{
			string result;
			try
			{
				result = SAFConfiguration.readParameterBusinessSection(parameter);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration.readParameter. readParameterBusinessSection failed. Err:" + ex.Message, ex);
				result = null;
			}
			return result;
		}
		public static string readParameterExternal(string parameter)
		{
			string result;
			try
			{
				result = SAFConfiguration.readParameterBusinessExternalSection(parameter);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration.readParameterExternal. readParameterBusinessExternalSection failed. Err:" + ex.Message, ex);
				result = null;
			}
			return result;
		}
		public static string readParameterAppConfigBase(string parameter)
		{
			return SAFConfiguration.readParameterAppConfigSection("SF.Expand/Expand.SAF", parameter);
		}
		public static string readParameterAppConfigExternal(string parameter)
		{
			return SAFConfiguration.readParameterAppConfigSection("SF.Expand/Expand.SAF.External", parameter);
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
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Configuration.SAFConfiguration.readParameterAppConfigSection[S:",
					section,
					",P:",
					parameter,
					"] Failed. Msg:",
					ex.Message
				}), ex);
				result = null;
			}
			return result;
		}
	}
}
