using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
namespace SF.Expand.LOG
{
	public static class SAFLOGGER
	{
		[Serializable]
		public enum LOGGEREventID
		{
			EXCEPTION = 1,
			ERROR,
			WARNING,
			INFORMATION
		}
		private const string cBASEAPPNAME = "SAFLOGGER";
		private const string cBASE_NAME = "http://sfexpandsecure.logger.softfinanca.com/";
		private const string cSAFTRACE_LEVEL = "SAFTRACE_LEVEL";
		private const string cSAFTRACE_FILEPATH = "SAFTRACE_FILEPATH";
		private const string cSAFTRACE_BASEFILENAME = "SAFTRACE_BASEFILENAME";
		private const string cSAFTRACE_FILELOCKDELAY = "SAFTRACE_FILELOCKDELAY";
		private const string cSAFTRACE_FILENAMEFORMATER = "SAFTRACE_FILENAMEFORMATER";
		private const string cBASE_FILEFORMATER = "yyyyMMddHH";
		private const int cBASE_FILELOCKDELAY = 1000;
		private static TextWriter s_Writer = null;
		private static object s_Lock = new object();
		private static string[] _getMachineInfo(int _option)
		{
			Process _thisProc = null;
			string[] result;
			try
			{
				switch (_option)
				{
				case 1:
				{
					string _procName = Registry.LocalMachine.OpenSubKey("hardware\\description\\system\\centralprocessor\\0").GetValue("ProcessorNameString").ToString();
					result = new string[]
					{
						"# " + Assembly.GetExecutingAssembly().FullName.ToString(),
						string.Format("# machine/ name:{0}; CPU(s):[count:{1} type:{2}]; OS:{3}; CLR:{4}", new object[]
						{
							Environment.MachineName,
							Environment.ProcessorCount,
							_procName,
							Environment.OSVersion.VersionString,
							Environment.Version.ToString()
						})
					};
					break;
				}
				case 2:
					_thisProc = Process.GetCurrentProcess();
					result = new string[]
					{
						string.Format("# process/ startTime:{0}; totalProcessorTime:{1} Priority/ ({2});{3}", new object[]
						{
							_thisProc.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
							_thisProc.TotalProcessorTime,
							_thisProc.BasePriority,
							_thisProc.PriorityClass
						}),
						string.Format("#          virtual memory allocated:{0}MBytes; physical allocated:{1}MBytes; used memory:{2}MBytes", (float)_thisProc.VirtualMemorySize64 / 1024f / 1024f, (float)_thisProc.WorkingSet64 / 1024f / 1024f, (float)_thisProc.PrivateMemorySize64 / 1024f / 1024f)
					};
					break;
				default:
					result = null;
					break;
				}
			}
			catch
			{
				result = null;
			}
			finally
			{
				if (_thisProc != null)
				{
					_thisProc.Dispose();
				}
				_thisProc = null;
			}
			return result;
		}
		private static string _formatLogMSG(SAFLOGGER.LOGGEREventID loggerEventID, string appBASEid, string[] appMSGtext)
		{
			Process _thisProc = null;
			string result;
			try
			{
				_thisProc = Process.GetCurrentProcess();
				result = string.Format("{0} [{1}] [{2}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), loggerEventID.ToString(), _thisProc.ProcessName) + " " + string.Join(" ", appMSGtext) + ((loggerEventID == SAFLOGGER.LOGGEREventID.EXCEPTION) ? string.Concat(new string[]
				{
					Environment.NewLine,
					SAFLOGGER._getMachineInfo(2)[0],
					Environment.NewLine,
					SAFLOGGER._getMachineInfo(2)[1],
					Environment.NewLine
				}) : "");
			}
			catch
			{
				result = "... [ unable to format message ] ...";
			}
			finally
			{
				if (_thisProc != null)
				{
					_thisProc.Dispose();
				}
				_thisProc = null;
			}
			return result;
		}
		private static void writeToEventLog(EventLogEntryType evtLogType, string evtLogText)
		{
			try
			{
				if (!EventLog.SourceExists("SAFLOGGER", "."))
				{
					EventLog.CreateEventSource(new EventSourceCreationData("SAFLOGGER", "SAFLOGGER"));
				}
				new EventLog("SAFLOGGER", ".", "SAFLOGGER").WriteEntry(evtLogText, evtLogType);
			}
			catch (Exception ex)
			{
				Debug.Write(SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.EXCEPTION, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
				{
					"unable to write on EventLOG",
					ex.ToString()
				}));
				Debug.Write(evtLogText);
			}
		}
		private static void writeToFile(string fileBaseName, string _logMessage)
		{
			int _filelockDelay = 0;
			string _baseFilePath = null;
			try
			{
				string _fileFormater;
				if (null == (_fileFormater = ConfigurationManager.AppSettings.Get("SAFTRACE_FILENAMEFORMATER")))
				{
					_fileFormater = "yyyyMMddHH";
				}
				string _filePathName;
				if (null == (_filePathName = ConfigurationManager.AppSettings.Get("SAFTRACE_FILEPATH")))
				{
					_filePathName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				}
				_baseFilePath = Path.Combine(_filePathName, string.Format(fileBaseName.Trim() + "{0}.LOG", DateTime.Now.ToString(_fileFormater)));
				int.TryParse("0" + ConfigurationManager.AppSettings.Get("SAFTRACE_FILELOCKDELAY"), out _filelockDelay);
			}
			catch (Exception ex)
			{
				Debug.Write(SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.WARNING, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
				{
					"unable to write message!",
					ex.ToString()
				}));
				Debug.Write(_logMessage);
				return;
			}
			if (Monitor.TryEnter(SAFLOGGER.s_Lock, (_filelockDelay < 1000) ? 1000 : _filelockDelay))
			{
				try
				{
					object obj;
					Monitor.Enter(obj = SAFLOGGER.s_Lock);
					try
					{
						if (SAFLOGGER.s_Writer == null)
						{
							if (!File.Exists(_baseFilePath))
							{
								SAFLOGGER.s_Writer = File.CreateText(_baseFilePath);
								string[] _fileHeaders = SAFLOGGER._getMachineInfo(1);
								SAFLOGGER.s_Writer.WriteLine(_fileHeaders[0] + Environment.NewLine + _fileHeaders[1] + Environment.NewLine);
							}
							else
							{
								SAFLOGGER.s_Writer = File.AppendText(_baseFilePath);
							}
						}
						SAFLOGGER.s_Writer.WriteLine(_logMessage);
						SAFLOGGER.s_Writer.Flush();
						SAFLOGGER.s_Writer.Close();
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				catch (Exception ex)
				{
					Debug.Write(SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.WARNING, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
					{
						"unable to write message!",
						ex.ToString()
					}));
					Debug.Write(_logMessage);
				}
				finally
				{
					SAFLOGGER.s_Writer = null;
					Monitor.Exit(SAFLOGGER.s_Lock);
				}
			}
			else
			{
				SAFLOGGER.writeToEventLog(EventLogEntryType.Error, SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.EXCEPTION, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
				{
					"Timeout; could not obtain lock to log file!"
				}) + Environment.NewLine + _logMessage);
				Debug.Write(SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.WARNING, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
				{
					"Timeout; could not obtain lock to log file!"
				}));
			}
		}
		public static void Write(SAFLOGGER.LOGGEREventID loggerEventID, string appBASEid, string[] appMSGtext)
		{
			try
			{
				int _SAFDEPLOYTRACELevel;
				if (-9 == (_SAFDEPLOYTRACELevel = int.Parse("0" + ConfigurationManager.AppSettings.Get("SAFTRACE_LEVEL"))))
				{
					Debug.Write(SAFLOGGER._formatLogMSG(loggerEventID, appBASEid, appMSGtext));
					if (loggerEventID != (SAFLOGGER.LOGGEREventID)(-1))
					{
						return;
					}
				}
				if (loggerEventID <= (SAFLOGGER.LOGGEREventID)_SAFDEPLOYTRACELevel)
				{
					SAFLOGGER.writeToFile(appBASEid, SAFLOGGER._formatLogMSG(loggerEventID, appBASEid, appMSGtext));
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.writeToEventLog(EventLogEntryType.Error, SAFLOGGER._formatLogMSG(SAFLOGGER.LOGGEREventID.EXCEPTION, "http://sfexpandsecure.logger.softfinanca.com/", new string[]
				{
					ex.ToString(),
					"unable to dump message log!!"
				}) + Environment.NewLine + SAFLOGGER._formatLogMSG(loggerEventID, appBASEid, appMSGtext));
			}
		}
		public static void dump(SAFLOGGER.LOGGEREventID loggerEventID, string appBASEid, string[] appMSGtext)
		{
			SAFLOGGER.writeToFile(appBASEid, SAFLOGGER._formatLogMSG(loggerEventID, appBASEid, appMSGtext));
		}
	}
}
