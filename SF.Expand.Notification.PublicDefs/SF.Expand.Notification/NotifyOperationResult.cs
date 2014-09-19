using System;
namespace SF.Expand.Notification
{
	[Serializable]
	public enum NotifyOperationResult
	{
		Success,
		Error = 201,
		DestinationsToNotifyNotFound,
		ApplicationEventsToNotifyNotFound,
		ChannelProcessorNotFound,
		ChannelProcessorError,
		ChannelProcessorTimeOut = 205
	}
}
