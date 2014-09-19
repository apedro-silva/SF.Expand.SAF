
using System;
using System.ComponentModel;
using SF.Expand.SAF.CorePublicItf;


namespace SF.Expand.SAF.DeployToken
{
    /// <summary>
    /// </summary>
    public interface IDeployToken
    {
        /// <summary>
        /// </summary>
        /// <param name="blobData"></param>
        /// <param name="appContentType"></param>
        /// <param name="Base64TokenApplication"></param>
        /// <returns></returns>
        [DescriptionAttribute("Create/Associate one token application with blob data")]
        OperationResult AssembleTokenApplication(Byte[] blobData, out string appContentType, out string Base64TokenApplication);
    }
}
