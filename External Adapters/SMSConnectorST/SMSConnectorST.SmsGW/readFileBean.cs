using SMSConnectorST.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
namespace SMSConnectorST.SmsGW
{
	[GeneratedCode("System.Web.Services", "4.0.30319.1"), DesignerCategory("code"), DebuggerStepThrough, WebServiceBinding(Name = "readFileBeanPort", Namespace = "http://example.org")]
	public class readFileBean : SoapHttpClientProtocol
	{
		private SendOrPostCallback sendMobSMSOperationCompleted;
		private bool useDefaultCredentialsSetExplicitly;
		public event sendMobSMSCompletedEventHandler sendMobSMSCompleted;
		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly && !this.IsLocalFileSystemWebService(value))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}
		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}
		public readFileBean()
		{
			this.Url = Settings.Default.SMSConnectorST_SmsGW_readFileBean;
			if (this.IsLocalFileSystemWebService(this.Url))
			{
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			}
			else
			{
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}
		[SoapRpcMethod("", RequestNamespace = "http://example.org", ResponseNamespace = "http://example.org")]
		public void sendMobSMS(string @string, string string0)
		{
			base.Invoke("sendMobSMS", new object[]
			{
				@string,
				string0
			});
		}
		public void sendMobSMSAsync(string @string, string string0)
		{
			this.sendMobSMSAsync(@string, string0, null);
		}
		public void sendMobSMSAsync(string @string, string string0, object userState)
		{
			if (this.sendMobSMSOperationCompleted == null)
			{
				this.sendMobSMSOperationCompleted = new SendOrPostCallback(this.OnsendMobSMSOperationCompleted);
			}
			base.InvokeAsync("sendMobSMS", new object[]
			{
				@string,
				string0
			}, this.sendMobSMSOperationCompleted, userState);
		}
		private void OnsendMobSMSOperationCompleted(object arg)
		{
			if (this.sendMobSMSCompleted != null)
			{
				InvokeCompletedEventArgs invokeArgs = (InvokeCompletedEventArgs)arg;
				this.sendMobSMSCompleted(this, new AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
			}
		}
		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}
		private bool IsLocalFileSystemWebService(string url)
		{
			bool result;
			if (url == null || url == string.Empty)
			{
				result = false;
			}
			else
			{
				Uri wsUri = new Uri(url);
				result = (wsUri.Port >= 1024 && string.Compare(wsUri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0);
			}
			return result;
		}
	}
}
