

using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace SF.Expand.SAF.Utils
{
    public struct SecurityInfo
    {
        private byte[] _iv;
        private byte[] _key;
        private byte[] _masterKey;

        public byte[] MasterKey { get { return _masterKey; } }
        public byte[] Key { get { return _key; } }
        public byte[] Iv { get { return _iv; } }

        public SecurityInfo(string masterKey, string key, string iv)
        {
            Encoding _ascii = Encoding.ASCII;
            Encoding _unicode = Encoding.Unicode;
            _key = Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(key));
            _iv = Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(iv));
            _masterKey = Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(masterKey));
        }
        public SecurityInfo(byte[] masterKey, byte[] key, byte[] iv)
        {
            _key = key; _iv = iv; _masterKey = masterKey;
        }
    }

    [Serializable]
    public class SAFConfigurationParameter
    {
        public string section;
        public string name;
        public string value;
        public DateTime lastUTCupdate = DateTime.MinValue;
        public bool frozen = true;
        public bool hidden = true;
    }

    //public class SAFConfigurationParametersList : List<SAFConfigurationParameter>
    //{
    //    public static SAFConfigurationParametersList GetAllBusinessParameters()
    //    {

    //        System.Collections.Specialized.StringCollection rr = new StringCollection();
    //        //System.Collections.Specialized.
    //        //rr.
    //        return null;
    //    }
    //}

    public class SAFConfigurationParametersMap : SortedDictionary<string, SAFConfigurationParameter>
    {
        public DateTime LastUpdate = DateTime.MinValue;
    }

    public static class SAFConfiguration
    {
        private const int cDefaultCacheSecs = 5;

        private const string _cSAFDBaseConnection = "SAFDBaseConnection";
        private const string _cSAFDBaseBusinessConnection = "SAFDBaseBusinessConnection";

        private const string _cSAFMasterKey = "SAFMasterKey";
        private const string _cSAFSecurityKey = "SecurityKey";
        private const string _cSAFSecurityIV = "SecurityIV";
        private const string _cSAFServerID = "ServerID";

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
            return readParameterAppConfigBase(_cSAFMasterKey).Trim();
        }

        public static string readInfoKey()
        {
            return readParameterAppConfigBase(_cSAFSecurityKey).Trim();
        }

        public static string readInfoIV()
        {
            return readParameterAppConfigBase(_cSAFSecurityIV).Trim();
        }

        private static byte __serverID()
        {
            byte res;
            string sServerID = readParameterAppConfigBase(_cSAFServerID).Trim();
            if (!byte.TryParse(sServerID, out res))
                return 0;

            return res;
        }

        private static byte _serverID = __serverID();
        public static byte readServerID()
        {
            return _serverID;
        }

        public static string readConnectionStringCoreEncrypted()
        {
            return readParameterAppConfigBase(_cSAFDBaseConnection);
        }

        public static string readConnectionStringBusiness()
        {
            return readParameterAppConfigBase(_cSAFDBaseBusinessConnection);
        }

        public static SAFConfigurationParametersMap readParameters()
        {
            return _CheckCache(0);
        }

        public static SAFConfigurationParametersMap readParameters(int cacheSecs)
        {
            return _CheckCache(cacheSecs);
        }

        public static string readParameterBusinessSection(string parameter)
        {
            //default cache time of cDefaultCacheSecs secs
            return _readParameterBusiness(_expandSAFConfSection + '@' + parameter, cDefaultCacheSecs);
        }

        public static string readParameterBusinessExternalSection(string parameter)
        {
            //default cache time of cDefaultCacheSecs secs
            return _readParameterBusiness(_expandSAFConfSectionExternal + '@' + parameter, cDefaultCacheSecs);
        }

        public static int updateParameterBusiness(string userName, string parameterName, string parameterValue)
        {
            return _updateParameterBusiness(userName, parameterName, parameterValue);
        }

        private static int _updateParameterBusiness(string userName, string parameterName, string parameterValue)
        {
            int rowsAffected = 0;
            try
            {
                string connStr = SAFConfiguration.readConnectionStringBusiness();

                using (IDbConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SetConfigurationParameter";
                        command.Parameters.Add(new SqlParameter("@UserName", userName));
                        command.Parameters.Add(new SqlParameter("@ParameterName", parameterName));
                        command.Parameters.Add(new SqlParameter("@ParameterValue", parameterValue));
                        rowsAffected = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::_updateParameterBusiness failed. Err:" + ex.Message, ex);
            }

            //Refresh parameters
            _CheckCache(0);

            return rowsAffected;
        }

        private static SAFConfigurationParametersMap _CheckCache(int cacheSecs)
        {
            AppDomain domainCurr = AppDomain.CurrentDomain;
            SAFConfigurationParametersMap cacheData = domainCurr.GetData("ExpandSAFConfigurationDomainCache") as SAFConfigurationParametersMap;

            bool needsUpdate = false;
            if (cacheData == null)
            {
                needsUpdate = true;
            }
            else
            {
                TimeSpan elapsedSpan = DateTime.UtcNow - cacheData.LastUpdate;
                double totSecs = elapsedSpan.TotalSeconds;
                if (totSecs > (double)cacheSecs || totSecs < 0)
                    needsUpdate = true;
            }

            if (needsUpdate)
            {
                cacheData = GetAllParametersFromDB();
                if (cacheData == null)
                    return null;

                cacheData.LastUpdate = DateTime.UtcNow;
                domainCurr.SetData("ExpandSAFConfigurationDomainCache", cacheData);
            }

            return cacheData;
        }

        private static string _readParameterBusiness(string parameterFullName, int cacheSecs)
        {
            SAFConfigurationParametersMap cacheData = _CheckCache(cacheSecs);
            if (cacheData == null)
                return null;

            try
            {
                return (string)cacheData[parameterFullName].value;
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::readParameterBusinessChangeable error getting parameter[" +
                    parameterFullName + "]. Err:" + ex.Message, ex);
                return null;
            }
        }

        private static SAFConfigurationParametersMap GetAllParametersFromDB()
        {
            SAFConfigurationParametersMap cacheData = new SAFConfigurationParametersMap();

            try
            {
                string connStr = SAFConfiguration.readConnectionStringBusiness();

                using (IDbConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();

                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetAllConfiguration";

                        IDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                        while (dr.Read())
                        {
                            SAFConfigurationParameter par = new SAFConfigurationParameter();
                            par.section = dr.GetString(0);
                            par.name = dr.GetString(1);
                            par.value = dr.IsDBNull(2) ? null : dr.GetString(2);
                            par.lastUTCupdate = dr.GetDateTime(3);
                            par.frozen = dr.GetBoolean(4);
                            par.hidden = dr.GetBoolean(5);

                            string fullname = par.section + "@" + par.name;

                            cacheData.Add(fullname, par);
                        }
                    }
                }
                //All went fine
                return cacheData;
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration::GetAllParameters failed. Err:" + ex.Message, ex);
                return null;
            }
        }

        public static string readParameter(string parameter)
        {
            try
            {
                //return (string)((NameValueCollection)ConfigurationManager.GetSection(_expandSAFConfSection)).Get(parameter);

                return readParameterBusinessSection(parameter);
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration.readParameter. readParameterBusinessSection failed. Err:" + ex.Message, ex);
                return null;
            }
        }

        public static string readParameterExternal(string parameter)
        {
            try
            {
                //return (string)((NameValueCollection)ConfigurationManager.GetSection(_expandSAFConfSectionExternal)).Get(parameter);

                return readParameterBusinessExternalSection(parameter);
            }
            catch(Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration.readParameterExternal. readParameterBusinessExternalSection failed. Err:" + ex.Message, ex);
                return null;
            }
        }

        public static string readParameterAppConfigBase(string parameter)
        {
            return readParameterAppConfigSection(_expandSAFConfSection, parameter);
        }

        public static string readParameterAppConfigExternal(string parameter)
        {
            return readParameterAppConfigSection(_expandSAFConfSectionExternal, parameter);
        }

        private static string readParameterAppConfigSection(string section, string parameter)
        {
            try
            {
                return (string)((NameValueCollection)ConfigurationManager.GetSection(section)).Get(parameter);
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Configuration.SAFConfiguration.readParameterAppConfigSection[" +
                    "S:" + section + ",P:" + parameter + "] Failed. Msg:" + ex.Message, ex);
                return null;
            }
        }

    }

    public static class SAFSecurityKeys
    {
        public static SecurityInfo getSecurityInfoFromWConfig()
        {
            return new SecurityInfo(
                            SAFConfiguration.readMasterKey(),
                            SAFConfiguration.readInfoKey(),
                            SAFConfiguration.readInfoIV());
        }

        public static string loadKeysFromFile()
        {
            FileStream pFSmInput = null;

            try
            {
                pFSmInput = new FileStream(SAFConfiguration.readParameter(SAFConfiguration.cSAFConfigurationKeyParsPath), FileMode.Open, FileAccess.Read);
                byte[] pBytData = new byte[pFSmInput.Length];
                pFSmInput.Read(pBytData, 0, (int)pFSmInput.Length);
                string pStrData = Encoding.ASCII.GetString(pBytData);
                pFSmInput.Close();
                return pStrData;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (pFSmInput != null)
                    pFSmInput.Close();
            }
        }


        private static byte[] _prepareKey(string masterKey)
        {
            if (masterKey == null || masterKey.Length < 1)
            {
                return new byte[0];
            }

            try
            {
                long _ind = 0;
                long _nBoxLen = 255;
                long _keyLen = masterKey.Length;
                byte[] _box = new byte[_nBoxLen];

                Encoding _ascii = Encoding.ASCII;
                Encoding _unicode = Encoding.Unicode;

                byte[] _asciiBytes = Encoding.Convert(_unicode, _ascii, _unicode.GetBytes(masterKey));
                char[] _asciiChars = new char[_ascii.GetCharCount(_asciiBytes, 0, _asciiBytes.Length)];
                _ascii.GetChars(_asciiBytes, 0, _asciiBytes.Length, _asciiChars, 0);

                for (long count = 0; count < _nBoxLen; count++)
                {
                    _box[count] = (byte)count;
                }
                for (long count = 0; count < _nBoxLen; count++)
                {
                    _ind = (_ind + _box[count] + _asciiChars[count % _keyLen]) % _nBoxLen;
                    byte temp = _box[count];
                    _box[count] = _box[_ind];
                    _box[_ind] = temp;
                }
                return _box;
            }
            catch
            {
                return new byte[0];
            }
        }
    }
}
