using System;
using System.ComponentModel;
namespace SF.Expand.Notification
{
	public interface INotifyChannelAsync
	{
		[Description("Afther notification sended")]
		event EventHandler AfterNotification;
		[Description("Send notification message")]
		void SendNotification(NotificationEvent notificationEvent, out int returnStatus);
	}
}
