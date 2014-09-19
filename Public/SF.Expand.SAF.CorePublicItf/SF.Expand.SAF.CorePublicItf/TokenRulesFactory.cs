using SF.Expand.LOG;
using System;
using System.Reflection;
namespace SF.Expand.SAF.CorePublicItf
{
	public class TokenRulesFactory
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.TokenRulesFactory.softfinanca.com/";
		public static ITokenRules LoadAssembly(string typeName)
		{
			ITokenRules result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenRulesFactory.softfinanca.com/",
					"[ITokenRules]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(ITokenRules).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.TokenRulesFactory.softfinanca.com/",
							"[ITokenRules]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (ITokenRules)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.TokenRulesFactory.softfinanca.com/",
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
