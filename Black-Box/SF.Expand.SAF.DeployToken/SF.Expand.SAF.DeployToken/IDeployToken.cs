using SF.Expand.SAF.CorePublicItf;
using System;
using System.ComponentModel;
namespace SF.Expand.SAF.DeployToken
{
	public interface IDeployToken
	{
		[Description("Create/Associate one token application with blob data")]
		OperationResult AssembleTokenApplication(byte[] blobData, out string appContentType, out string Base64TokenApplication);
	}
}
