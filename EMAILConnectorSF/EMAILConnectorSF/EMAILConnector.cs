using SF.Expand.LOG;
using SF.Expand.Notification;
using System;
namespace EMAILConnectorSF
{
	public class EMAILConnector : INotifyChannelProcessor
	{
		private const string cMODULE_NAME = "EMAILCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.EMAILConnectorSF.EMAILConnector.softfinanca.com/";
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
					notificationEvent.NotificationMessageContent,
					notificationEvent.NotificationFrom,
					"http://sfexpand.EMAILConnectorSF.EMAILConnector.softfinanca.com/"
				}))
				{
					SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.INFORMATION, "EMAILCONNECTOR", new string[]
					{
						"http://sfexpand.EMAILConnectorSF.EMAILConnector.softfinanca.com/",
						"[OK] Message sended to destination:" + notificationEvent.NotificationDestination.Trim()
					});
					result = NotifyOperationResult.Success;
				}
				else
				{
					SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.ERROR, "EMAILCONNECTOR", new string[]
					{
						"http://sfexpand.EMAILConnectorSF.EMAILConnector.softfinanca.com/",
						"[NOK] Error wile send message to destination:" + notificationEvent.NotificationDestination.Trim()
					});
					result = NotifyOperationResult.Error;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.EXCEPTION, "EMAILCONNECTOR", new string[]
				{
					"http://sfexpand.EMAILConnectorSF.EMAILConnector.softfinanca.com/",
					"[NOK] Error wile send message to destination:" + notificationEvent.NotificationDestination.Trim(),
					ex.ToString()
				});
				result = NotifyOperationResult.Error;
			}
			return result;
		}
	}
}
