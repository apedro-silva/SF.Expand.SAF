using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core.Factory
{
	public class TokensFactory
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokensFactory.softfinanca.com/";
		public static ITokens LoadAssembly(string typeName)
		{
			ITokens result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensFactory.softfinanca.com/",
					"[ITokens]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(ITokens).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
						{
							"http://sfexpand.SAFCore.TokensFactory.softfinanca.com/",
							"[ITokens]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (ITokens)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.TokensFactory.softfinanca.com/",
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
