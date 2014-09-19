

using System;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;


namespace SF.Expand.SAF.Utils
{
    public class InMemoryLogging
    {
        private string m_strName = string.Empty;
        private string m_strLog = string.Empty;
        private bool m_bTimestamp = false;
        private bool m_bLineTerminate = true;
        private int m_nMaxChars = -1;
        private bool m_bReverseOrder = false;
        private static Hashtable m_LogsTable = new Hashtable();
        private static StringBuilder m_strBuilder = null;


        /// <summary>
        /// 
        /// </summary>
        private string FileName { get { return m_strName; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readExistingFile"></param>
        private InMemoryLogging(string name, bool readExistingFile)
        {
            m_strName = name;
            if (readExistingFile)
            {
                ReadLog();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadLog()
        {
            if (!File.Exists(FileName))
            {
                return;
            }
            lock (m_strLog)
            {
                using (StreamReader sr = File.OpenText(FileName))
                {
                    m_strLog = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteLog()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            if (m_strLog.Length == 0)
            {
                return;
            }
            lock (m_strLog)
            {
                using (StreamWriter sw = File.CreateText(FileName))
                {
                    sw.Write(m_strLog);
                    sw.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readExistingFile"></param>
        /// <returns></returns>
        public static InMemoryLogging GetLogString(string name, bool readExistingFile)
        {
            return GetLogString(name, readExistingFile, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readExistingFile"></param>
        /// <returns></returns>
        public static InMemoryLogging GetLogString(string name, bool readExistingFile, int baseCapacity)
        {
            if (m_LogsTable.ContainsKey(name))
            {
                return (InMemoryLogging)m_LogsTable[name];
            }
            InMemoryLogging rv = new InMemoryLogging(name, readExistingFile);
            if (baseCapacity > 0)
            {
                m_strBuilder = new StringBuilder(baseCapacity > 1024 ? baseCapacity : 1024);
            }
            m_LogsTable.Add(name, rv);
            return rv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveLogString(string name)
        {
            if (m_LogsTable.ContainsKey(name))
            {
                InMemoryLogging l = (InMemoryLogging)m_LogsTable[name];
                l.Clear();
                m_LogsTable.Remove(name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PersistAll()
        {
            ICollection loglist = m_LogsTable.Values;
            foreach (InMemoryLogging ls in loglist)
            {
                ls.Persist();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearAll()
        {
            ICollection loglist = m_LogsTable.Values;
            foreach (InMemoryLogging ls in loglist)
            {
                ls.Clear();
            }
        }


        public delegate void LogUpdateDelegate();
        public event LogUpdateDelegate OnLogUpdate;

        public string Name { get { return m_strName; } }
        public bool Timestamp { get { return m_bTimestamp; } set { m_bTimestamp = value; } }
        public bool LineTerminate { get { return m_bTimestamp; } set { m_bTimestamp = value; } }
        public bool ReverseOrder { get { return m_bReverseOrder; } set { m_bReverseOrder = value; } }
        public int MaxChars { get { return m_nMaxChars; } set { m_nMaxChars = value; } }


        /// <summary>
        /// 
        /// </summary>
        public string Log
        {
            get
            {
                lock (m_strLog)  // lock the resource
                {
                    return m_strLog;
                }
            }
        }




        public void AddTrace(string str)
        {
            bool retry = false;
            string toadd = "";
            if (m_bTimestamp)
            {
                toadd += DateTime.Now.ToUniversalTime().ToString() + ": ";
            }
            toadd += str;
            if (m_bLineTerminate)
            {
                toadd += "\r\n";
            }
            try
            {
                lock (m_strLog)
                {
                    if (m_bReverseOrder)
                    {
                        m_strLog = toadd + m_strLog;
                    }
                    else
                    {
                        m_strLog += toadd;
                    }
                    if (m_nMaxChars > 1 && m_strLog.Length > m_nMaxChars)
                    {
                        if (m_bReverseOrder)
                        {
                            m_strLog = m_strLog.Substring(0, m_nMaxChars);
                        }
                        else
                        {
                            m_strLog = m_strLog.Substring(m_strLog.Length - m_nMaxChars);
                        }
                    }
                }
                if (OnLogUpdate != null)
                {
                    OnLogUpdate();
                }
            }
            catch (Exception)
            {
                //
            }
            while (retry) ;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void Add(string str)
        {
            bool retry = false;
            string toadd = "";
            if (m_bTimestamp)
            {
                toadd += DateTime.Now.ToUniversalTime().ToString() + ": ";
            }
            toadd += str;
            if (m_bLineTerminate)
            {
                toadd += "\r\n";
            }
            try
            {
                lock (m_strLog)
                {
                    if (m_bReverseOrder)
                    {
                        m_strLog = toadd + m_strLog;
                    }
                    else
                    {
                        m_strLog += toadd;
                    }
                    if (m_nMaxChars > 1 && m_strLog.Length > m_nMaxChars)
                    {
                        if (m_bReverseOrder)
                        {
                            m_strLog = m_strLog.Substring(0, m_nMaxChars);
                        }
                        else
                        {
                            m_strLog = m_strLog.Substring(m_strLog.Length - m_nMaxChars);
                        }
                    }
                }
                if (OnLogUpdate != null)
                {
                    OnLogUpdate();
                }
            }
            catch (Exception)
            {
                //
            }
            while (retry) ;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Persist()
        {
            WriteLog();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (m_strLog)
            {
                m_strLog = string.Empty;
            }
            WriteLog();
            if (OnLogUpdate != null)
            {
                OnLogUpdate();
            }
        }
    }
}