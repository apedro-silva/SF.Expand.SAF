using System;
using System.ComponentModel;
namespace SF.Expand.Notification
{
	public interface INotifyChannelProcessor
	{
		[Description("Send notification message")]
		NotifyOperationResult SendNotification(NotificationEvent notificationEvent, out int returnStatus);
	}
}
