using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public class TokensValidatorDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokensValidatorDAO.softfinanca.com/";
		private const string spGET_DEPLOY_ASSEMBLY_BY_TOKENID = "GetDeployAssemblyByTokenID";
		private const string spGET_VALIDATOR_ASSEMBLY_BY_TOKENID = "GetValidatorAssemblyByTokenID";
		private const string spGET_DEPLOY_ASSEMBLY_BY_TOKENVENDORID = "GetDeployAssemblyByTokenVendorID";
		private const string spGET_VALIDATOR_ASSEMBLY_BY_TOKENVENDORID = "GetValidatorAssemblyByTokenVendorID";
		private string _loadFromDB(string value, string sqlExec)
		{
			IDbCommand _cmd = null;
			string result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand(sqlExec, CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", value));
				base.Connection.Open();
				object res = _cmd.ExecuteScalar();
				result = (string)res;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensValidatorDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
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
		public string DeployAssemblyNameByTokenID(string tokenID)
		{
			return this._loadFromDB(tokenID, "GetDeployAssemblyByTokenID");
		}
		public string ValidatorAssemblyNameByTokenID(string tokenVendorID)
		{
			return this._loadFromDB(tokenVendorID, "GetValidatorAssemblyByTokenID");
		}
		public string DeployAssemblyNameByTokenParamsID(string tokenID)
		{
			return this._loadFromDB(tokenID, "GetDeployAssemblyByTokenVendorID");
		}
		public string ValidatorAssemblyNameByTokenParamsID(string tokenVendorID)
		{
			return this._loadFromDB(tokenVendorID, "GetValidatorAssemblyByTokenVendorID");
		}
	}
}
