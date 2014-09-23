using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Utils;
using System;
using System.Data;
namespace SF.Expand.SAF.Core
{
	public class TokensChallengeRequestDAO : DALSqlServer
	{
		private const string NEW_LINE = "\r\n";
		private const string cBASE_NAME = "SF.Expand.SAF.Core.TokensChallengeRequestDAO";
		private static string spINSERT_NEW_CHALLENGE_REQUEST = "InsertNewChallengeRequest";
		private static string spVALID_CHALLENGE_REQUEST_BY_TOKENID = "ValidChallengeRequestByTokenID";
		private static string spGET_CHALLENGE_REQUEST_BY_TOKENID = "GetChallengeRequestByTokenID";
		private static string spRESET_CHALLENGE_REQUEST_BY_TOKENID = "ResetChallengeRequestByTokenID";
		private static string spCLEAR_OVERDUE_CHALLENGE_REQUEST = "ClearOverdueChallengeRequest";
		public object loadChallengeRequest(string tokenID)
		{
			IDbCommand dbCommand = null;
			IDbCommand dbCommand2 = null;
			object result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand2 = base.CreateCommand(TokensChallengeRequestDAO.spVALID_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
				dbCommand2.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				if (1 == (int)dbCommand2.ExecuteScalar())
				{
					dbCommand2 = base.CreateCommand(TokensChallengeRequestDAO.spGET_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
					dbCommand2.Parameters.Add(base.AddParameter("@tkID", tokenID));
					result = dbCommand2.ExecuteScalar();
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core::loadChallengeRequest[]\r\n" + ex.Message, null);
				result = null;
			}
			finally
			{
				if (dbCommand2 != null)
				{
					dbCommand2.Dispose();
				}
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult persistChallengeRequest(string tokenID, string challengeRequest, DateTime challengeRequestValidThru)
		{
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand(TokensChallengeRequestDAO.spINSERT_NEW_CHALLENGE_REQUEST, CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				dbCommand.Parameters.Add(base.AddParameter("@param1", challengeRequest));
				dbCommand.Parameters.Add(base.AddParameter("@param2", challengeRequestValidThru));
				base.Connection.Open();
				result = ((dbCommand.ExecuteNonQuery() == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensChallengeRequest::persistChallengeRequest[]\r\n" + ex.Message, null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (base.Connection != null && base.Connection.State == ConnectionState.Open)
				{
					base.Connection.Dispose();
				}
			}
			return result;
		}
		public OperationResult resetChallengeRequest(string tokenID)
		{
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand(TokensChallengeRequestDAO.spRESET_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				result = ((dbCommand.ExecuteNonQuery() == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensChallengeRequest::resetChallengeRequest[]\r\n" + ex.Message, null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (base.Connection != null && base.Connection.State == ConnectionState.Open)
				{
					base.Connection.Dispose();
				}
			}
			return result;
		}
		public long clearChallengeRequestOverdue(string tokenID)
		{
			IDbCommand dbCommand = null;
			long result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand(TokensChallengeRequestDAO.spCLEAR_OVERDUE_CHALLENGE_REQUEST, CommandType.StoredProcedure);
				base.Connection.Open();
				result = (long)dbCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensChallengeRequest::clearChallengeRequestOverdue[]\r\n" + ex.Message, null);
				result = -1L;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (base.Connection != null && base.Connection.State == ConnectionState.Open)
				{
					base.Connection.Dispose();
				}
			}
			return result;
		}
	}
}
