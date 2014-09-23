using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Runtime.CompilerServices;
namespace SF.Expand.SAF.Utils
{
	public sealed class LOGGER
	{
		public enum LogCategory
		{
			EXCEPTION,
			ERROR,
			WARNING,
			INFORMATION,
			DEBUG,
			FATAL,
			NONE
		}
		public enum LogContextid
		{
			TOKEN_ID,
			APPLICATION_USER
		}
		public struct LogContextProperties
		{
			private LOGGER.LogContextid _logContextidid;
			private string _logContextValue;
			public LOGGER.LogContextid LOGContextID
			{
				get
				{
					return this._logContextidid;
				}
			}
			public string LOGContextValue
			{
				get
				{
					return this._logContextValue;
				}
			}
			public LogContextProperties(LOGGER.LogContextid logContextidid, string logContextValue)
			{
				this._logContextidid = logContextidid;
				this._logContextValue = logContextValue;
			}
		}
		private static ILog _log;
		private LOGGER()
		{
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LOG(LOGGER.LogCategory logCategory, string message, Exception ex)
		{
			LOGGER._log = LogManager.GetLogger(Type.GetType("System.Object"));
			XmlConfigurator.Configure();
			switch (logCategory)
			{
			case LOGGER.LogCategory.ERROR:
				LOGGER._log.Error(message, ex);
				return;
			case LOGGER.LogCategory.WARNING:
				LOGGER._log.Warn(message, ex);
				return;
			case LOGGER.LogCategory.INFORMATION:
				LOGGER._log.Info(message, ex);
				return;
			case LOGGER.LogCategory.DEBUG:
				LOGGER._log.Debug(message, ex);
				return;
			case LOGGER.LogCategory.FATAL:
				LOGGER._log.Fatal(message, ex);
				return;
			default:
				return;
			}
		}
		public static void Write(LOGGER.LogCategory logcategory, string message, object logObject)
		{
			LOGGER.LOG(logcategory, message, (Exception)logObject);
		}
	}
}
