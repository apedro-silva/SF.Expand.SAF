using SF.Expand.SAF.Utils;
using System;
using System.Data;
using System.Data.OracleClient;
namespace SF.Expand.SAF.CorePublicItf
{
	public abstract class DALOracle
	{
		private IDbConnection _DBConnection;
		private IDbTransaction _DBTransaction;
		private string _dbConnString;
		public IDbConnection Connection
		{
			get
			{
				if (this._DBConnection == null)
				{
					try
					{
						this._DBConnection = new OracleConnection(this._dbConnString);
					}
					catch (Exception)
					{
						LOGGER.Write(LOGGER.LogCategory.ERROR, "{OracleConnection}SF.Expand.SAF.DALOracle::Connection", null);
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
		public void CloseConnection()
		{
			if (this.Connection != null && this.Connection.State == ConnectionState.Open)
			{
				if (this.Transaction != null)
				{
					this.RollbackTrans();
				}
				this.Connection.Close();
				this.Connection.Dispose();
			}
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
				LOGGER.Write(LOGGER.LogCategory.ERROR, "{OracleParameter}SF.Expand.SAF.DALOracle::CreateCommand", null);
				result = null;
			}
			return result;
		}
		public IDataParameter AddParameter(string name, object value)
		{
			return new OracleParameter(name, value);
		}
		public void BeginTrans()
		{
			this.BeginTrans(IsolationLevel.ReadCommitted);
		}
		public void BeginTrans(IsolationLevel isoLevel)
		{
			if (this.Transaction == null)
			{
				this.Transaction = this.Connection.BeginTransaction(isoLevel);
			}
		}
		public void CommitTrans()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Commit();
				this.Transaction.Dispose();
			}
		}
		public void RollbackTrans()
		{
			if (this.Transaction != null)
			{
				this.Transaction.Rollback();
				this.Transaction.Dispose();
			}
		}
	}
}
