using SF.Expand.SAF.CorePublicItf;
using System;
using System.Configuration;
namespace SAFExportEvents
{
	public class SAFExportEvents : ISAFEvents
	{
		private const string cMODULE_NAME = "SAFEXPORTEVENTS";
		private const string cBASE_NAME = "http://sfexpand.SAFExportEvents.MSMQHELPER.softfinanca.com/";
		private const string cEXPORT_MESSAGEQUE_PARAMETER = "SAFEXPORTEVENTS_MSMQPATH";
		public void Export(APPEVENTSDeff appEVENTSDeff, int appEventID, string appBASEMODULE, string[] appMESSAGES, out object returnValue)
		{
			returnValue = null;
			string __mqParams = ConfigurationManager.AppSettings.Get("SAFEXPORTEVENTS_MSMQPATH");
			if (__mqParams != null && (__mqParams ?? "").Length >= 1)
			{
				string[] _gatewayParams = __mqParams.Split(new char[]
				{
					'|'
				});
				if (_gatewayParams.Length >= 2)
				{
					string[] arrExport = (appMESSAGES != null) ? new string[appMESSAGES.Length + 2] : new string[0];
					try
					{
						if (APPEVENTSDeff.GENERIC_EVENTS == appEVENTSDeff)
						{
							appMESSAGES.CopyTo(arrExport, 0);
							arrExport.SetValue(appBASEMODULE, appMESSAGES.Length);
							arrExport.SetValue(appEventID.ToString(), appMESSAGES.Length + 1);
							MSMQHELPER.sendMQMessage(_gatewayParams[0], _gatewayParams[1], arrExport);
						}
					}
					finally
					{
					}
				}
			}
		}
	}
}
