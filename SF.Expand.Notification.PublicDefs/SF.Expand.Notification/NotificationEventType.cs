using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public class NotificationEventType
	{
		public int NotificationEventTypeID;
		public string NotificationEventDescription;
		public int NotificationChannelTrahHoldEventsCount;
		public int NotificationChannelTrahHoldEventsTime;
		public int NotificationEventTypeReserved;
		public static NotificationEventType loadNotificationEventType(int notificationEventTypeID, string notificationEventDescription, int notificationChannelTrahHoldEventsCount, int notificationChannelTrahHoldEventsTime, int notificationEventTypeReserved)
		{
			return new NotificationEventType
			{
				NotificationEventTypeID = notificationEventTypeID,
				NotificationEventDescription = notificationEventDescription,
				NotificationChannelTrahHoldEventsCount = notificationChannelTrahHoldEventsCount,
				NotificationChannelTrahHoldEventsTime = notificationChannelTrahHoldEventsTime,
				NotificationEventTypeReserved = notificationEventTypeReserved
			};
		}
	}
}
