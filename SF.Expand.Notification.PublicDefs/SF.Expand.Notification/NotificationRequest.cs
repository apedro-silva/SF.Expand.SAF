using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public class NotificationRequest
	{
		public long NotificationID;
		public long ApplicationEventID;
		public int ChannelID;
		public DateTime RequestDate;
		public DateTime SentDate;
		public DateTime ExpiredDate;
		public string Content;
		public string Subject;
		public string ToDestination;
		public string FromRequested;
		public int GatewayStatus;
		public static NotificationRequest loadNotificationRequest(long notifyRequestID, long applicationEventID, int notificationChannelID, DateTime notifyRequestDate, DateTime notifyRequestSentDate, DateTime notificationExpirationDate, int notificationGatewayStatus, string notifyDestination, string notifyMessage, string notificationMessageSubject, string notificationUserRequested)
		{
			return new NotificationRequest
			{
				NotificationID = notifyRequestID,
				ApplicationEventID = applicationEventID,
				ChannelID = notificationChannelID,
				RequestDate = notifyRequestDate,
				SentDate = notifyRequestSentDate,
				ExpiredDate = notificationExpirationDate,
				GatewayStatus = notificationGatewayStatus,
				FromRequested = notificationUserRequested,
				ToDestination = notifyDestination,
				Content = notifyMessage,
				Subject = notificationMessageSubject
			};
		}
	}
}
