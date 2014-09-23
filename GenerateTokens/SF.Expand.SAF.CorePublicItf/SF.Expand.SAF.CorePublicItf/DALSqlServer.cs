using SF.Expand.SAF.Utils;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SF.Expand.SAF.CorePublicItf
{
	public abstract class DALSqlServer
	{
		private string _dbConnString;
		private IDbConnection _DBConnection;
		private IDbTransaction _DBTransaction;
		public IDbConnection Connection
		{
			get
			{
				if (this._DBConnection == null)
				{
					try
					{
						this._DBConnection = new SqlConnection(this._dbConnString);
					}
					catch (Exception ex)
					{
						LOGGER.Write(LOGGER.LogCategory.ERROR, "{SqlConnection}SF.Expand.SAF.DALSqlServer::Connection" + Environment.NewLine + ex.Message, null);
						LOGGER.Write(LOGGER.LogCategory.ERROR, "{SqlConnection}SF.Expand.SAF.DALSqlServer::Connection [" + this._dbConnString + "]", null);
						return null;
					}
				}
				return this._DBConnection;
			}
		}
		public IDbTransaction Transaction
		{
			get
			{
				return this._DBTransaction;
			}
			set
			{
				this._DBTransaction = value;
			}
		}
		public string ConnectionString
		{
			set
			{
				this._dbConnString = value;
			}
		}
		public void OpenConnection()
		{
			if (this.Connection != null && this.Connection.State == ConnectionState.Closed)
			{
				this._DBConnection.Open();
			}
		}
		public void CloseConnection()
		{
			if (this.Connection != null && this.Connection.State == ConnectionState.Open)
			{
				if (this.Transaction != null)
				{
					this.TransactionRollback();
				}
				this._DBConnection.Close();
				this._DBConnection.Dispose();
			}
		}
		public DataAdapter CreateDataAdapter(IDbCommand _dbComm)
		{
			DataAdapter result;
			try
			{
				result = new SqlDataAdapter((SqlCommand)_dbComm);
			}
			catch
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "{SqlCommand}SF.Expand.SAF.DALSqlServer::CreateAdapter", null);
				result = null;
			}
			return result;
		}
		public IDbCommand CreateCommand(string commandText, CommandType commandType)
		{
			IDbCommand result;
			try
			{
				IDbCommand dbCommand = this.Connection.CreateCommand();
				dbCommand.CommandText = commandText;
				dbCommand.CommandType = commandType;
				result = dbCommand;
			}
			catch (Exception)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "{SqlCommand}SF.Expand.SAF.DALSqlServer::CreateCommand", null);
				result = null;
			}
			return result;
		}
		public IDataParameter AddParameter(string name, object value)
		{
			if (value != null)
			{
				return new SqlParameter(name, value);
			}
			return new SqlParameter(name, DBNull.Value)
			{
				IsNullable = true
			};
		}
		public IDataParameter AddParameterWithType(string name, object value, DbType dbType)
		{
			if (value != null)
			{
				return new SqlParameter(name, value)
				{
					DbType = dbType
				};
			}
			return new SqlParameter(name, DBNull.Value)
			{
				IsNullable = true,
				DbType = dbType
			};
		}
		public IDataParameter AddParameterOutputChar(string name)
		{
			return new SqlParameter(name, SqlDbType.VarChar)
			{
				IsNullable = true,
				Direction = ParameterDirection.Output
			};
		}
		public string GetParameterOutputString(IDbCommand _dbComm, string name)
		{
			string result;
			try
			{
				result = (string)((SqlParameter)_dbComm.Parameters[name]).Value;
			}
			catch
			{
				result = null;
			}
			return result;
		}
		public IDataParameter AddParameterOutputInt32(string name)
		{
			return new SqlParameter(name, SqlDbType.Int)
			{
				IsNullable = true,
				Direction = ParameterDirection.Output
			};
		}
		public void TransactionBegin(IsolationLevel isolationLevel)
		{
			if (this.Transaction == null)
			{
				this.Transaction = this.Connection.BeginTransaction(isolationLevel);
			}
		}
		public void TransactionCommit()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Commit();
				this.Transaction = null;
			}
		}
		public void TransactionRollback()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Rollback();
				this.Transaction = null;
			}
		}
	}
}
