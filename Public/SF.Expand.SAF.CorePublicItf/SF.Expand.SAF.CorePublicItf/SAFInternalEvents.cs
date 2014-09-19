using SF.Expand.SAF.Configuration;
using System;
namespace SF.Expand.SAF.CorePublicItf
{
	public static class SAFInternalEvents
	{
		public static void Export(APPEVENTSDeff appeventDeff, int appEventID, string appMODName, string[] appMessages)
		{
			SAFInternalEvents.Export(SAFConfiguration.readParameterExternal(appeventDeff.ToString()), appeventDeff, appEventID, appMODName, appMessages);
		}
		public static void Export(string eventHandlerTypeName, APPEVENTSDeff appeventDeff, int appEventID, string appMODName, string[] appMessages)
		{
			object retVAL = null;
			try
			{
				ISAFEvents safEvents = SAFEventsFactory.LoadAssembly((eventHandlerTypeName == null || eventHandlerTypeName.Length < 1) ? SAFConfiguration.readParameterExternal("SAFAPPEventHandler") : eventHandlerTypeName);
				if (safEvents != null)
				{
					safEvents.Export(appeventDeff, appEventID, appMODName, appMessages, out retVAL);
				}
			}
			finally
			{
			}
		}
	}
}
