using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.DeployToken
{
	public class DeployTokenDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFBUSINESSDEPLOY";
		private const string cBASE_NAME = "http://sfexpand.SAFDeploy.DeployTokenDAO.softfinanca.com/";
		private const string cLOAD_CUSTOM_TEMPLATE_BY_USERAGENT = "SELECT tokenDeployProcessorTypeName FROM tokensDeployProcessor WITH (NOLOCK) WHERE tokenDeployProcessorID=(SELECT TOP(1) tokenDeployProcessorID FROM tokensDeployProcessorHttpRules WITH (NOLOCK) WHERE tokenParamsID=@param0 AND @param1 LIKE '%' + httpRequestUserAgentLike + '%')";
		private const string cLOAD_DEFAULT_TEMPLATE_BY_TOKENPARAMSID = "SELECT TOP(1) tokenDeployProcessorTypeName FROM tokensDeployProcessor WITH (NOLOCK) WHERE tokenParamsID=@param0 AND tokenDeployProcessorDefault=1";
		public OperationResult loadDeployProcessor(string tokenParamsID, string httpUserAgent, out string deployProcessor)
		{
			deployProcessor = null;
			IDbCommand _cmd = null;
			IDbCommand _cmdDefault = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureDeployTokens();
				_cmd = base.CreateCommand("SELECT tokenDeployProcessorTypeName FROM tokensDeployProcessor WITH (NOLOCK) WHERE tokenDeployProcessorID=(SELECT TOP(1) tokenDeployProcessorID FROM tokensDeployProcessorHttpRules WITH (NOLOCK) WHERE tokenParamsID=@param0 AND @param1 LIKE '%' + httpRequestUserAgentLike + '%')", CommandType.Text);
				_cmd.Parameters.Add(base.AddParameter("@param0", tokenParamsID));
				_cmd.Parameters.Add(base.AddParameter("@param1", httpUserAgent));
				base.Connection.Open();
				object _resp = _cmd.ExecuteScalar();
				if (_resp != null)
				{
					deployProcessor = _resp.ToString().Trim();
					result = OperationResult.Success;
				}
				else
				{
					_cmdDefault = base.CreateCommand("SELECT TOP(1) tokenDeployProcessorTypeName FROM tokensDeployProcessor WITH (NOLOCK) WHERE tokenParamsID=@param0 AND tokenDeployProcessorDefault=1", CommandType.Text);
					_cmdDefault.Parameters.Add(base.AddParameter("@param0", tokenParamsID));
					_resp = _cmdDefault.ExecuteScalar();
					if (_resp != DBNull.Value)
					{
						deployProcessor = _resp.ToString().Trim();
						result = OperationResult.Success;
					}
					else
					{
						result = OperationResult.Error;
					}
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DeployTokenDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				if (_cmdDefault != null)
				{
					_cmdDefault.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
