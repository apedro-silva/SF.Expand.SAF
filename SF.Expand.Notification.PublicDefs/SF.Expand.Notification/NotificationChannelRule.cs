using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public class NotificationChannelRule
	{
		public int NotificationChannelRuleID;
		public int NotificationChannel;
		public int ApplicationEventOperation;
		public string NotificationSourceApplication;
		public string NotificationMessageTemplate;
		public bool NotificationRuleStatus;
		public static NotificationChannelRule loadNotificationChannelRule(int notificationChannelRuleID, int notificationChannel, int applicationEventOperation, string notificationSourceApplication, string notificationMessageTemplate, bool notificationRuleStatus)
		{
			return new NotificationChannelRule
			{
				NotificationChannelRuleID = notificationChannelRuleID,
				NotificationChannel = notificationChannel,
				NotificationSourceApplication = notificationSourceApplication,
				ApplicationEventOperation = applicationEventOperation,
				NotificationMessageTemplate = notificationMessageTemplate,
				NotificationRuleStatus = notificationRuleStatus
			};
		}
	}
}
