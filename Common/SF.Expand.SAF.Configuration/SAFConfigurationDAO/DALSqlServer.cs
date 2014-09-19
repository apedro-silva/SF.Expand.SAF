using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace SAFConfigurationDAO
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
				IDbConnection result;
				if (this._DBConnection == null)
				{
					try
					{
						this._DBConnection = new SqlConnection(this._dbConnString);
					}
					catch
					{
						result = null;
						return result;
					}
				}
				result = this._DBConnection;
				return result;
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
			return new SqlDataAdapter((SqlCommand)_dbComm);
		}
		public IDbCommand CreateCommand(string commandText, CommandType commandType)
		{
			IDbCommand _cmd = this.Connection.CreateCommand();
			_cmd.CommandText = commandText;
			_cmd.CommandType = commandType;
			return _cmd;
		}
		public IDataParameter AddParameter(string name, object value)
		{
			IDataParameter result;
			if (value != null)
			{
				result = new SqlParameter(name, value);
			}
			else
			{
				result = new SqlParameter(name, DBNull.Value)
				{
					IsNullable = true
				};
			}
			return result;
		}
		public IDataParameter AddParameterWithType(string name, object value, DbType dbType)
		{
			IDataParameter result;
			if (value != null)
			{
				result = new SqlParameter(name, value)
				{
					DbType = dbType
				};
			}
			else
			{
				result = new SqlParameter(name, DBNull.Value)
				{
					IsNullable = true,
					DbType = dbType
				};
			}
			return result;
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
				Direction = ParameterDirection.Output,
				IsNullable = true
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
