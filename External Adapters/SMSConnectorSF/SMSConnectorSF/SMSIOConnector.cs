using SF.Expand.Notification;
using System;
using System.IO;
using System.Text;
namespace SMSConnectorSF
{
	public class SMSIOConnector : INotifyChannelProcessor
	{
		private const string cMODULE_NAME = "SMSIOConnectorSF";
		private const string cBASE_NAME = "http://sfexpand.SMSConnector.SMSIOConnectorSF.softfinanca.com/";
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
				string _fnane = _gatewayParams[0].Trim() + DateTime.Now.ToString("yyyyMMdd$HHmmss.fff!") + notificationEvent.NotificationDestination.Trim() + _gatewayParams[1].Trim();
				StreamWriter sw = new StreamWriter(_fnane, false, Encoding.ASCII);
				sw.AutoFlush = true;
				sw.Write(notificationEvent.NotificationMessageSubject.Trim());
				sw.Close();
				result = NotifyOperationResult.Success;
			}
			catch
			{
				result = NotifyOperationResult.Error;
			}
			return result;
		}
	}
}
