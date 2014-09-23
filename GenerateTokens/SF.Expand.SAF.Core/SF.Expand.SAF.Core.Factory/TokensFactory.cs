using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Utils;
using System;
namespace SF.Expand.SAF.Core.Factory
{
	public class TokensFactory
	{
		public static ITokens LoadAssembly(string typeName)
		{
			if (typeName == null || typeName.Length == 0)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "{ITokens}SF.Expand.SAF.Core.Factory::LoadAssembly, typename cannot be null or empty!", null);
				return null;
			}
			ITokens result;
			try
			{
				Type type = Type.GetType(typeName, true);
				if (!typeof(ITokens).IsAssignableFrom(type))
				{
					LOGGER.Write(LOGGER.LogCategory.ERROR, "{ITokens!IsAssignableFrom}SF.Expand.SAF.Core.Factory::LoadAssembly[" + typeName + "]", null);
					result = null;
				}
				else
				{
					result = (ITokens)Activator.CreateInstance(type, true);
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"{ITokens}SF.Expand.SAF.Core.Factory::LoadAssembly[",
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
