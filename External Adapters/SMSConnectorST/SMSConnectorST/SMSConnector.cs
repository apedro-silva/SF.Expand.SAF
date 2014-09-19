using SF.Expand.LOG;
using SF.Expand.Notification;
using SMSConnectorST.SmsGW;
using System;
using System.Net;
using System.Reflection;
using System.Web.Services.Protocols;
namespace SMSConnectorST
{
	public class SMSConnector : INotifyChannelProcessor
	{
		private const string cMODULE_NAME = "SMSCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/";
		public NotifyOperationResult SendNotification(NotificationEvent notificationEvent, out int returnStatus)
		{
			returnStatus = 0;
			DateTime startTime = DateTime.Now;
			readFileBean proxy = new readFileBean();
			string[] _gatewayParams = null;
			SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.INFORMATION, "SMSCONNECTOR", new string[]
			{
				"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
				"from='" + notificationEvent.NotificationFrom + "'",
				"destination='" + notificationEvent.NotificationDestination,
				"message='" + notificationEvent.NotificationMessageContent
			});
			NotifyOperationResult result;
			if (notificationEvent.NotificationDestination == null || notificationEvent.NotificationDestination.Trim().Length < 9)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.ERROR, "SMSCONNECTOR", new string[]
				{
					"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"Invalid or inexistent phone number field!"
				});
				result = NotifyOperationResult.DestinationsToNotifyNotFound;
			}
			else
			{
				_gatewayParams = notificationEvent.NotificationChannelInfo.ChannelBaseParameters.Split(new char[]
				{
					'|'
				});
				string _tmpMsg = notificationEvent.NotificationMessageContent.Replace("\"", "%22").Replace("&", "%26").Replace("'", "%27").Replace("<", "%3C").Replace(">", "%3E");
				if (1 == _tmpMsg.CompareTo(notificationEvent.NotificationMessageContent))
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SMSCONNECTOR", new string[]
					{
						"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
						"not allowed characters found! MESSAGE CONTENT REPLACED!"
					});
				}
				if (_tmpMsg.Length > 160)
				{
					_tmpMsg = _tmpMsg.Substring(0, 160);
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SMSCONNECTOR", new string[]
					{
						"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
						" Message length exceeded 160 Chars! MESSAGE TRUNCATED!"
					});
				}
				try
				{
					proxy.Url = _gatewayParams[0];
					proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
					proxy.Timeout = ((notificationEvent.NotificationChannelInfo.ChannelTimout > 1000) ? notificationEvent.NotificationChannelInfo.ChannelTimout : 3000);
					proxy.SoapVersion = SoapProtocolVersion.Soap11;
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.INFORMATION, "SMSCONNECTOR", new string[]
					{
						"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
						"Service end point[" + _gatewayParams[1] + "]",
						"End point timeout[" + proxy.Timeout.ToString() + "]"
					});
					proxy.sendMobSMS(notificationEvent.NotificationDestination.Trim(), _tmpMsg);
					result = NotifyOperationResult.Success;
				}
				catch (Exception ex)
				{
					SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.ERROR, "SMSCONNECTOR", new string[]
					{
						"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						"ERROR! Message was not send!",
						ex.ToString()
					});
					result = ((ex.Message.IndexOf("timed out") > 0) ? NotifyOperationResult.ChannelProcessorError : NotifyOperationResult.ChannelProcessorError);
				}
				finally
				{
					SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.INFORMATION, "SMSCONNECTOR", new string[]
					{
						"http://sfexpand.SMSConnector.SMSConnectorST.softfinanca.com/",
						"Service :: [",
						_gatewayParams[0] + "] :: took [" + (DateTime.Now - startTime).TotalSeconds.ToString() + " sec]"
					});
					if (proxy != null)
					{
						proxy.Dispose();
					}
					proxy = null;
				}
			}
			return result;
		}
	}
}
