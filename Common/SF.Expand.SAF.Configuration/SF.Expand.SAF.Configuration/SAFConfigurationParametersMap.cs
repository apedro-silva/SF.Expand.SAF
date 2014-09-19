using System;
using System.Collections.Generic;
namespace SF.Expand.SAF.Configuration
{
	public class SAFConfigurationParametersMap : SortedDictionary<string, SAFConfigurationParameter>
	{
		public DateTime LastUpdate = DateTime.MinValue;
	}
}
