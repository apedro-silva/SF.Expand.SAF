using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using SF.Expand.Secure.Business;
using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;
namespace SF.Expand.SAF.DeployToken
{
	public static class HttpDeployProcessor
	{
		private const string cMODULE_NAME = "SAFBUSINESSDEPLOY";
		private const string cBASE_NAME = "http://sfexpand.SAFDeploy.DEPLOYJ1JAVAINFOSRV.softfinanca.com/";
		private const string cHTTPREQUEST_SERVER_KEYS = "HTTPREQUEST_SERVER_KEYS";
		private const string cTOKENDEPLOY_SERVICE_PUBLICNAME = "TOKENDEPLOY_SERVICE_PUBLICNAME";
		private static OperationResult _processTokenEvents(string tokenID, string appUserID, string httpRequestFields, out long tkEventID)
		{
			OperationResult result;
			if (OperationResult.Success == new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 107, 0, appUserID, out tkEventID))
			{
				if (OperationResult.Success == new TokensBusinessEventsDAO().insertTokenEventFields(tkEventID, httpRequestFields))
				{
					result = OperationResult.Success;
					return result;
				}
			}
			result = OperationResult.Error;
			return result;
		}
		public static string processRequest(object httpContext)
		{
			StringBuilder _inputParameters = new StringBuilder();
			if (httpContext == null || httpContext.GetType() != typeof(HttpContext))
			{
				throw new Exception("Object type not supported");
			}
			string result;
			try
			{
				HttpContext context = (HttpContext)httpContext;
				_inputParameters.AppendFormat("ID|{0}", context.Request.QueryString.Get(SAFConfiguration.readParameterExternal("TOKENDEPLOY_SERVICE_PUBLICNAME")));
				string[] _confServerVarsKeys = SAFConfiguration.readParameterExternal("HTTPREQUEST_SERVER_KEYS").Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < _confServerVarsKeys.Length; i++)
				{
					_inputParameters.AppendFormat("|{0}|{1}", _confServerVarsKeys[i], context.Request.ServerVariables.Get(_confServerVarsKeys[i]));
				}
				result = HttpDeployProcessor.processRequest(_inputParameters.ToString());
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DEPLOYJ1JAVAINFOSRV.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
			}
			return result;
		}
		public static string processRequest(string httpParamters)
		{
			string appBase64Data = null;
			string appContentType = null;
			string _deployProcessorTypeName = null;
			long requestDeployEventID = 0L;
			string[] _inParams = httpParamters.Split(new char[]
			{
				'|'
			});
			string result;
			if (_inParams.Length % 2 != 0 || _inParams.Length < 4)
			{
				result = null;
			}
			else
			{
				DataTable _dTableEvent;
				if (OperationResult.Error == new TokensBusinessEventsDAO().loadDeployEventInfo(_inParams[1], out _dTableEvent))
				{
					result = null;
				}
				else
				{
					if (_dTableEvent.Rows.Count != 1)
					{
						result = null;
					}
					else
					{
						TokenCryptoData _tokenCryptoData = new TokensDAO().loadTokenCryptoData(_dTableEvent.Rows[0][0].ToString());
						if (_tokenCryptoData.ID == null)
						{
							result = null;
						}
						else
						{
							if (OperationResult.Success != HttpDeployProcessor._processTokenEvents(_dTableEvent.Rows[0][0].ToString(), _dTableEvent.Rows[0][2].ToString(), httpParamters, out requestDeployEventID))
							{
								result = null;
							}
							else
							{
								new DeployTokenDAO().loadDeployProcessor(_dTableEvent.Rows[0][1].ToString(), _inParams[3].Trim(), out _deployProcessorTypeName);
								if (_deployProcessorTypeName == null)
								{
									new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), 1);
									result = null;
								}
								else
								{
									try
									{
										IDeployToken _deployProcessor = DeployTokenFactory.LoadAssembly(_deployProcessorTypeName);
										if (OperationResult.Success == _deployProcessor.AssembleTokenApplication(BaseFunctions.HexDecoder(_tokenCryptoData.CryptoData.SupportCryptoData), out appContentType, out appBase64Data))
										{
											if (OperationResult.Success == SAFBaseFunctions.tokenEnable(_dTableEvent.Rows[0][2].ToString(), _dTableEvent.Rows[0][0].ToString(), null))
											{
												result = appContentType + "|" + appBase64Data;
												return result;
											}
										}
										new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), 1);
										result = null;
									}
									catch (Exception ex)
									{
										SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
										{
											"http://sfexpand.SAFDeploy.DEPLOYJ1JAVAINFOSRV.softfinanca.com/",
											Assembly.GetExecutingAssembly().FullName.ToString(),
											ex.ToString()
										});
										new TokensBusinessEventsDAO().updateEventStatus(requestDeployEventID.ToString(), 1);
										result = null;
									}
									finally
									{
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
