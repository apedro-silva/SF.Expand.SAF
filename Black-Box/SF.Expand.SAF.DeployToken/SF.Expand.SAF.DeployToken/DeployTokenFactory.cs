using SF.Expand.LOG;
using System;
using System.Reflection;
namespace SF.Expand.SAF.DeployToken
{
	public class DeployTokenFactory
	{
		private const string cMODULE_NAME = "SAFBUSINESSDEPLOY";
		private const string cBASE_NAME = "http://sfexpand.SAFDeploy.DeployTokenFactory.softfinanca.com/";
		public static IDeployToken LoadAssembly(string typeName)
		{
			IDeployToken result;
			if (typeName == null || typeName.Length == 0)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESSDEPLOY", new string[]
				{
					"http://sfexpand.SAFDeploy.DeployTokenFactory.softfinanca.com/",
					"[IDeployToken]::" + typeName.Trim(),
					"Invalid or null typename!"
				});
				result = null;
			}
			else
			{
				try
				{
					Type _type = Type.GetType(typeName, true);
					if (!typeof(IDeployToken).IsAssignableFrom(_type))
					{
						SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESSDEPLOY", new string[]
						{
							"http://sfexpand.SAFDeploy.DeployTokenFactory.softfinanca.com/",
							"[IDeployToken]::" + typeName.Trim(),
							"typename not Assignable!"
						});
						result = null;
					}
					else
					{
						result = (IDeployToken)Activator.CreateInstance(_type, true);
					}
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESSDEPLOY", new string[]
					{
						"http://sfexpand.SAFDeploy.DeployTokenFactory.softfinanca.com/",
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
