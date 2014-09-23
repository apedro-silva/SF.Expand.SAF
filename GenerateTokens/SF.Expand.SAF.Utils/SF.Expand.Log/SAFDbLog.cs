using System;
using System.Data;
using System.Data.SqlClient;
namespace SF.Expand.Log
{
	public class SAFDbLog
	{
		private string m_connectionString;
		public string ConnectionString
		{
			get
			{
				return this.m_connectionString;
			}
			set
			{
				this.m_connectionString = value;
			}
		}
		public virtual int DoAppend(SAFLogData logData)
		{
			int result = -1;
			using (IDbConnection connection = this.GetConnection())
			{
				connection.Open();
				using (IDbCommand dbCommand = connection.CreateCommand())
				{
					this.InitializeCommandInsert(dbCommand);
					this.SetCommandValues(dbCommand, logData);
					result = (int)dbCommand.ExecuteScalar();
				}
			}
			return result;
		}
		public virtual void UpdateLog(int logID, SAFLogData logData)
		{
			using (IDbConnection connection = this.GetConnection())
			{
				connection.Open();
				using (IDbCommand dbCommand = connection.CreateCommand())
				{
					this.InitializeCommandUpdate(dbCommand);
					this.SetCommandValues(dbCommand, logData);
					dbCommand.ExecuteNonQuery();
				}
			}
		}
		protected virtual IDbConnection GetConnection()
		{
			return new SqlConnection(this.m_connectionString);
		}
		protected virtual void InitializeCommandInsert(IDbCommand command)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "LogEntryInsert";
			IDbDataParameter dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Date";
			dbDataParameter.DbType = DbType.DateTime;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Thread";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Level";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Logger";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@User";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Message";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Exception";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
		}
		protected virtual void InitializeCommandUpdate(IDbCommand command)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "LogEntryInfoUpdate";
			IDbDataParameter dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Id";
			dbDataParameter.DbType = DbType.Int32;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Level";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Message";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
			dbDataParameter = command.CreateParameter();
			dbDataParameter.ParameterName = "@Exception";
			dbDataParameter.DbType = DbType.String;
			command.Parameters.Add(dbDataParameter);
		}
		protected virtual void SetCommandValues(IDbCommand command, SAFLogData logData)
		{
			if (command.Parameters.Contains("@Id"))
			{
				((IDbDataParameter)command.Parameters["@Id"]).Value = logData.Id;
			}
			if (command.Parameters.Contains("@Level"))
			{
				if (logData.LevelName == null || logData.LevelName == string.Empty)
				{
					((IDbDataParameter)command.Parameters["@Level"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Level"]).Value = logData.LevelName;
				}
			}
			if (command.Parameters.Contains("@Message"))
			{
				if (logData.MessageString == null || logData.MessageString == string.Empty)
				{
					((IDbDataParameter)command.Parameters["@Message"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Message"]).Value = logData.MessageString;
				}
			}
			if (command.Parameters.Contains("@Exception"))
			{
				if (logData.ExceptionString == null || logData.ExceptionString == string.Empty)
				{
					((IDbDataParameter)command.Parameters["@Exception"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Exception"]).Value = logData.ExceptionString;
				}
			}
			if (command.Parameters.Contains("@Date"))
			{
				if (logData.TimeStampUTC == DateTime.MinValue)
				{
					((IDbDataParameter)command.Parameters["@Date"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Date"]).Value = logData.TimeStampUTC;
				}
			}
			if (command.Parameters.Contains("@Thread"))
			{
				if (logData.ThreadName == null)
				{
					((IDbDataParameter)command.Parameters["@Thread"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Thread"]).Value = logData.ThreadName;
				}
			}
			if (command.Parameters.Contains("@Logger"))
			{
				if (logData.LoggerName == null || logData.LoggerName == string.Empty)
				{
					((IDbDataParameter)command.Parameters["@Logger"]).Value = DBNull.Value;
				}
				else
				{
					((IDbDataParameter)command.Parameters["@Logger"]).Value = logData.LoggerName;
				}
			}
			if (command.Parameters.Contains("@User"))
			{
				if (logData.UserName == null || logData.UserName == string.Empty)
				{
					((IDbDataParameter)command.Parameters["@User"]).Value = DBNull.Value;
					return;
				}
				((IDbDataParameter)command.Parameters["@User"]).Value = logData.UserName;
			}
		}
	}
}
