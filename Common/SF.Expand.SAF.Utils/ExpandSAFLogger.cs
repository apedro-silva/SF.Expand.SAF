
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;


namespace SF.Expand.SAF.Utils
{
    public sealed class LOGGER
    {
        private static ILog _log = null;
        public enum LogCategory { EXCEPTION, ERROR, WARNING, INFORMATION, DEBUG, FATAL, NONE };

        /// <summary>
        /// </summary>
        private LOGGER()
        {
            //
        }

        /// <summary>
        /// </summary>
        /// <param name="logcategory"></param>
        /// <param name="message"></param>
        /// <param name="logObject"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Write(LogCategory logcategory, string message, object logObject)
        {
            _log = LogManager.GetLogger(Type.GetType("System.Object"));
            log4net.Config.XmlConfigurator.Configure();

            switch (logcategory)
            {
                case LogCategory.DEBUG:
                    LOGGER._log.Debug(message,(Exception)logObject);
                    break;
                case LogCategory.INFORMATION:
                    LOGGER._log.Info(message,(Exception)logObject);
                    break;
                case LogCategory.WARNING:
                    LOGGER._log.Warn(message,(Exception)logObject);
                    break;
                case LogCategory.ERROR:
                    LOGGER._log.Error(message,(Exception)logObject);
                    break;
                case LogCategory.FATAL:
                    LOGGER._log.Fatal(message,(Exception)logObject);
                    break;
            }
        }
    }
}
