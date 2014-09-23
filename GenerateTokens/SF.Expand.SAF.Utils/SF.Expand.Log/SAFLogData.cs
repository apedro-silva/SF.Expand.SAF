using System;
namespace SF.Expand.Log
{
	public class SAFLogData
	{
		public int Id = -1;
		public DateTime TimeStampUTC = DateTime.MinValue;
		public string ThreadName;
		public string LevelName;
		public string LoggerName;
		public string UserName;
		public string MessageString;
		public string ExceptionString;
	}
}
