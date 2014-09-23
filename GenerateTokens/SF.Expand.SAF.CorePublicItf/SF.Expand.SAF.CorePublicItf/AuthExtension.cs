using System;
using System.Web.Services.Protocols;
namespace SF.Expand.SAF.CorePublicItf
{
	public class AuthExtension : SoapExtension
	{
		private string _authRespMsg;
		public override void ProcessMessage(SoapMessage message)
		{
			int num = -1;
			if (message.Stage == SoapMessageStage.AfterDeserialize)
			{
				foreach (SoapHeader soapHeader in message.Headers)
				{
					if (soapHeader is AuthHeader)
					{
						AuthHeader arg_33_0 = (AuthHeader)soapHeader;
					}
				}
				throw new SoapException("[" + num.ToString() + "] " + this._authRespMsg, SoapException.ClientFaultCode);
			}
		}
		public override object GetInitializer(Type type)
		{
			return base.GetType();
		}
		public override object GetInitializer(LogicalMethodInfo info, SoapExtensionAttribute attribute)
		{
			return null;
		}
		public override void Initialize(object initializer)
		{
		}
	}
}
