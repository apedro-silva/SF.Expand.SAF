using SAFConfigurationDAO;
using SF.Expand.LOG;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Configuration
{
	public class SAFConfigurationDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusinessConfig.SAFConfigurationDAO.softfinanca.com/";
		private const string cGETALLCONFIGURATION = "SAFGetConfiguration";
		private const string cSETCONFIGURATIONPARAMETER = "SAFSetConfigurationParameter";
		public SAFConfigurationParametersMap GetAllParametersFromDB()
		{
			IDbCommand _cmd = null;
			IDataReader _rd = null;
			SAFConfigurationParametersMap cacheData = new SAFConfigurationParametersMap();
			SAFConfigurationParametersMap result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFBusinessConfiguration();
				_cmd = base.CreateCommand("SAFGetConfiguration", CommandType.StoredProcedure);
				base.Connection.Open();
				_rd = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_rd.Read())
				{
					SAFConfigurationParameter par = new SAFConfigurationParameter();
					par.section = _rd.GetString(0);
					par.name = _rd.GetString(1);
					par.value = (_rd.IsDBNull(2) ? null : _rd.GetString(2));
					par.lastUTCupdate = _rd.GetDateTime(3);
					par.frozen = _rd.GetBoolean(4);
					par.hidden = _rd.GetBoolean(5);
					cacheData.Add(par.section + "@" + par.name, par);
				}
				result = cacheData;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusinessConfig.SAFConfigurationDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
				if (_rd != null)
				{
					_rd.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public int UpdateParameterBusiness(string parameterName, string parameterValue)
		{
			IDbCommand _cmd = null;
			int result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFBusinessConfiguration();
				_cmd = base.CreateCommand("SAFSetConfigurationParameter", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@UserName", null));
				_cmd.Parameters.Add(base.AddParameter("@ParameterName", parameterName));
				_cmd.Parameters.Add(base.AddParameter("@ParameterValue", parameterValue));
				base.Connection.Open();
				result = (int)_cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusinessConfig.SAFConfigurationDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = -1;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
