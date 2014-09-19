using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public class TokensChallengeRequestDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokensChallengeRequestDAO.softfinanca.com/";
		private static string spINSERT_NEW_CHALLENGE_REQUEST = "InsertNewChallengeRequest";
		private static string spVALID_CHALLENGE_REQUEST_BY_TOKENID = "ValidChallengeRequestByTokenID";
		private static string spGET_CHALLENGE_REQUEST_BY_TOKENID = "GetChallengeRequestByTokenID";
		private static string spRESET_CHALLENGE_REQUEST_BY_TOKENID = "ResetChallengeRequestByTokenID";
		private static string spCLEAR_OVERDUE_CHALLENGE_REQUEST = "ClearOverdueChallengeRequest";
		public object loadChallengeRequest(string tokenID)
		{
			IDbCommand _cmd = null;
			IDbCommand _cmdValid = null;
			object result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmdValid = base.CreateCommand(TokensChallengeRequestDAO.spVALID_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
				_cmdValid.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				if (1 == (int)_cmdValid.ExecuteScalar())
				{
					_cmdValid = base.CreateCommand(TokensChallengeRequestDAO.spGET_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
					_cmdValid.Parameters.Add(base.AddParameter("@tkID", tokenID));
					result = _cmdValid.ExecuteScalar();
				}
				else
				{
					result = null;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensChallengeRequestDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
				if (_cmdValid != null)
				{
					_cmdValid.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult persistChallengeRequest(string tokenID, string challengeRequest, DateTime challengeRequestValidThru)
		{
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand(TokensChallengeRequestDAO.spINSERT_NEW_CHALLENGE_REQUEST, CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@param1", challengeRequest));
				_cmd.Parameters.Add(base.AddParameter("@param2", challengeRequestValidThru));
				base.Connection.Open();
				result = ((_cmd.ExecuteNonQuery() == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensChallengeRequestDAO.softfinanca.com/",
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
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
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand(TokensChallengeRequestDAO.spRESET_CHALLENGE_REQUEST_BY_TOKENID, CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				result = ((_cmd.ExecuteNonQuery() == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensChallengeRequestDAO.softfinanca.com/",
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
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
			IDbCommand _cmd = null;
			long result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand(TokensChallengeRequestDAO.spCLEAR_OVERDUE_CHALLENGE_REQUEST, CommandType.StoredProcedure);
				base.Connection.Open();
				result = (long)_cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensChallengeRequestDAO.softfinanca.com/",
					ex.ToString()
				});
				result = -1L;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
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
