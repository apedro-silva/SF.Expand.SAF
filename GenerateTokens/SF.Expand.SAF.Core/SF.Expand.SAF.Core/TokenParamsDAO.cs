using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using System;
using System.Data;
namespace SF.Expand.SAF.Core
{
	public class TokenParamsDAO : DALSqlServer
	{
		private const string NEW_LINE = "\r\n";
		private const string cBASE_NAME = "SF.Expand.SAF.Core.TokenParamsDAO";
		private const string spGET_TOKEN_BASE_PARAMS = "GetTokenBaseParams";
		public TokenTypeBaseParams loadTokenBaseParams(string tokenParamsID)
		{
			IDataReader dataReader = null;
			IDbCommand dbCommand = null;
			TokenTypeBaseParams result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("GetTokenBaseParams", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param1", tokenParamsID));
				base.Connection.Open();
				dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
				dataReader.Read();
				result = new TokenTypeBaseParams((int)((byte)dataReader[0]), (int)((byte)dataReader[1]), (int)dataReader[2], (long)dataReader[3], (TokenSeedType)((byte)dataReader[4]), (TokenMovingFactorType)((byte)dataReader[5]), (long)((int)dataReader[6]), tokenParamsID, (int)dataReader[7]);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokenParamsDAO::loadTokenBaseParams[]\r\n" + ex.Message, null);
				result = default(TokenTypeBaseParams);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Dispose();
				}
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
