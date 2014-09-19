using System;
namespace SF.Expand.SAF.CorePublicItf
{
	public class SAFEventsFactory
	{
		private const string cMODULE_NAME = "SAFAPPEVENTS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.SAFEventsFactory.softfinanca.com/";
		public static ISAFEvents LoadAssembly(string typeName)
		{
			ISAFEvents result;
			if (typeName == null || typeName.Length == 0)
			{
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(ISAFEvents).IsAssignableFrom(_type))
					{
						result = null;
					}
					else
					{
						result = (ISAFEvents)Activator.CreateInstance(_type, true);
					}
				}
				catch
				{
					result = null;
				}
			}
			return result;
		}
	}
}
