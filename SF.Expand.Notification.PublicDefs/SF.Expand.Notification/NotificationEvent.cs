using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public class NotificationEvent
	{
		public long BusinessEventID;
		public string BusinessApplicationID;
		public string NotificationFrom;
		public string NotificationDestination;
		public string NotificationMessageSubject;
		public string NotificationMessageContent;
		public NotificationChannel NotificationChannelInfo;
		public static NotificationEvent loadNotificationEvent(long businessEventID, string businessApplicationID, string notificationFrom, string notificationDestination, string notificationMessageSubject, string notificationMessageContent, NotificationChannel notificationChannelInfo)
		{
			return new NotificationEvent
			{
				BusinessEventID = businessEventID,
				BusinessApplicationID = businessApplicationID,
				NotificationFrom = notificationFrom,
				NotificationDestination = notificationDestination,
				NotificationMessageSubject = notificationMessageSubject,
				NotificationMessageContent = notificationMessageContent,
				NotificationChannelInfo = notificationChannelInfo
			};
		}
	}
}
