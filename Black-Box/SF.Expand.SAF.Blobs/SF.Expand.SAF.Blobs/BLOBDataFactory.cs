using SF.Expand.LOG;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Blobs
{
	public class BLOBDataFactory
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.BLOBDataFactory.softfinanca.com/";
		public static IBLOBData LoadAssembly(string typeName)
		{
			IBLOBData result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.BLOBDataFactory.softfinanca.com/",
					"[IBLOBData]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(IBLOBData).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
						{
							"http://sfexpand.SAFCore.BLOBDataFactory.softfinanca.com/",
							"[IBLOBData]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (IBLOBData)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.BLOBDataFactory.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						ex.ToString()
					});
					result = null;
				}
			}
			return result;
		}
	}
}
