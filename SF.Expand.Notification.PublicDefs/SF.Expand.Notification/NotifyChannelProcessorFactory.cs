using SF.Expand.LOG;
using System;
namespace SF.Expand.Notification
{
	public class NotifyChannelProcessorFactory
	{
		private const string cMODULE_NAME = "SFNOTIFICATION";
		private const string cBASE_NAME = "http://sfexpand.notification.NotifyChannelProcessorFactory.softfinanca.com/";
		public static INotifyChannelProcessor LoadSMSAssembly(string typeName)
		{
			INotifyChannelProcessor result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SFNOTIFICATION", new string[]
				{
					"http://sfexpand.notification.NotifyChannelProcessorFactory.softfinanca.com/",
					"[INotifyChannelProcessor]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(INotifyChannelProcessor).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SFNOTIFICATION", new string[]
						{
							"http://sfexpand.notification.NotifyChannelProcessorFactory.softfinanca.com/",
							"[INotifyChannelProcessor]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (INotifyChannelProcessor)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SFNOTIFICATION", new string[]
					{
						"http://sfexpand.notification.NotifyChannelProcessorFactory.softfinanca.com/",
						ex.ToString()
					});
					result = null;
				}
			}
			return result;
		}
	}
}
