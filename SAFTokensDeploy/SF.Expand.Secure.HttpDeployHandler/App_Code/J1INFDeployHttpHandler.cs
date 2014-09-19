

using System;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Web.Services.Protocols;

using SF.Expand.Secure;


namespace SF.Expand.Secure.HttpDeployHandler
{
    public class J1INFDeployHttpHandler : IHttpHandler
    {
        private const string NEW_LINE = @"\r\n";
        private const string _DeployHandlerName = "J1INFDeployHttpHandler";
        private const string _baseName = @"http://sfexpandsecure.httpdeployhandler.softfinanca.com//";
        private const string cHTTPREQUEST_SERVER_KEYS = "HTTPREQUEST_SERVER_KEYS";
        private const string cTOKENDEPLOY_URLSERVICE = "TOKENDEPLOY_URLSERVICE";
        private const string cTOKENDEPLOY_URLSERVICE_TIMEOUT = "TOKENDEPLOY_URLSERVICE_TIMEOUT";
        private const string cTOKENDEPLOY_SERVICE_PUBLICNAME = "TOKENDEPLOY_SERVICE_PUBLICNAME";
        private const string cFRENDLY_ERRORMSG_TEMPLATE = "<h1><font color=red>{0}</font></h1><br><b>{1}</b></br>";


        /// <summary>
        /// </summary>
        /// <param name="serverrUrl"></param>
        /// <param name="serverTimeout"></param>
        /// <returns></returns>
        private string[] _callDeployService(string _requestID, string deployServiceParams)
        {
            SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.INFORMATION, _requestID, deployServiceParams);

            int _serverTimeout = int.Parse(ConfigurationManager.AppSettings.Get(cTOKENDEPLOY_URLSERVICE_TIMEOUT));
            String _serverrUrl = ConfigurationManager.AppSettings.Get(cTOKENDEPLOY_URLSERVICE);

            SAFBusinessSrv.SAFBusinessServices proxy = new SAFBusinessSrv.SAFBusinessServices();
            proxy.Url = _serverrUrl;
            proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            proxy.Timeout = _serverTimeout > 15000 ? _serverTimeout : 15000;
            proxy.SoapVersion = SoapProtocolVersion.Soap11;

            SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.INFORMATION, _requestID, "[" + proxy.Timeout.ToString() + "Sec] " + _serverrUrl);

            try
            {
                string _responseDeployWebMethod = proxy.loadDeploymentInfo(deployServiceParams);
                if (_responseDeployWebMethod == null)
                {
                    SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.ERROR, _requestID, "Error on deploy service>>null value return");
                    _responseDeployWebMethod = "text/html|" + string.Format(cFRENDLY_ERRORMSG_TEMPLATE, "Ocorreu um na descarga do seu token móvel", "Contacte os nossos serviços de apoio: 800 999 999");
                }
                return _responseDeployWebMethod.Split('|');
            }
            catch (Exception ex)
            {
                SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.ERROR, _requestID, ex.Message);
                return null;
            }
            finally
            {
                if (proxy != null)
                {
                    proxy.Dispose();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string processServerVariables(string _requestID, HttpContext context)
        {
            String[] _confServerVarsKeys = null;
            StringBuilder _inputParameters = new StringBuilder();

            _inputParameters.AppendFormat("{0}|{1}", ConfigurationManager.AppSettings.Get(cTOKENDEPLOY_SERVICE_PUBLICNAME), context.Request.QueryString.Get(ConfigurationManager.AppSettings.Get(cTOKENDEPLOY_SERVICE_PUBLICNAME)));
            _inputParameters.AppendFormat("|HTTP_REQUEST_USERAGENT|{0}", context.Request.UserAgent);
            _inputParameters.AppendFormat("|HTTP_HANDLER_NAME|{0}", context.CurrentHandler.ToString());
            _inputParameters.AppendFormat("|HTTP_REQUEST_URI|{0}", context.Request.Url.OriginalString);
            _inputParameters.AppendFormat("|PRIV_REQUEST_ID|{0}", _requestID);

            _confServerVarsKeys = ConfigurationManager.AppSettings.Get(cHTTPREQUEST_SERVER_KEYS).Split('|');
            for (int i = 0; i < _confServerVarsKeys.Length; i++)
            {
                _inputParameters.AppendFormat("|{0}|{1}", _confServerVarsKeys[i], context.Request.ServerVariables.Get(_confServerVarsKeys[i]));
            }
            return _inputParameters.ToString();
        }


        /// <summary>
        /// </summary>
        public bool IsReusable { get { return false; } }


        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            DateTime startTime = DateTime.Now;
            string _requestID = Guid.NewGuid().ToString("N");
            string[] _arrayReponseDeployWebMethod = null;

            SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.INFORMATION, _requestID, context.Request.Url.ToString());

            try
            {
                _arrayReponseDeployWebMethod = _callDeployService(_requestID, processServerVariables(_requestID, context));
                SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.INFORMATION, _requestID, "[" + _arrayReponseDeployWebMethod[0].Trim() + "] " + "data recivied from deploy service");

                switch (_arrayReponseDeployWebMethod[0].Trim())
                {
                    case "text/html":
                        context.Response.ContentType = _arrayReponseDeployWebMethod[0];
                        context.Response.Write(_arrayReponseDeployWebMethod[1]);
                        break;

                    case "application/java-archive":
                        context.Response.ContentType = _arrayReponseDeployWebMethod[0];
                        context.Response.AppendHeader("content-disposition", "attachment;filename=token.jar");
                        context.Response.BinaryWrite(Convert.FromBase64String(_arrayReponseDeployWebMethod[1]));
                        break;

                    case "application/vnd.ms-cab-compressed":
                        context.Response.ContentType = _arrayReponseDeployWebMethod[0];
                        context.Response.AppendHeader("content-disposition", "attachment;filename=token.cab");
                        context.Response.BinaryWrite(Convert.FromBase64String(_arrayReponseDeployWebMethod[1]));
                        break;

                    default:
                        SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.ERROR, _requestID, "Invalid data returned from deployment server");
                        throw new Exception("Não foi possivel precessar o pedido");
                }
            }
            catch (Exception ex)
            {
                string _errMsg = ex.Message;
                context.Response.ContentType = "text/html";
                context.Response.Write(string.Format(cFRENDLY_ERRORMSG_TEMPLATE, (object)_baseName, (object)_errMsg));
                SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.ERROR, _requestID, _errMsg);
            }
            finally
            {
                _arrayReponseDeployWebMethod = null;
                SAFTokenDeployLogger.write(_DeployHandlerName, SAFTokenDeployLogger.HandlerLoggerCategory.INFORMATION, _requestID, "::Deployment Terminate [" + ((TimeSpan)(DateTime.Now - startTime)).TotalSeconds.ToString() + " sec]::");
                context.Response.End();
            }
        }
    }
}