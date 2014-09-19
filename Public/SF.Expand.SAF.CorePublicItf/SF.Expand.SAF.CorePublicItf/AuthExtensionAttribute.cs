using System;
using System.Web.Services.Protocols;
namespace SF.Expand.SAF.CorePublicItf
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AuthExtensionAttribute : SoapExtensionAttribute
	{
		private int _priority = 1;
		public override int Priority
		{
			get
			{
				return this._priority;
			}
			set
			{
				this._priority = value;
			}
		}
		public override Type ExtensionType
		{
			get
			{
				return typeof(AuthExtension);
			}
		}
	}
}
