using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
namespace SF.Expand.SAF.Utils
{
	public static class SAFUtilities
	{
		private static string getExceptionInfo(string typeNameInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StackTrace stackTrace = new StackTrace();
			int num = 0;
			typeNameInfo = "CustomTraceListener";
			MethodBase method;
			string fullName;
			do
			{
				num++;
				StackFrame frame = stackTrace.GetFrame(num);
				method = frame.GetMethod();
				fullName = method.ReflectedType.FullName;
			}
			while (fullName.StartsWith("System") || fullName.EndsWith(typeNameInfo));
			stringBuilder.Append(DateTime.Now.ToString());
			stringBuilder.Append(": ");
			stringBuilder.Append(fullName);
			stringBuilder.Append(".");
			stringBuilder.Append(method.Name);
			stringBuilder.Append("( ");
			int i = 0;
			ParameterInfo[] parameters = method.GetParameters();
			while (i < parameters.Length)
			{
				stringBuilder.Append(parameters[i].ParameterType.Name);
				stringBuilder.Append(" ");
				stringBuilder.Append(parameters[i].Name);
				i++;
				if (i != parameters.Length)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(" ): ");
			return stringBuilder.ToString();
		}
	}
}
