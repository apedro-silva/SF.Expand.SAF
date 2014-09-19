using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public class NotificationChannel
	{
		public int ChannelID;
		public string ChannelDescription;
		public string ChannelProcessor;
		public long ResquestExpiration;
		public bool ChannelStatus;
		public int ChannelTimout;
		public string ChannelProtocol;
		public string ChannelBaseParameters;
		public static NotificationChannel loadNotificationChannel(int notificationChannelID, string notificationChannelDesc, string notificationChannelProcessor, int notificationChannelTimout, string notificationChannelProtocol, long notificationResquestExpiration, bool notificationChannelStatus, string notificationChannelBaseParameters)
		{
			return new NotificationChannel
			{
				ChannelDescription = notificationChannelDesc,
				ChannelID = notificationChannelID,
				ChannelProcessor = notificationChannelProcessor,
				ResquestExpiration = notificationResquestExpiration,
				ChannelStatus = notificationChannelStatus,
				ChannelProtocol = notificationChannelProtocol,
				ChannelBaseParameters = notificationChannelBaseParameters,
				ChannelTimout = notificationChannelTimout
			};
		}
	}
}
