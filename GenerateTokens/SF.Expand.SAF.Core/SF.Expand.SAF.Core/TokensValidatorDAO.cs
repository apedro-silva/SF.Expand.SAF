using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Utils;
using System;
using System.Data;
namespace SF.Expand.SAF.Core
{
	public class TokensValidatorDAO : DALSqlServer
	{
		private const string spGET_DEPLOY_ASSEMBLY_BY_TOKENID = "GetDeployAssemblyByTokenID";
		private const string spGET_VALIDATOR_ASSEMBLY_BY_TOKENID = "GetValidatorAssemblyByTokenID";
		private const string spGET_DEPLOY_ASSEMBLY_BY_TOKENVENDORID = "GetDeployAssemblyByTokenVendorID";
		private const string spGET_VALIDATOR_ASSEMBLY_BY_TOKENVENDORID = "GetValidatorAssemblyByTokenVendorID";
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
		private string _loadFromDB(string value, string sqlExec)
		{
			IDbCommand dbCommand = null;
			string result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand(sqlExec, CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param1", value));
				base.Connection.Open();
				object obj = dbCommand.ExecuteScalar();
				result = (string)obj;
			}
			catch (Exception logObject)
			{
				LOGGER.Write(LOGGER.LogCategory.EXCEPTION, "SF.Expand.SAF.Core.TokensDAO::_loadFromDB[" + sqlExec + "]", logObject);
				result = null;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
