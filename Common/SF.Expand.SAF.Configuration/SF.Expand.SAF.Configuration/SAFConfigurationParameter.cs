using System;
namespace SF.Expand.SAF.Configuration
{
	[Serializable]
	public class SAFConfigurationParameter
	{
		public string section;
		public string name;
		public string value;
		public bool frozen = true;
		public bool hidden = true;
		public DateTime lastUTCupdate = DateTime.MinValue;
	}
}
