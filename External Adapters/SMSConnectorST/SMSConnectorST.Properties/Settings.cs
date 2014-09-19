using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace SMSConnectorST.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}
		[ApplicationScopedSetting, DefaultSettingValue("http://ssinternasp01:10001/mBPM-server/mobSMSService/readFile"), SpecialSetting(SpecialSetting.WebServiceUrl), DebuggerNonUserCode]
		public string SMSConnectorST_SmsGW_readFileBean
		{
			get
			{
				return (string)this["SMSConnectorST_SmsGW_readFileBean"];
			}
		}
	}
}
