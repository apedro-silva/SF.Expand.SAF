using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core.Factory
{
	public class TokenValidatorFactory
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokenValidatorFactory.softfinanca.com/";
		public static ITokenValidator LoadAssembly(string typeName)
		{
			ITokenValidator result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokenValidatorFactory.softfinanca.com/",
					"[ITokenValidator]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(ITokenValidator).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
						{
							"http://sfexpand.SAFCore.TokenValidatorFactory.softfinanca.com/",
							"[ITokenValidator]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (ITokenValidator)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.TokenValidatorFactory.softfinanca.com/",
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
