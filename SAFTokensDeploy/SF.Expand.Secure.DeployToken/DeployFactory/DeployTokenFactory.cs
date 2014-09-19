

using System;
using SF.Expand.SAF.CorePublicItf;


namespace SF.Expand.SAF.DeployToken
{
    public class DeployTokenFactory
    {
        private const string _Nl = @"\r\n";
        private const string _baseName = @"http://sfexpandsecure.business.tokensDeployfactory.softfinanca.com::";

        /// <summary>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static IDeployToken LoadAssembly(string typeName)
        {
            if (typeName == null || typeName.Length == 0)
            {
                return null;
            }

            try
            {
                Type _type = Type.GetType(typeName, true);
                if (!typeof(IDeployToken).IsAssignableFrom(_type))
                {
                    return null;
                }
                return (IDeployToken)Activator.CreateInstance(_type, true);
            }
            catch
            {
                return null;
            }
        }
    }
}
