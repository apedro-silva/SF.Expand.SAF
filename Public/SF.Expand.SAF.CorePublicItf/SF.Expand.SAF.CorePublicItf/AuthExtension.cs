using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CryptoEngine;
using System;
using System.Text;
using System.Web.Services.Protocols;
namespace SF.Expand.SAF.CorePublicItf
{
	public class AuthExtension : SoapExtension
	{
		private const int cBUFFER = 8192;
		private const string cFORMAT_USER = "USERID '{0}'  ";
		private const string cMODULE_NAME = "SAFAPILOGGER";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.AuthExtension.softfinanca.com/";
		private string _processSoap(SoapMessage message)
		{
			StringBuilder sb = new StringBuilder(8192);
			string result;
			try
			{
				sb.Append(message.MethodInfo.Name);
				for (int i = 0; i < message.MethodInfo.Parameters.Length; i++)
				{
					sb.Append("[" + message.MethodInfo.Parameters[i].Name + "=");
					if (message.MethodInfo.Parameters[i].ParameterType.FullName.StartsWith("System.") && !message.MethodInfo.Parameters[i].IsOut)
					{
						sb.Append((message.GetInParameterValue(i) != null) ? message.GetInParameterValue(i) : "null");
					}
					else
					{
						sb.Append(message.MethodInfo.Parameters[i].ParameterType.FullName);
					}
					sb.Append("]");
				}
				result = sb.ToString();
			}
			catch
			{
				result = null;
			}
			finally
			{
			}
			return result;
		}
		public override void ProcessMessage(SoapMessage message)
		{
			try
			{
				if (message.Stage == SoapMessageStage.AfterDeserialize)
				{
					string _mtd = this._processSoap(message);
					string _userExecute = string.Format("USERID '{0}'  ", "unknown");
					foreach (SoapHeader header in message.Headers)
					{
						if (header is AuthHeader)
						{
							AuthHeader credentials = (AuthHeader)header;
							try
							{
								switch (credentials.CryptoAlgorithm)
								{
								case AuthHeader.CryptoAlgorithmEnum.NONE:
									_userExecute = string.Format("USERID '{0}'  ", credentials.AuthKey.Split(new char[]
									{
										'|'
									})[0]);
									break;
								case AuthHeader.CryptoAlgorithmEnum.TRIPLEDES:
									_userExecute = string.Format("USERID '{0}'  ", CryptorEngineTripleDES.Decrypt(SAFConfiguration.readConnectionStringCoreEncrypted(), new SecurityInfo(SAFConfiguration.readMasterKey(), SAFConfiguration.readInfoKey(), SAFConfiguration.readInfoIV()), true).Split(new char[]
									{
										'|'
									})[0]);
									break;
								}
							}
							catch (SoapException ex)
							{
								SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFAPILOGGER", new string[]
								{
									"http://sfexpand.SAFBusiness.AuthExtension.softfinanca.com/",
									ex.ToString()
								});
							}
						}
					}
					SAFLOGGER.dump(SAFLOGGER.LOGGEREventID.INFORMATION, "SAFAPILOGGER", new string[]
					{
						_userExecute + _mtd
					});
				}
			}
			catch
			{
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
