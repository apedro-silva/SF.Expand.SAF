using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
namespace SF.Expand.Secure.Business
{
	public class TokensBusinessEventsDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/";
		private const string spTOKEN_EVENTS_INSERT = "TokenEventsInsert";
		private const string spTOKEN_EVENTS_DELETE = "TokenEventsDelete";
		private const string spTOKEN_EVENTS_UPDATE_STATUS = "TokenEventsUpdateStatus";
		private const string spGET_TOKENS_EVENTS_PAG = "GetTokensEventsPAG";
		private const string spGET_TOKENID_BY_EVENT_DEPLOY = "GetTokenIdByEventDeploy";
		private const string spTOKEN_EVENTS_FIELDS_INSERT = "TokenEventsFieldsInsert";
		private const string spTOKEN_EVENTS_FIELDS = "TokenEventsFields";
		private const string spTOKEN_EVENTS_FIELDS_DELETE_BY_EVENTID = "TokenEventsFieldsDelete";
		private void GetTotRows(IDbCommand cmd, out int totRows)
		{
			totRows = -1;
			if (cmd.Parameters.Contains("@totRows"))
			{
				IDataParameter totParam = (IDataParameter)cmd.Parameters["@totRows"];
				if (totParam != null && totParam.Value is int)
				{
					totRows = (int)totParam.Value;
				}
			}
		}
		private void _PrepPagination(IDbCommand _cmd, int iOffset, int pageSize, int pageDirection)
		{
			_cmd.Parameters.Add(base.AddParameter("@offset", iOffset));
			_cmd.Parameters.Add(base.AddParameter("@maximumRows", pageSize));
			_cmd.Parameters.Add(base.AddParameter("@forw", pageDirection));
			_cmd.Parameters.Add(base.AddParameterOutputInt32("@totRows"));
		}
		private TokensEvents[] _loadBusinessEvents(IDbCommand cmd, out int totRows)
		{
			totRows = -1;
			IDataReader _dr = null;
			List<TokensEvents> _tokensEvents = new List<TokensEvents>();
			TokensEvents[] result;
			try
			{
				base.Connection.Open();
				_dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tokensEvents.Add(this._buildBusinessEventRequest(_dr, false));
				}
				this.GetTotRows(cmd, out totRows);
				result = _tokensEvents.ToArray();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
				if (_dr != null)
				{
					_dr.Dispose();
				}
				if (cmd != null)
				{
					cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		private TokensEvents _buildBusinessEventRequest(IDataReader dReader, bool readNext)
		{
			TokensEvents result;
			try
			{
				if (readNext)
				{
					dReader.Read();
				}
				result = TokensEvents.loadTokensEvents(dReader.GetInt64(0), (dReader[1] != DBNull.Value) ? dReader.GetDateTime(1).ToString() : "", dReader.GetInt32(2), (dReader[3] != DBNull.Value) ? dReader.GetString(3) : "", (dReader[4] != DBNull.Value) ? dReader.GetString(4) : "", (dReader[5] != DBNull.Value) ? dReader.GetDateTime(5).ToString() : "", (dReader[6] != DBNull.Value) ? dReader.GetByte(6).ToString() : "", (dReader[7] != DBNull.Value) ? dReader.GetByte(7).ToString() : "");
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			return result;
		}
		public OperationResult loadDeployEventInfo(string deployEventID, out DataTable dataTable)
		{
			DataSet _ds = new DataSet("EventDeploy");
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenIdByEventDeploy", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", deployEventID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				dataTable = null;
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult insertTokenEvent(string tokenID, int eventOperation, int eventOperationStatus, string applicationUser, out long tokenEventID)
		{
			tokenEventID = -1L;
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenEventsInsert", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param1", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@param2", eventOperation));
				_cmd.Parameters.Add(base.AddParameter("@param3", (byte)eventOperationStatus));
				_cmd.Parameters.Add(base.AddParameter("@param4", applicationUser));
				base.Connection.Open();
				tokenEventID = long.Parse(_cmd.ExecuteScalar().ToString());
				result = ((0L != tokenEventID) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
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
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult insertTokenEventFields(long tokenEventID, string tokenEventFields)
		{
			IDbCommand _cmd = null;
			string[] _arrayEventFields = tokenEventFields.Split(new char[]
			{
				'|'
			});
			OperationResult result;
			if (_arrayEventFields.Length % 2 != 0 || _arrayEventFields.Length < 4)
			{
				result = OperationResult.Error;
			}
			else
			{
				try
				{
					base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
					_cmd = base.CreateCommand("TokenEventsFieldsInsert", CommandType.StoredProcedure);
					_cmd.Parameters.Add(base.AddParameter("@param0", tokenEventID));
					_cmd.Parameters.Add(base.AddParameter("@param1", null));
					_cmd.Parameters.Add(base.AddParameter("@param2", null));
					base.Connection.Open();
					for (int i = 0; i < _arrayEventFields.Length; i += 2)
					{
						((IDataParameter)_cmd.Parameters["@param1"]).Value = _arrayEventFields[i];
						((IDataParameter)_cmd.Parameters["@param2"]).Value = _arrayEventFields[i + 1];
						_cmd.ExecuteNonQuery();
					}
					result = OperationResult.Success;
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
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
					base.CloseConnection();
				}
			}
			return result;
		}
		public TokensEventFields[] loadAllTokenEventFields(long tokenEventID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			List<TokensEventFields> _tokensEventsFields = new List<TokensEventFields>();
			TokensEventFields[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenEventsFields", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", tokenEventID));
				base.Connection.Open();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tokensEventsFields.Add(TokensEventFields.loadTokensEventFields((long)_dr[0], (DateTime)_dr[1], (string)_dr[2], (string)_dr[3]));
				}
				result = _tokensEventsFields.ToArray();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
				if (_dr != null)
				{
					_dr.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult updateEventStatus(string tokenEventID, byte tokenStatus)
		{
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenEventsUpdateStatus", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param1", tokenStatus));
				_cmd.Parameters.Add(base.AddParameter("@param2", tokenEventID));
				base.Connection.Open();
				result = ((_cmd.ExecuteNonQuery() != 0) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
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
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult deleteEvent(string tokenEventID)
		{
			IDbCommand _cmd = null;
			IDbCommand _cmdFlds = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenEventsDelete", CommandType.StoredProcedure);
				_cmdFlds = base.CreateCommand("TokenEventsFieldsDelete", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", tokenEventID));
				_cmdFlds.Parameters.Add(base.AddParameter("@param0", tokenEventID));
				base.Connection.Open();
				_cmdFlds.ExecuteNonQuery();
				result = ((_cmd.ExecuteNonQuery() != 0) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmdFlds != null)
				{
					_cmdFlds.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public TokensEvents[] loadAllEvents(DateTime dateInitial, DateTime dateFinal, string tokenParamsID, string operationID, string tokenID, string applicationUser, string eventOperationStatus, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			TokensEvents[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensEventsPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", dateInitial));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", dateFinal));
				_cmd.Parameters.Add(base.AddParameter("@TokenParamsID", tokenParamsID));
				_cmd.Parameters.Add(base.AddParameter("@EventOperationID", operationID));
				_cmd.Parameters.Add(base.AddParameter("@TokenID", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@ApplicationUser", applicationUser));
				_cmd.Parameters.Add(base.AddParameter("@EventOperationStatus", eventOperationStatus));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadBusinessEvents(_cmd, out totRows);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = null;
			}
			finally
			{
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
