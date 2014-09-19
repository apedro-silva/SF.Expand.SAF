using SF.Expand.Notification;
using System;
namespace SMSConnectorSF
{
	public class SMSConnector : INotifyChannelProcessor
	{
		private const string cMODULE_NAME = "SMSCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.SMSConnector.SMSConnectorSF.softfinanca.com/";
		public NotifyOperationResult SendNotification(NotificationEvent notificationEvent, out int returnStatus)
		{
			returnStatus = 0;
			NotifyOperationResult result;
			try
			{
				string[] _gatewayParams = notificationEvent.NotificationChannelInfo.ChannelBaseParameters.Split(new char[]
				{
					'|'
				});
				if (MSMQHELPER.sendMQMessage(_gatewayParams[0], _gatewayParams[1], new string[]
				{
					notificationEvent.NotificationDestination,
					notificationEvent.NotificationMessageSubject,
					"http://sfexpand.SMSConnector.SMSConnectorSF.softfinanca.com/"
				}))
				{
					result = NotifyOperationResult.Success;
				}
				else
				{
					result = NotifyOperationResult.Error;
				}
			}
			catch
			{
				result = NotifyOperationResult.Error;
			}
			return result;
		}
	}
}
