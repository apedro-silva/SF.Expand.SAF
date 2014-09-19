using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Log;

namespace SF.Expand.SAF.Utils
{
    public class SAFStatsData
    {
        public SAFStatsData(int trnCode, byte serverID, short trnRes)
        {
            TrnCode = (short)trnCode;
            ServerID = serverID;
            TrnRes = trnRes;
        }

        public byte ServerID = 0;
        public short TrnCode;
        public byte Attr0 = 0; //TokenType
        public short TrnRes;
        public int RTIntervalSecs = 5;
        public DateTime TimeStampUTC = DateTime.MinValue;
        
        //public int Id = -1;
        //public DateTime TimeStampUTC = DateTime.MinValue;
        //public string ThreadName = null;
        //public string LevelName = null;
        //public string LoggerName = null;
        //public string UserName = null;
        //public string MessageString = null;
        //public string ExceptionString = null;
    }

    public class WSStackerData
    {
        private WSStackerData() //cannot be called. Must use another constructor
        {
        }

        public WSStackerData(string trnName, int trnCode, byte serverID, short trnRes, string loggerName, string user)
        {
            StatsData = new SAFStatsData(trnCode, serverID, trnRes);

            LogData = new SAFLogData(trnCode, trnName, loggerName, user);
        }

        //SuppressStore flags
        internal bool suppressStats = false;
        internal bool suppressLog = false;

        public string UserName
        {
            get { return LogData.UserName; }
            set { LogData.UserName = value; }
        }

        public short TrnRes
        {
            get { return StatsData.TrnRes; }
            set { StatsData.TrnRes = value; }
        }

        public string Message
        {
            get { return LogData.MessageString; }
            set { LogData.MessageString = value; }
        }

        public DateTime TimeStampUTC
        {
            set {
                LogData.TimeStampUTC = value;
                StatsData.TimeStampUTC = value;
            }
        }

        public void SetTrnResAndMessage(short trnRes, string message)
        {
            StatsData.TrnRes = trnRes;
            LogData.MessageString = message;
            LogData.TrnRes = trnRes;
        }

        public void SetTrnResAndMessageUTC(short trnRes, string message, DateTime timeStampUTC)
        {
            StatsData.TrnRes = trnRes;
            LogData.MessageString = message;
            TimeStampUTC = timeStampUTC;
            LogData.TrnRes = trnRes;
        }

        //Stats Data
        public SAFStatsData StatsData;
        //Log Data
        public SAFLogData LogData;
    }

    public class WSStacker : IDisposable
    {
        /// <summary>
        /// To signal whether the object has already been disposed
        /// </summary>
        private bool disposed = false;

        private WSStackerData stackData = null;

        private WSStacker() //cannot be called. Must use another constructor
        {
        }

        public WSStacker(WSStackerData stackData)
        {
            if (stackData == null)
                throw new ArgumentException("stackData cannot be null.", "stackData");

            this.stackData = stackData;
        }

        ~WSStacker()
        {
            Dispose(false);
        }

        public WSStackerData SD
        {
            get { return stackData; }
        }

        #region IDisposable Members

        /// <summary>
        /// Implementation of the Dispose method of IDisposable interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Take yourself off the Finalization queue
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Managed cleanup code here, only managed refs. Dispose / Clean / Close them...

                StoreStackStats();
                StoreStackLog();
            }

            // Unmanaged cleanup code here
            // ...

            disposed = true;
        }

        #endregion

        public void SuppressStats(bool suppress)
        {
            stackData.suppressStats = suppress;
        }

        public void SuppressLog(bool suppress)
        {
            stackData.suppressLog = suppress;
        }


        /// <summary>
        /// StoreStackStats persists Statistics data.
        /// Remember that it will be automatically called during Dispose (even if explicitly being called once) if suppressStats is false.
        /// </summary>
        /// <remarks>
        /// Remember that it will be automatically called during Dispose (even if explicitly being called once) if suppressStats is false.
        /// </remarks>
        /// <returns></returns>
        public virtual bool StoreStackStats()
        {
            //if (disposed)
            //    return false;
            //if (stackData.suppressStats)
            //    return true;

            //try
            //{
            //    Exception exRes;
            //    SAFStatsData sd = stackData.StatsData;
            //    string statsConnStr = SAFConfiguration.readConnectionStringBusiness();

            //    SF.Expand.SAF.Stats.Store store = new SF.Expand.SAF.Stats.Store(statsConnStr);
            //    if (sd.TimeStampUTC==DateTime.MinValue)
            //        exRes = store.SetStatisticsNow(sd.ServerID, sd.TrnCode, sd.Attr0, sd.TrnRes, sd.RTIntervalSecs);
            //    else
            //        exRes = store.SetStatisticsWithAppDate(sd.ServerID, sd.TrnCode, sd.Attr0, sd.TrnRes, sd.TimeStampUTC, sd.RTIntervalSecs);

            //    if (exRes == null)
            //        return true;

            //    LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Utils.WSStacker::StoreStackStats. SetStatistics failed with Err:" + exRes.Message, exRes);
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Utils.WSStacker::StoreStackStats. Err:" + ex.Message, ex);
            //}
            return false;
        }

        /// <summary>
        /// StoreStackLog Logs to storage.
        /// Remember that it will be automatically called during Dispose (even if explicitly being called once) if suppressLog is false.
        /// </summary>
        /// <remarks>
        /// Remember that it will be automatically called during Dispose (even if explicitly being called once) if suppressLog is false.
        /// </remarks>
        /// <returns></returns>
        public virtual bool StoreStackLog()
        {
            if (disposed)
                return false;
            if (stackData.suppressLog)
                return true;

            try
            {
                short trnRes = stackData.StatsData.TrnRes;
                if (trnRes < 0)
                    stackData.LogData.LevelName = "ERROR";
                else
                    stackData.LogData.LevelName = "INFO";

                SAFDbLog dbLog = new SAFDbLog(SAFConfiguration.readConnectionStringBusiness());

                int idLog = dbLog.AppendLog(stackData.LogData);

                return (idLog!=-1);
            }
            catch (Exception ex)
            {
                LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Utils.WSStacker::StoreStackLog. Err:" + ex.Message, ex);
            }
            return false;
        }
    }
}
