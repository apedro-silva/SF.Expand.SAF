

using System;
using System.Data;
using System.Web;
using System.Text;

using SF.Expand.SAF.Core;
using SF.Expand.SAF.Defs;
using SF.Expand.Secure.Business;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Configuration;
using SF.Expand.Secure;


namespace SF.Expand.SAF.DeployToken
{
    public static class HttpDeployProcessor
    {
        private const string NEW_LINE = @"\r\n";
        private const string _className = "HttpDeployProcessor";
        private const string _baseName = @"http://sfexpandsecure.deploytoken.softfinanca.com//";
        private const string cHTTPREQUEST_SERVER_KEYS = "HTTPREQUEST_SERVER_KEYS";
        private const string cTOKENDEPLOY_SERVICE_PUBLICNAME = "TOKENDEPLOY_SERVICE_PUBLICNAME";




        /// <summary>
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="appUserID"></param>
        /// <param name="httpRequestFields"></param>
        /// <param name="tkEventID"></param>
        /// <returns></returns>
        private static OperationResult _processTokenEvents(string tokenID, string appUserID, string httpRequestFields, out long tkEventID)
        {
            if (OperationResult.Success == new TokensBusinessEventsDAO().insertTokenEvent(tokenID, (int)TokenEventOperation.cTOKEN_HTTP_DEPLOY_REQUESTED, 0, appUserID, out tkEventID))
            {
                if (OperationResult.Success == new TokensBusinessEventsDAO().insertTokenEventFields(tkEventID, httpRequestFields))
                {
                    return OperationResult.Success;
                }
            }
            return OperationResult.Error;
        }

        
        
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string processRequest(object httpContext)
        {
            String[] _confServerVarsKeys = null;
            StringBuilder _inputParameters = new StringBuilder();

            if ((httpContext == null) ||(httpContext.GetType() != typeof(HttpContext)))
            {
                throw new Exception("Object type not supported");
            }

            try
            {
                HttpContext context = (HttpContext)httpContext;
                _inputParameters.AppendFormat("ID|{0}", context.Request.QueryString.Get(SAFConfiguration.readParameterExternal(cTOKENDEPLOY_SERVICE_PUBLICNAME)));
                _confServerVarsKeys = SAFConfiguration.readParameterExternal(cHTTPREQUEST_SERVER_KEYS).Split('|');

                for (int i = 0; i < _confServerVarsKeys.Length; i++)
                {
                    _inputParameters.AppendFormat("|{0}|{1}", _confServerVarsKeys[i], context.Request.ServerVariables.Get(_confServerVarsKeys[i]));
                }

                return processRequest(_inputParameters.ToString());
            }
            catch
            {
                return null;
            }
            finally
            {
                _confServerVarsKeys = null;
                _inputParameters = null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="httpParamters"></param>
        /// <returns></returns>
        public static string processRequest(string httpParamters)
        {
            string appBase64Data = null;
            string appContentType = null;
            string _deployProcessorTypeName = null;
            long requestDeployEventID = 0;

            DataTable _dTableEvent;
            IDeployToken _deployProcessor = null;

            string[] _inParams = httpParamters.Split(new char[] { '|' });
            if (_inParams.Length % 2 != 0 || _inParams.Length < 4)
            {
                return null;
            }

            if (OperationResult.Error == new TokensBusinessEventsDAO().loadDeployEventInfo(_inParams[1], out _dTableEvent))
            {
                return null;
            }
            if (_dTableEvent.Rows.Count != 1)
            {
                return null;
            }
            TokenCryptoData _tokenCryptoData = new TokensDAO().loadTokenCryptoData(_dTableEvent.Rows[0][0].ToString());
            if (_tokenCryptoData.ID == null)
            {
                return null;
            }
            if (OperationResult.Success != _processTokenEvents(_dTableEvent.Rows[0][0].ToString(), _dTableEvent.Rows[0][2].ToString(), httpParamters, out requestDeployEventID))
            {
                return null;
            }
            new DeployTokenDAO().loadDeployProcessor(_dTableEvent.Rows[0][1].ToString(), _inParams[3].Trim(), out _deployProcessorTypeName);
            if (_deployProcessorTypeName == null)
            {
                new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), (byte)1);
                return null;
            }
            try
            {
                _deployProcessor = DeployTokenFactory.LoadAssembly(_deployProcessorTypeName);
                if (OperationResult.Success == _deployProcessor.AssembleTokenApplication(BaseFunctions.HexDecoder(_tokenCryptoData.CryptoData.SupportCryptoData), out appContentType, out appBase64Data))
                {
                    if (OperationResult.Success == SAFBaseFunctions.tokenEnable(_dTableEvent.Rows[0][2].ToString(), _dTableEvent.Rows[0][0].ToString(), null))
                    {
                        return appContentType + "|" + appBase64Data;
                    }
                }

                new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), (byte)1);
                return null;
            }
            catch
            {
                new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), (byte)1);
                return null;
            }
            finally
            {
                _deployProcessor = null;
            }
        }
    }
}
