
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using SF.Expand.SAF.Utils;
using SF.Expand.SAF.CorePublicItf;


namespace SF.Expand.Secure.Business
{
    public class DeployTokenDAO : DALSqlServer
    {

        private const string NEW_LINE = @"\r\n";
        private const string _baseName = @"http://sfexpandsecure.business.deploytokenDAO.softfinanca.com::";

        private const string cLOAD_DEFAULT_TEMPLATE_BY_TOKENPARAMSID =
                "SELECT a.tokenDeployProcessorTypeName FROM tokensDeployProcessor AS a WITH (NOLOCK) " +
                "INNER JOIN tokensDeployProcessorHttpRules AS b ON a.tokenDeployProcessorID=b.tokenDeployProcessorID " +
                "WHERE a.tokenParamsID=@param0 AND a.tokenDeployProcessorDefault=1 GROUP BY a.tokenDeployProcessorTypeName";

        private const string cLOAD_CUSTOM_TEMPLATE_BY_USERAGENT =
                "SELECT a.tokenDeployProcessorTypeName FROM tokensDeployProcessor AS a WITH (NOLOCK) " +
                "INNER JOIN tokensDeployProcessorHttpRules AS b ON a.tokenDeployProcessorID=b.tokenDeployProcessorID " +
                "WHERE a.tokenParamsID=@param0 AND b.httpRequestUserAgentLike LIKE '%' + @param1 + '%' GROUP BY a.tokenDeployProcessorTypeName";


        /// <summary>
        /// </summary>
        /// <param name="tokenParamsID"></param>
        /// <param name="httpUserAgent"></param>
        /// <param name="deployProcessor"></param>
        /// <returns></returns>
        public OperationResult loadDeployProcessorByHttpUserAgent(string tokenParamsID, string httpUserAgent, out string deployProcessor)
        {
            object _resp = null;
            deployProcessor = null;
            IDbCommand _cmd = null;
            IDbCommand _cmdDefault = null;

            try
            {
                this.ConnectionString = DBConnectionString.ExpandSecureBusiness();
                _cmd = base.CreateCommand(cLOAD_CUSTOM_TEMPLATE_BY_USERAGENT, CommandType.Text);
                _cmd.Parameters.Add(base.AddParameter("@param0", (object)tokenParamsID));
                _cmd.Parameters.Add(base.AddParameter("@param1", (object)httpUserAgent));

                this.Connection.Open();
                _resp = (object)_cmd.ExecuteScalar();
                if (_resp != null)
                {
                    deployProcessor = _resp.ToString().Trim();
                    return OperationResult.Success;
                }
                _cmdDefault = base.CreateCommand(cLOAD_DEFAULT_TEMPLATE_BY_TOKENPARAMSID, CommandType.Text);
                _cmdDefault.Parameters.Add(base.AddParameter("@param0", (object)tokenParamsID));
                _resp = (object)_cmdDefault.ExecuteScalar();
                if (_resp != DBNull.Value)
                {
                    deployProcessor = _resp.ToString().Trim();
                    return OperationResult.Success;
                }
                return OperationResult.Error;

            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, _baseName + "loadDeployProcessorByHttpUserAgent[]" + NEW_LINE + ex.Message, null);
                return OperationResult.Error;
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
                this.CloseConnection();
            }
        }
    }
}