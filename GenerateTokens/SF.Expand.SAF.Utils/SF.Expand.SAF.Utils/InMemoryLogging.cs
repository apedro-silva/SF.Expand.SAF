using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
namespace SF.Expand.SAF.Utils
{
	public class InMemoryLogging
	{
		public delegate void LogUpdateDelegate();
		private string m_strName = string.Empty;
		private string m_strLog = string.Empty;
		private bool m_bTimestamp;
		private bool m_bLineTerminate = true;
		private int m_nMaxChars = -1;
		private bool m_bReverseOrder;
		private static Hashtable m_LogsTable = new Hashtable();
        //public event InMemoryLogging.LogUpdateDelegate OnLogUpdate
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.OnLogUpdate = (InMemoryLogging.LogUpdateDelegate)Delegate.Combine(this.OnLogUpdate, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.OnLogUpdate = (InMemoryLogging.LogUpdateDelegate)Delegate.Remove(this.OnLogUpdate, value);
        //    }
        //}
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
		private InMemoryLogging(string name, bool readExistingFile)
		{
			this.m_strName = name;
			if (readExistingFile)
			{
				this.ReadLog();
			}
		}
		private void ReadLog()
		{
			if (!File.Exists(this.FileName))
			{
				return;
			}
			string strLog;
			Monitor.Enter(strLog = this.m_strLog);
			try
			{
				using (StreamReader streamReader = File.OpenText(this.FileName))
				{
					this.m_strLog = streamReader.ReadToEnd();
					streamReader.Close();
				}
			}
			finally
			{
				Monitor.Exit(strLog);
			}
		}
		private void WriteLog()
		{
			if (File.Exists(this.FileName))
			{
				File.Delete(this.FileName);
			}
			if (this.m_strLog.Length == 0)
			{
				return;
			}
			string strLog;
			Monitor.Enter(strLog = this.m_strLog);
			try
			{
				using (StreamWriter streamWriter = File.CreateText(this.FileName))
				{
					streamWriter.Write(this.m_strLog);
					streamWriter.Close();
				}
			}
			finally
			{
				Monitor.Exit(strLog);
			}
		}
		public static InMemoryLogging GetLogString(string name, bool readExistingFile)
		{
			if (InMemoryLogging.m_LogsTable.ContainsKey(name))
			{
				return (InMemoryLogging)InMemoryLogging.m_LogsTable[name];
			}
			InMemoryLogging inMemoryLogging = new InMemoryLogging(name, readExistingFile);
			InMemoryLogging.m_LogsTable.Add(name, inMemoryLogging);
			return inMemoryLogging;
		}
		public static void RemoveLogString(string name)
		{
			if (InMemoryLogging.m_LogsTable.ContainsKey(name))
			{
				InMemoryLogging inMemoryLogging = (InMemoryLogging)InMemoryLogging.m_LogsTable[name];
				inMemoryLogging.Clear();
				InMemoryLogging.m_LogsTable.Remove(name);
			}
		}
		public static void PersistAll()
		{
			ICollection values = InMemoryLogging.m_LogsTable.Values;
			foreach (InMemoryLogging inMemoryLogging in values)
			{
				inMemoryLogging.Persist();
			}
		}
		public static void ClearAll()
		{
			ICollection values = InMemoryLogging.m_LogsTable.Values;
			foreach (InMemoryLogging inMemoryLogging in values)
			{
				inMemoryLogging.Clear();
			}
		}
		public void AddTrace(string str)
		{
			bool flag = false;
			string text = "";
			if (this.m_bTimestamp)
			{
				text = text + DateTime.Now.ToUniversalTime().ToString() + ": ";
			}
			text += str;
			if (this.m_bLineTerminate)
			{
				text += "\r\n";
			}
			try
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					if (this.m_bReverseOrder)
					{
						this.m_strLog = text + this.m_strLog;
					}
					else
					{
						this.m_strLog += text;
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
                //if (this.OnLogUpdate != null)
                //{
                //    this.OnLogUpdate();
                //}
			}
			catch (Exception)
			{
			}
			while (flag)
			{
			}
		}
		public void Add(string str)
		{
			bool flag = false;
			string text = "";
			if (this.m_bTimestamp)
			{
				text = text + DateTime.Now.ToUniversalTime().ToString() + ": ";
			}
			text += str;
			if (this.m_bLineTerminate)
			{
				text += "\r\n";
			}
			try
			{
				string strLog;
				Monitor.Enter(strLog = this.m_strLog);
				try
				{
					if (this.m_bReverseOrder)
					{
						this.m_strLog = text + this.m_strLog;
					}
					else
					{
						this.m_strLog += text;
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
                //if (this.OnLogUpdate != null)
                //{
                //    this.OnLogUpdate();
                //}
			}
			catch (Exception)
			{
			}
			while (flag)
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
            //if (this.OnLogUpdate != null)
            //{
            //    this.OnLogUpdate();
            //}
		}
	}
}
