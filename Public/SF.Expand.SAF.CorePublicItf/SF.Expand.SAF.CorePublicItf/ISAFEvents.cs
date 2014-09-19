using System;
using System.ComponentModel;
namespace SF.Expand.SAF.CorePublicItf
{
	public interface ISAFEvents
	{
		[Description("Export SAF application events!")]
		void Export(APPEVENTSDeff appEVENTSDeff, int appEventID, string appBASEMODULE, string[] appMESSAGES, out object returnValue);
	}
}
