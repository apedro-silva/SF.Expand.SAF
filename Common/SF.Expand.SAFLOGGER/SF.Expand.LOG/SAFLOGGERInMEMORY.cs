using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
namespace SF.Expand.LOG
{
	public class SAFLOGGERInMEMORY
	{
		public delegate void LogUpdateDelegate();
		private int m_nMaxChars = -1;
		private bool m_bTimestamp = false;
		private bool m_bLineTerminate = true;
		private string m_strName = string.Empty;
		private string m_strLog = string.Empty;
		private bool m_bReverseOrder = false;
		private static Hashtable m_LogsTable = new Hashtable();
		private static StringBuilder m_strBuilder = null;
		public event SAFLOGGERInMEMORY.LogUpdateDelegate OnLogUpdate;
		private string FileName
		{
			get
			{
				return this.m_strName;
			}
		}
		public string Name
		{
			get
			{
				return this.m_strName;
			}
		}
		public bool Timestamp
		{
			get
			{
				return this.m_bTimestamp;
			}
			set
			{
				this.m_bTimestamp = value;
			}
		}
		public bool LineTerminate
		{
			get
			{
				return this.m_bTimestamp;
			}
			set
			{
				this.m_bTimestamp = value;
			}
		}
		public bool ReverseOrder
		{
			get
			{
				return this.m_bReverseOrder;
			}
			set
			{
				this.m_bReverseOrder = value;
			}
		}
		public int MaxChars
		{
			get
			{
				return this.m_nMaxChars;
			}
			set
			{
				this.m_nMaxChars = value;
			}
		}
		public string Log
		{
			get
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				string strLog2;
				try
				{
					strLog2 = this.m_strLog;
				}
				finally
				{
					Monitor.Exit(strLog);
				}
				return strLog2;
			}
		}
		private SAFLOGGERInMEMORY(string name, bool readExistingFile)
		{
			this.m_strName = name;
			if (readExistingFile)
			{
				this.ReadLog();
			}
		}
		private void ReadLog()
		{
			if (File.Exists(this.FileName))
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					using (StreamReader sr = File.OpenText(this.FileName))
					{
						this.m_strLog = sr.ReadToEnd();
						sr.Close();
					}
				}
				finally
				{
					Monitor.Exit(strLog);
				}
			}
		}
		private void WriteLog()
		{
			if (File.Exists(this.FileName))
			{
				File.Delete(this.FileName);
			}
			if (this.m_strLog.Length != 0)
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					using (StreamWriter sw = File.CreateText(this.FileName))
					{
						sw.Write(this.m_strLog);
						sw.Close();
					}
				}
				finally
				{
					Monitor.Exit(strLog);
				}
			}
		}
		public static SAFLOGGERInMEMORY GetLogString(string name, bool readExistingFile)
		{
			return SAFLOGGERInMEMORY.GetLogString(name, readExistingFile, -1);
		}
		public static SAFLOGGERInMEMORY GetLogString(string name, bool readExistingFile, int baseCapacity)
		{
			SAFLOGGERInMEMORY result;
			if (SAFLOGGERInMEMORY.m_LogsTable.ContainsKey(name))
			{
				result = (SAFLOGGERInMEMORY)SAFLOGGERInMEMORY.m_LogsTable[name];
			}
			else
			{
				SAFLOGGERInMEMORY rv = new SAFLOGGERInMEMORY(name, readExistingFile);
				if (baseCapacity > 0)
				{
					SAFLOGGERInMEMORY.m_strBuilder = new StringBuilder((baseCapacity > 1024) ? baseCapacity : 1024);
				}
				SAFLOGGERInMEMORY.m_LogsTable.Add(name, rv);
				result = rv;
			}
			return result;
		}
		public static void RemoveLogString(string name)
		{
			if (SAFLOGGERInMEMORY.m_LogsTable.ContainsKey(name))
			{
				SAFLOGGERInMEMORY i = (SAFLOGGERInMEMORY)SAFLOGGERInMEMORY.m_LogsTable[name];
				i.Clear();
				SAFLOGGERInMEMORY.m_LogsTable.Remove(name);
			}
		}
		public static void PersistAll()
		{
			ICollection loglist = SAFLOGGERInMEMORY.m_LogsTable.Values;
			foreach (SAFLOGGERInMEMORY ls in loglist)
			{
				ls.Persist();
			}
		}
		public static void ClearAll()
		{
			ICollection loglist = SAFLOGGERInMEMORY.m_LogsTable.Values;
			foreach (SAFLOGGERInMEMORY ls in loglist)
			{
				ls.Clear();
			}
		}
		public void AddTrace(string str)
		{
			bool retry = false;
			string toadd = "";
			if (this.m_bTimestamp)
			{
				toadd = toadd + DateTime.Now.ToUniversalTime().ToString() + ": ";
			}
			toadd += str;
			if (this.m_bLineTerminate)
			{
				toadd += "\r\n";
			}
			try
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					if (this.m_bReverseOrder)
					{
						this.m_strLog = toadd + this.m_strLog;
					}
					else
					{
						this.m_strLog += toadd;
					}
					if (this.m_nMaxChars > 1 && this.m_strLog.Length > this.m_nMaxChars)
					{
						if (this.m_bReverseOrder)
						{
							this.m_strLog = this.m_strLog.Substring(0, this.m_nMaxChars);
						}
						else
						{
							this.m_strLog = this.m_strLog.Substring(this.m_strLog.Length - this.m_nMaxChars);
						}
					}
				}
				finally
				{
					Monitor.Exit(strLog);
				}
				if (this.OnLogUpdate != null)
				{
					this.OnLogUpdate();
				}
			}
			catch (Exception)
			{
			}
			while (retry)
			{
			}
		}
		public void Add(string str)
		{
			bool retry = false;
			string toadd = "";
			if (this.m_bTimestamp)
			{
				toadd = toadd + DateTime.Now.ToUniversalTime().ToString() + ": ";
			}
			toadd += str;
			if (this.m_bLineTerminate)
			{
				toadd += "\r\n";
			}
			try
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					if (this.m_bReverseOrder)
					{
						this.m_strLog = toadd + this.m_strLog;
					}
					else
					{
						this.m_strLog += toadd;
					}
					if (this.m_nMaxChars > 1 && this.m_strLog.Length > this.m_nMaxChars)
					{
						if (this.m_bReverseOrder)
						{
							this.m_strLog = this.m_strLog.Substring(0, this.m_nMaxChars);
						}
						else
						{
							this.m_strLog = this.m_strLog.Substring(this.m_strLog.Length - this.m_nMaxChars);
						}
					}
				}
				finally
				{
					Monitor.Exit(strLog);
				}
				if (this.OnLogUpdate != null)
				{
					this.OnLogUpdate();
				}
			}
			catch (Exception)
			{
			}
			while (retry)
			{
			}
		}
		public void Persist()
		{
			this.WriteLog();
		}
		public void Clear()
		{
			string strLog;
			Monitor.Enter(strLog = this.m_strLog);
			try
			{
				this.m_strLog = string.Empty;
			}
			finally
			{
				Monitor.Exit(strLog);
			}
			this.WriteLog();
			if (this.OnLogUpdate != null)
			{
				this.OnLogUpdate();
			}
		}
	}
}
