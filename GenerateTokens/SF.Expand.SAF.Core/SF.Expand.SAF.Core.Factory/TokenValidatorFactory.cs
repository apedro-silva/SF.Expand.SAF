using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Utils;
using System;
namespace SF.Expand.SAF.Core.Factory
{
	public class TokenValidatorFactory
	{
		public static ITokenValidator LoadAssembly(string typeName)
		{
			ITokenValidator result;
			try
			{
				Type type = Type.GetType(typeName, true);
				if (!typeof(ITokenValidator).IsAssignableFrom(type))
				{
					LOGGER.Write(LOGGER.LogCategory.ERROR, "{ITokenValidator}SF.Expand.SAF.Core.Factory::LoadAssembly[" + typeName + "]", null);
					result = null;
				}
				else
				{
					result = (ITokenValidator)Activator.CreateInstance(type, true);
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"{ITokenValidator}SF.Expand.SAF.Core.Factory::LoadAssembly[",
					typeName,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = null;
			}
			return result;
		}
	}
}
