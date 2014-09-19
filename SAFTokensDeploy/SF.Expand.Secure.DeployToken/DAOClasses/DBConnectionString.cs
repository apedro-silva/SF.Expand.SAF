
using System;
using SF.Expand.SAF.Utils;
using SF.Expand.SAF.Configuration;


namespace SF.Expand.SAF.DeployToken
{
    public static class DBConnectionString
    {
        private const string NEW_LINE = "\r\n";
        private const string _baseName = @"http://sfexpandsecure.business.deploytokenDAO.softfinanca.com//";

        public static string ExpandSecureDeployTokens()
        {
            try
            {
                return SAFConfiguration.readConnectionStringBusiness();
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, _baseName + NEW_LINE + ex.Message, null);
                return null;
            }
        }
    }
}
