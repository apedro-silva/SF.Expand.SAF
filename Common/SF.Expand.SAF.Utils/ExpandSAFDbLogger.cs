using System;
using System.Data;
using System.Data.SqlClient;

namespace SF.Expand.Log
{
    public class SAFLogData
    {
        public SAFLogData(int trnCode, string trnName, string loggerName, string userName)
        {
            LoggerName = loggerName;
            UserName = userName;
            TrnName = trnName;
            ThreadName = "";
            LevelName = "ERROR";
            TrnCode = trnCode;
            TrnRes = -9;
            InitialTimeStamp = DateTime.Now;
        }

        private string FormatMessage(string msg)
        {
            if (messageString == null)
                return messageString;

            return msg.Replace("$TRC$", TrnCode.ToString()).Replace("$TRN$", TrnName);
        }

        public string MessageString
        {
            set { messageString = value; }
            get
            {
                return FormatMessage(messageString);
            }
        }

        public int Id = -1;
        public DateTime TimeStampUTC = DateTime.MinValue;
        public string ThreadName = null;
        public string LevelName = null;
        public string LoggerName = null;
        public string UserName = null;
        public string ExceptionString = null;
        public int TrnCode = 0;
        public int TrnRes = 0;
        public DateTime InitialTimeStamp;
        private string messageString = null;
        public string TrnName = "";
    }

    public class SAFDbLog
    {
        public SAFDbLog()
        {
        }

        public SAFDbLog(string connStr)
        {
            m_connectionString = connStr;
        }

        private string m_connectionString;

        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        public virtual int AppendLog(SAFLogData logData)
        {
            logData.Id = -1;
            using (IDbConnection connection = GetConnection())
            {
                // Open the connection for each event, this takes advantage
                // of the builtin connection pooling
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    InitializeCommandInsert(command);

                    SetCommandValues(command, logData);
                    command.ExecuteNonQuery();
                    //logID = (int)command.ExecuteScalar();

                    if (command.Parameters.Contains("@LogID"))
                    {
                        IDataParameter logID = command.Parameters["@LogID"] as IDataParameter;
                        if (logID != null && logID.Value is int)
                            logData.Id = (int)logID.Value;
                    }
                }
            }

            return logData.Id;
        }

        public virtual void UpdateLog(SAFLogData logData)
        {
            using (IDbConnection connection = GetConnection())
            {
                // Open the connection for each event, this takes advantage
                // of the builtin connection pooling
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    InitializeCommandUpdate(command);

                    SetCommandValues(command, logData);
                    command.ExecuteNonQuery();
                }
            }
        }

        virtual protected IDbConnection GetConnection()
        {
            return new SqlConnection(m_connectionString);
        }

        virtual protected void InitializeCommandInsert(IDbCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "LogEntryInsert";

            IDbDataParameter param = null;

            // @Date
            param = command.CreateParameter();
            param.ParameterName = "@Date";
            param.DbType = DbType.DateTime;
            command.Parameters.Add(param);

            // @Thread
            param = command.CreateParameter();
            param.ParameterName = "@Thread";
            param.DbType = DbType.String;
            command.Parameters.Add(param);
            
            // @Level
            param = command.CreateParameter();
            param.ParameterName = "@Level";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @Logger
            param = command.CreateParameter();
            param.ParameterName = "@Logger";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @User
            param = command.CreateParameter();
            param.ParameterName = "@User";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @Message
            param = command.CreateParameter();
            param.ParameterName = "@Message";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @Exception
            param = command.CreateParameter();
            param.ParameterName = "@Exception";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @TrnCode
            param = command.CreateParameter();
            param.ParameterName = "@TrnCode";
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

            // @TrnRes
            param = command.CreateParameter();
            param.ParameterName = "@TrnRes";
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

            // @ResponseTime
            param = command.CreateParameter();
            param.ParameterName = "@ResponseTime";
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

            // @LogID
            param = command.CreateParameter();
            param.ParameterName = "@LogID";
            param.DbType = DbType.Int32;
            param.Direction = ParameterDirection.Output;
            command.Parameters.Add(param);
        }

        virtual protected void InitializeCommandUpdate(IDbCommand command)
        {
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "LogEntryInfoUpdate";

            IDbDataParameter param = null;

            // @Id
            param = command.CreateParameter();
            param.ParameterName = "@Id";
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

            // @Level
            param = command.CreateParameter();
            param.ParameterName = "@Level";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @Message
            param = command.CreateParameter();
            param.ParameterName = "@Message";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @Exception
            param = command.CreateParameter();
            param.ParameterName = "@Exception";
            param.DbType = DbType.String;
            command.Parameters.Add(param);

            // @TrnRes
            param = command.CreateParameter();
            param.ParameterName = "@TrnRes";
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);
        }

        virtual protected void SetCommandValues(IDbCommand command, SAFLogData logData)
        {
            //Update Only
            if (command.Parameters.Contains("@Id"))
                ((IDbDataParameter)command.Parameters["@Id"]).Value = logData.Id;

            //Insert & Update
            if (command.Parameters.Contains("@Level"))
                if (string.IsNullOrEmpty(logData.LevelName))
                    ((IDbDataParameter)command.Parameters["@Level"]).Value = DBNull.Value; //It reverts to 'INFO'
                else
                    ((IDbDataParameter)command.Parameters["@Level"]).Value = logData.LevelName;

            //Insert & Update
            if (command.Parameters.Contains("@Message"))
            {
                if (string.IsNullOrEmpty(logData.MessageString))
                    ((IDbDataParameter)command.Parameters["@Message"]).Value = DBNull.Value;
                else
                    ((IDbDataParameter)command.Parameters["@Message"]).Value = logData.MessageString;
            }

            //Insert & Update
            if (command.Parameters.Contains("@Exception"))
                if (string.IsNullOrEmpty(logData.ExceptionString))
                    ((IDbDataParameter)command.Parameters["@Exception"]).Value = DBNull.Value;
                else
                    ((IDbDataParameter)command.Parameters["@Exception"]).Value = logData.ExceptionString;

            //Insert Only
            if (command.Parameters.Contains("@Date"))
                if (logData.TimeStampUTC == DateTime.MinValue)
                    ((IDbDataParameter)command.Parameters["@Date"]).Value = DBNull.Value;
                else
                    ((IDbDataParameter)command.Parameters["@Date"]).Value = logData.TimeStampUTC;

            //Insert Only
            if (command.Parameters.Contains("@Thread"))
                if (string.IsNullOrEmpty(logData.ThreadName))
                    ((IDbDataParameter)command.Parameters["@Thread"]).Value = string.Empty;
                else
                    ((IDbDataParameter)command.Parameters["@Thread"]).Value = logData.ThreadName;

            //Insert Only
            if (command.Parameters.Contains("@Logger"))
                if (string.IsNullOrEmpty(logData.LoggerName))
                    ((IDbDataParameter)command.Parameters["@Logger"]).Value = string.Empty;
                else
                    ((IDbDataParameter)command.Parameters["@Logger"]).Value = logData.LoggerName;

            //Insert Only
            if (command.Parameters.Contains("@User"))
                if (string.IsNullOrEmpty(logData.UserName))
                    ((IDbDataParameter)command.Parameters["@User"]).Value = DBNull.Value;
                else
                    ((IDbDataParameter)command.Parameters["@User"]).Value = logData.UserName;

            //Insert Only
            if (command.Parameters.Contains("@TrnCode"))
                ((IDbDataParameter)command.Parameters["@TrnCode"]).Value = logData.TrnCode;

            //Insert & Update
            if (command.Parameters.Contains("@TrnRes"))
                ((IDbDataParameter)command.Parameters["@TrnRes"]).Value = logData.TrnRes;

            //Insert & Update
            if (command.Parameters.Contains("@ResponseTime"))
                ((IDbDataParameter)command.Parameters["@ResponseTime"]).Value = (DateTime.Now-logData.InitialTimeStamp).Milliseconds;

        }
    }
}
