using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
namespace SF.Expand.Secure.Business
{
	public class TokenBusinessDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/";
		private const string spGET_TOKENS_USER_ALL_PAG = "GetTokensUserAllPAG";
		private const string spGET_TOKENS_USER_ALL = "GetTokensUserAll";
		private const string spGET_TOKENS_USER_BY_PHONE_PAG = "GetTokensUserByPhonePAG";
		private const string spGET_TOKENS_USER_BY_PHONE = "GetTokensUserByPhone";
		private const string spGET_VALIDATIONS_COUNT = "GetTokenValidationsCount";
		private const string spGET_TOKENS_USERS_TOTALS = "GetTokenUserTotals";
		private const string spGET_TOKEN_TOTALS_BY_TYPE = "GetTokenTotalsByTypePAG";
		private const string spGET_VALIDATIONS_COUNT_BY_USER_PAG = "GetTokenValidationsCountByUserPAG";
		private const string spGET_TOKENS_TOTALS = "GetTokenTotals";
		private const string spGET_TOKENS_BY_USER_AND_STATUS_PAG = "GetTokensByUserAndStatusPAG";
		private const string spGET_TOKENS_BY_USER_AND_STATUS = "GetTokensByUserAndStatus";
		private const string spGET_TOKENS_BY_USER_AND_TYPE_PAG = "GetTokensByUserAndTypePAG";
		private const string spGET_TOKENS_BY_USER_AND_TYPE = "GetTokensByUserAndType";
		private const string spGET_TOKEN_CONTACTS_BY_USER_AND_TOKENID = "GetTokenContactsByUserAndTokenID";
		private const string spGET_TOKEN_BY_USER_BY_SUPPLIERSN_AND_TYPE = "GetTokenByUserBySupplierSNAndParamID";
		private const string spGET_TOKEN_BY_SUPPLIERSN_AND_TYPE = "GetTokensBySupplierSNAndType";
		private const string spGET_TOKENS_BY_USER_AND_SERIAL = "GetTokensByUserAndSupplierSN";
		private const string spGET_TOKENS_BY_USER_AND_TYPE_AND_STATUS = "GetTokensByUserAndTypeAndStatus";
		private const string spGET_TOKENID_BY_VENDORID_AND_SERIAL = "GetTokensByVendorIDAndIntSerial";
		private const string spUPDATE_TOKEN_VALIDATION_ATTEMPTS = "TokenUpdateValidationAttempts";
		private const string spGET_ALL_TOKENS_BY_STATUS_AND_BETWEEN_DATES_PAG = "TokensGetByStatusAndBetweenDatesPAG";
		private const string spGET_ALL_TOKENS_CREATED_AND_BETWEEN_DATES_PAG = "TokensGetCreatedAndBetweenDatesPAG";
		private const string spGET_TOKEN_INFO = "GetTokenInfo";
		private const string spINSERT_TOKENS_USER = "TokenUserInsert";
		private const string spCHANGE_TOKEN_STATUS = "TokenChangeStatus";
		private const string spGET_TOKEN_USER_STATUS = "GetTokenUserStatus";
		private const string spGET_TOKEN_USER_VENDOR = "GetTokenUserVendor";
		private const string spTOKEN_EVENTS_INSERT = "TokenEventsInsert";
		private const string spTOKEN_VENDORS_LOAD_TABLE = "TokenVendorsLoadTable";
		private const string spGET_TANCODE_LOT_MATRIX = "TokensTanGetCodeLotMatrix";
		private string _prepareUSADateFormat(string inputDate, DateTime defaultDate)
		{
			string result;
			try
			{
				DateTime _datetime;
				DateTime.TryParse(inputDate, out _datetime);
				_datetime = ((_datetime == DateTime.MinValue) ? defaultDate : _datetime);
				result = string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]
				{
					_datetime.Month.ToString(),
					_datetime.Day.ToString(),
					_datetime.Year.ToString(),
					_datetime.Hour.ToString(),
					_datetime.Minute.ToString(),
					_datetime.Second.ToString()
				});
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					ex.ToString()
				});
				result = inputDate;
			}
			return result;
		}
		private void _PrepPagination(IDbCommand _cmd, int iOffset, int pageSize, int pageDirection)
		{
			_cmd.Parameters.Add(base.AddParameter("@offset", iOffset));
			_cmd.Parameters.Add(base.AddParameter("@maximumRows", pageSize));
			_cmd.Parameters.Add(base.AddParameter("@forw", pageDirection));
			_cmd.Parameters.Add(base.AddParameterOutputInt32("@totRows"));
		}
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
		private TokenInfo[] _loadTokensDataFull(IDbCommand cmd)
		{
			int dummy;
			return this._loadTokensDataPAG(cmd, out dummy);
		}
		private TokenInfo[] _loadTokensDataPAG(IDbCommand cmd, out int totRows)
		{
			totRows = -1;
			IDataReader _dr = null;
			List<TokenInfo> _tkInf = new List<TokenInfo>();
			TokenInfo[] result;
			try
			{
				base.Connection.Open();
				_dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkInf.Add(TokenInfo.loadTokenInfo((TokenStatus)((byte)_dr[2]), (byte)_dr[7], (int)_dr[0], (_dr[5] == DBNull.Value) ? "" : ((string)_dr[5]), (string)_dr[1], (_dr[3] == DBNull.Value) ? "" : ((string)_dr[3]), (_dr[6] == DBNull.Value) ? "" : ((string)_dr[6]), (DateTime)_dr[4], (_dr[8] == DBNull.Value) ? "" : _dr.GetString(8), (DateTime)_dr[11], new TokenInfoCore()));
				}
				_dr.Close();
				this.GetTotRows(cmd, out totRows);
				result = _tkInf.ToArray();
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
			}
			return result;
		}
		private TokenValidationCountByUser[] _loadTokensValidationsCountByUser(IDbCommand cmd, out int totRows)
		{
			totRows = -1;
			IDataReader _dr = null;
			List<TokenValidationCountByUser> _tkInf = new List<TokenValidationCountByUser>();
			TokenValidationCountByUser[] result;
			try
			{
				base.Connection.Open();
				_dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkInf.Add(TokenValidationCountByUser.loadValidationCountByUser((string)_dr[0], (string)_dr[2], (int)_dr[3], (int)_dr[4], (int)_dr[5]));
				}
				_dr.Close();
				this.GetTotRows(cmd, out totRows);
				result = _tkInf.ToArray();
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
			}
			return result;
		}
		private TokenTotalsByType[] _loadTokenTotalsByType(IDbCommand cmd, out int totRows)
		{
			totRows = -1;
			IDataReader _dr = null;
			List<TokenTotalsByType> _tkInf = new List<TokenTotalsByType>();
			TokenTotalsByType[] result;
			try
			{
				base.Connection.Open();
				_dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkInf.Add(TokenTotalsByType.tokenTotalsByType((DateTime)_dr[0], (int)_dr[1], (int)_dr[2], (int)_dr[3], (int)_dr[4]));
				}
				_dr.Close();
				this.GetTotRows(cmd, out totRows);
				result = _tkInf.ToArray();
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
			}
			return result;
		}
		private OperationResult _updateTokenStatus(TokenStatus tokenStatus, string applicationUser, string tokenID, out long tokenEventID)
		{
			tokenEventID = -1L;
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenChangeStatus", CommandType.StoredProcedure);
				IList arg_3C_0 = _cmd.Parameters;
				string arg_37_1 = "@param1";
				int num = (int)tokenStatus;
				arg_3C_0.Add(base.AddParameter(arg_37_1, num.ToString()));
				_cmd.Parameters.Add(base.AddParameter("@param2", applicationUser));
				_cmd.Parameters.Add(base.AddParameter("@param3", tokenID));
				base.Connection.Open();
				base.TransactionBegin(IsolationLevel.ReadCommitted);
				_cmd.Transaction = base.Transaction;
				if (1 == _cmd.ExecuteNonQuery())
				{
					if (OperationResult.Success == this._insertTokenEvent(tokenID, (int)tokenStatus, 0, applicationUser, out tokenEventID))
					{
						base.TransactionCommit();
						result = OperationResult.Success;
						return result;
					}
				}
				result = OperationResult.Error;
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
		private TokenInfo[] loadTokensUserByStatus(string tokenUserApplicationID, TokenStatus tokenStatus, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByUserAndStatusPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				IList arg_53_0 = _cmd.Parameters;
				string arg_4E_1 = "@param2";
				int num = (int)tokenStatus;
				arg_53_0.Add(base.AddParameter(arg_4E_1, num.ToString()));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		private OperationResult _insertTokenEvent(string tokenID, int eventOperation, int eventOperationStatus, string applicationUser, out long tokenEventID)
		{
			tokenEventID = -1L;
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				_cmd = base.CreateCommand("TokenEventsInsert", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param1", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@param2", eventOperation));
				_cmd.Parameters.Add(base.AddParameter("@param3", (byte)eventOperationStatus));
				_cmd.Parameters.Add(base.AddParameter("@param4", applicationUser));
				_cmd.Transaction = base.Transaction;
				tokenEventID = long.Parse(_cmd.ExecuteScalar().ToString());
				if (eventOperation == 102)
				{
					_cmd = base.CreateCommand("TokenUpdateValidationAttempts", CommandType.StoredProcedure);
					_cmd.Parameters.Add(base.AddParameter("@param1", tokenID));
					_cmd.Parameters.Add(base.AddParameter("@param2", (eventOperationStatus == 0 || eventOperationStatus == 200) ? 0 : 1));
				}
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
			}
			return result;
		}
		private OperationResult getTokenIDAndTokenIntSerial(int tokenParamsID, string tokenUserApplicationID, TokenStatus tokenStatus, out string tokenID, out string internalSerialNumber)
		{
			tokenID = null;
			internalSerialNumber = null;
			IDataReader _dr = null;
			IDbCommand _cmd = null;
			int _tRecords = 0;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByUserAndTypeAndStatus", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				IList arg_79_0 = _cmd.Parameters;
				string arg_74_1 = "@Param3";
				int num = (int)tokenStatus;
				arg_79_0.Add(base.AddParameter(arg_74_1, num.ToString()));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tRecords++;
					tokenID = _dr[0].ToString();
					internalSerialNumber = _dr[8].ToString();
				}
				if (_tRecords == 1)
				{
					result = OperationResult.Success;
				}
				else
				{
					tokenID = null;
					internalSerialNumber = null;
					result = OperationResult.Error;
				}
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
		private static DataTable BindToDataTable(Type enumType)
		{
			DataTable result;
			if (enumType == null || !enumType.IsEnum)
			{
				result = null;
			}
			else
			{
				try
				{
					DataTable _dTable = new DataTable(enumType.Name);
					_dTable.Columns.Add(new DataColumn("iKey", Type.GetType("System.Int32")));
					_dTable.Columns.Add(new DataColumn("sValue", Type.GetType("System.String")));
					foreach (object enumItem in Enum.GetValues(enumType))
					{
						_dTable.Rows.Add(new object[]
						{
							(int)enumItem,
							enumItem.ToString().Replace("_", " ")
						});
					}
					result = _dTable;
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
			}
			return result;
		}
		public static Hashtable BindToEnum(Type enumType)
		{
			Hashtable result;
			try
			{
				Hashtable _hashtbl = new Hashtable();
				foreach (object enumItem in Enum.GetValues(enumType))
				{
					_hashtbl.Add((int)enumItem, enumItem.ToString().Replace("_", " "));
				}
				result = _hashtbl;
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
		public TokensVendorList[] loadTokenVendorTable()
		{
			IDataReader _dr = null;
			IDbCommand _cmd = null;
			List<TokensVendorList> _tkVendorList = new List<TokensVendorList>();
			TokensVendorList[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenVendorsLoadTable", CommandType.StoredProcedure);
				base.Connection.Open();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkVendorList.Add(TokensVendorList.LoadTokensVendorList(_dr[0].ToString(), _dr[1].ToString(), _dr[2].ToString(), (bool)_dr[3]));
				}
				result = _tkVendorList.ToArray();
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
		public TokenInfo[] loadActiveTokensByUser(string tokenUserApplicationID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			return this.loadTokensUserByStatus(tokenUserApplicationID, TokenStatus.Enabled, firstItem, pageSize, pageDirection, out totRows);
		}
		public TokenInfo[] loadCanceledTokensByUser(string tokenUserApplicationID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			return this.loadTokensUserByStatus(tokenUserApplicationID, TokenStatus.Canceled, firstItem, pageSize, pageDirection, out totRows);
		}
		public TokenInfo[] loadDisabledTokensByUser(string tokenUserApplicationID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			return this.loadTokensUserByStatus(tokenUserApplicationID, TokenStatus.Disabled, firstItem, pageSize, pageDirection, out totRows);
		}
		public TokenInfo[] loadAllTokensUserFull(string tokenUserApplicationID)
		{
			IDbCommand _cmd = null;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensUserAll", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				result = this._loadTokensDataFull(_cmd);
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
		public TokenInfo[] loadAllTokensUserPAG(string tokenUserApplicationID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensUserAllPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public TokenInfo[] loadTokensByPhoneNumber(TokenStatus tkStatus, string[] applicationUserPhoneNumber, out int totRows)
		{
			totRows = 0;
			IDbCommand _cmd = null;
			string _sBuilder = null;
			TokenInfo[] result;
			try
			{
				for (int i = 0; i < applicationUserPhoneNumber.Length; i++)
				{
					string text = _sBuilder;
					_sBuilder = string.Concat(new string[]
					{
						text,
						(_sBuilder != null) ? "," : "",
						"'",
						applicationUserPhoneNumber[i],
						"'"
					});
				}
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensUserByPhone", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", (int)tkStatus));
				_cmd.Parameters.Add(base.AddParameter("@Param2", _sBuilder));
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public TokenInfo[] loadTokensByPhoneNumber(string applicationUserPhoneNumber, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			IDbCommand _cmd = null;
			totRows = -1;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensUserByPhonePAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", applicationUserPhoneNumber));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public TokenInfo[] loadAllTokensByStatusAndBetweenDates(TokenStatus tkStatus, string initialDate, string endDate, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			IDbCommand _cmd = null;
			totRows = -1;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokensGetByStatusAndBetweenDatesPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", (int)tkStatus));
				_cmd.Parameters.Add(base.AddParameter("@Param2", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@Param3", this._prepareUSADateFormat(endDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59"))));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public TokenInfo[] loadAllTokensCreatedBetweenDatesPAG(string initialDate, string endDate, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			IDbCommand _cmd = null;
			totRows = -1;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokensGetCreatedAndBetweenDatesPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param2", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@Param3", this._prepareUSADateFormat(endDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 23:59:59"))));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public string getTokenVendor(string tokenID, string tokenUserApplicationID)
		{
			IDbCommand _cmd = null;
			string result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenUserVendor", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenUserApplicationID));
				base.Connection.Open();
				result = _cmd.ExecuteScalar().ToString();
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
		public OperationResult getUserActiveTokenByVendorID(int tokenParamsID, string tokenUserApplicationID, out string tokenID, out string internalSerialNumber)
		{
			return this.getTokenIDAndTokenIntSerial(tokenParamsID, tokenUserApplicationID, TokenStatus.Enabled, out tokenID, out internalSerialNumber);
		}
		public OperationResult getTokenIDByVendorIDAndIntSerial(int tokenParamsID, string internalSerialNumber, out string tokenUserApplicationID, out string tokenID)
		{
			tokenID = null;
			tokenUserApplicationID = null;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			int _tRecords = 0;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByVendorIDAndIntSerial", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenParamsID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", internalSerialNumber));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tRecords++;
					tokenID = _dr[0].ToString();
					tokenUserApplicationID = _dr[1].ToString();
				}
				if (_tRecords == 1)
				{
					result = OperationResult.Success;
				}
				else
				{
					tokenID = null;
					tokenUserApplicationID = null;
					result = OperationResult.Error;
				}
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
		public OperationResult getTokenByUserBySupplierSNAndParamID(string tokenUserApplicationID, string SupplierSerialNumber, int tokenParamsID, out TokenInfo tokenInfo)
		{
			tokenInfo = new TokenInfo();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				IDbCommand _cmd = base.CreateCommand("GetTokenByUserBySupplierSNAndParamID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", SupplierSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@Param3", tokenParamsID));
				base.Connection.Open();
				IDataReader _dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (!_dr.Read())
				{
					result = OperationResult.Error;
				}
				else
				{
					tokenInfo = TokenInfo.loadTokenInfo((TokenStatus)((byte)_dr[2]), (byte)_dr[7], (int)_dr[0], (string)_dr[5], (string)_dr[1], (_dr[3] == DBNull.Value) ? "" : ((string)_dr[3]), (_dr[6] == DBNull.Value) ? "" : ((string)_dr[6]), (DateTime)_dr[4], (string)_dr[8], (DateTime)_dr[11], new TokenInfoCore());
					result = OperationResult.Success;
				}
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
			return result;
		}
		public OperationResult getTokenIDByUserAndSupplierSN(string tokenUserApplicationID, string SupplierSerialNumber, out int tokenParamsID, out string tokenID)
		{
			tokenID = null;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			tokenParamsID = -1;
			int _tRecords = 0;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByUserAndSupplierSN", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", SupplierSerialNumber));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tRecords++;
					tokenID = _dr[0].ToString();
					tokenParamsID = (int)((byte)_dr[7]);
				}
				if (_tRecords == 1)
				{
					result = OperationResult.Success;
				}
				else
				{
					tokenID = null;
					tokenParamsID = -1;
					result = OperationResult.Error;
				}
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
		public OperationResult getTokenIDBySupplierSNAndType(string SupplierSerialNumber, int tokenParamsID, out string tokenUserApplicationID, out string tokenID)
		{
			int _tRecords = 0;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			tokenID = null;
			tokenUserApplicationID = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensBySupplierSNAndType", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", SupplierSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tRecords++;
					tokenID = _dr[0].ToString();
					tokenUserApplicationID = _dr[1].ToString();
				}
				if (_tRecords == 1)
				{
					result = OperationResult.Success;
				}
				else
				{
					tokenID = null;
					tokenUserApplicationID = null;
					result = OperationResult.Error;
				}
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
		public TokenInfo getTokenInfo(string tokenID, TokenInfoCore tokenInfoCore)
		{
			IDataReader _dr = null;
			IDbCommand _cmd = null;
			TokenInfo result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenInfo", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenID));
				base.OpenConnection();
				(_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection)).Read();
				result = TokenInfo.loadTokenInfo((TokenStatus)((byte)_dr[2]), (byte)_dr[7], (int)_dr[0], (_dr[5] == DBNull.Value) ? "" : ((string)_dr[5]), (string)_dr[1], (_dr[3] == DBNull.Value) ? "" : ((string)_dr[3]), (_dr[6] == DBNull.Value) ? "" : ((string)_dr[6]), (DateTime)_dr[4], (string)_dr[8], (DateTime)_dr[11], tokenInfoCore);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = new TokenInfo();
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
		public OperationResult getTokenStatus(string tokenID, string tokenUserApplicationID, out TokenStatus tokenStatus)
		{
			tokenStatus = TokenStatus.Undefined;
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenUserStatus", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenUserApplicationID));
				base.Connection.Open();
				object _retVal = _cmd.ExecuteScalar();
				if (_retVal == null)
				{
					tokenStatus = TokenStatus.Undefined;
					result = OperationResult.Success;
				}
				else
				{
					tokenStatus = (TokenStatus)((byte)_retVal);
					result = OperationResult.Success;
				}
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
		public TokenInfo[] loadTokenUserByType(string tokenUserApplicationID, string tokenParamsID)
		{
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			List<TokenInfo> _tkInf = new List<TokenInfo>();
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByUserAndType", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				base.Connection.Open();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkInf.Add(TokenInfo.loadTokenInfo((TokenStatus)((byte)_dr[2]), (byte)_dr[7], (int)_dr[0], (_dr[5] == DBNull.Value) ? null : ((string)_dr[5]), (string)_dr[1], (_dr[3] == DBNull.Value) ? "" : ((string)_dr[3]), (_dr[6] == DBNull.Value) ? "" : ((string)_dr[6]), (DateTime)_dr[4], (string)_dr[8], (DateTime)_dr[11], new TokenInfoCore()));
				}
				result = _tkInf.ToArray();
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
		public TokenInfo[] loadTokenUserByType(string tokenUserApplicationID, string tokenParamsID, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			TokenInfo[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokensByUserAndTypePAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensDataPAG(_cmd, out totRows);
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
		public OperationResult loadTokenContacts(string tokenUserApplicationID, string tokenInternalID, out string phoneNumber, out string emailAddress)
		{
			int _tRecords = 0;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			phoneNumber = null;
			emailAddress = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenContactsByUserAndTokenID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param1", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenInternalID));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tRecords++;
					phoneNumber = _dr[0].ToString();
					emailAddress = _dr[1].ToString();
				}
				if (_tRecords == 1)
				{
					result = OperationResult.Success;
				}
				else
				{
					phoneNumber = null;
					emailAddress = null;
					result = OperationResult.Error;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				phoneNumber = null;
				emailAddress = null;
				result = OperationResult.Error;
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
		public OperationResult tokenAllowedUse(string applicationUser, string tokenID, out long tokenEventID)
		{
			return this._updateTokenStatus(TokenStatus.Enabled, applicationUser, tokenID, out tokenEventID);
		}
		public OperationResult tokenInhibitedUse(string applicationUser, string tokenID, out long tokenEventID)
		{
			return this._updateTokenStatus(TokenStatus.Disabled, applicationUser, tokenID, out tokenEventID);
		}
		public OperationResult tokenCancel(string applicationUser, string tokenID, out long tokenEventID)
		{
			return this._updateTokenStatus(TokenStatus.Canceled, applicationUser, tokenID, out tokenEventID);
		}
		public OperationResult insertTokenUser(TokenInfoCore tokenInfoCore, string applicationUser, string applicationPhoneNumber, string applicationEmail, out TokenInfo tokenInfo, out long tokenEventID)
		{
			tokenEventID = 0L;
			IDbCommand _cmd = null;
			tokenInfo = new TokenInfo();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokenUserInsert", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param1", tokenInfoCore.InternalID));
				_cmd.Parameters.Add(base.AddParameter("@param2", tokenInfoCore.TypeID));
				_cmd.Parameters.Add(base.AddParameter("@param3", tokenInfoCore.InternalStatus));
				_cmd.Parameters.Add(base.AddParameter("@param4", applicationUser));
				_cmd.Parameters.Add(base.AddParameter("@param5", applicationPhoneNumber));
				_cmd.Parameters.Add(base.AddParameter("@param6", applicationEmail));
				_cmd.Parameters.Add(base.AddParameter("@param7", tokenInfoCore.InternalSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@param8", tokenInfoCore.SupplierSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@param9", tokenInfoCore.SupplierLotID));
				base.Connection.Open();
				base.TransactionBegin(IsolationLevel.ReadCommitted);
				_cmd.Transaction = base.Transaction;
				if (1 == _cmd.ExecuteNonQuery())
				{
					if (OperationResult.Success == this._insertTokenEvent(tokenInfoCore.InternalID.ToString(), 100, 0, applicationUser, out tokenEventID))
					{
						if (tokenInfoCore.InternalStatus == TokenStatus.ReadyToDeploy)
						{
							if (OperationResult.Success != this._insertTokenEvent(tokenInfoCore.InternalID.ToString(), 98, 0, applicationUser, out tokenEventID))
							{
								result = OperationResult.Error;
								return result;
							}
						}
						base.TransactionCommit();
						tokenInfo = this.getTokenInfo(tokenInfoCore.InternalID.ToString(), tokenInfoCore);
						result = OperationResult.Success;
						return result;
					}
				}
				result = OperationResult.Error;
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
		public TokenValidationsCount[] loadTokenValidationsCount(int tokenType, string initialDate, string finalDate)
		{
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			List<TokenValidationsCount> _tkValidations = new List<TokenValidationsCount>();
			TokenValidationsCount[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenValidationsCount", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@TokenType", tokenType));
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", this._prepareUSADateFormat(finalDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkValidations.Add(TokenValidationsCount.loadTokensValidationsCount((int)_dr[0], (int)_dr[3], (int)_dr[2]));
				}
				result = _tkValidations.ToArray();
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
		public TokenUserTotals loadTokenUserTotals(string initialDate, string finalDate)
		{
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			TokenUserTotals _tkUserTotals = new TokenUserTotals();
			TokenUserTotals result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenUserTotals", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", this._prepareUSADateFormat(finalDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkUserTotals = TokenUserTotals.loadTokenUserTotals((int)_dr[0], (int)_dr[1], (int)_dr[2], (int)_dr[3], (int)_dr[4], (int)_dr[5], (int)_dr[6], (int)_dr[7]);
				}
				result = _tkUserTotals;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.TokenBusinessDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_tkUserTotals.TotalUsers = -1;
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
		public TokenTotalsByType[] loadTokenTotalsByType(string initialDate, string finalDate, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			TokenTotalsByType[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenTotalsByTypePAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", this._prepareUSADateFormat(finalDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokenTotalsByType(_cmd, out totRows);
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
		public TokenValidationCountByUser[] loadTokenValidationsCountByUserPAG(string applicationUser, int tokenType, string initialDate, string finalDate, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			TokenValidationCountByUser[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenValidationsCountByUserPAG", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@ApplicationUser", applicationUser));
				_cmd.Parameters.Add(base.AddParameter("@TokenType", tokenType));
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", this._prepareUSADateFormat(finalDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				result = this._loadTokensValidationsCountByUser(_cmd, out totRows);
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
		public TokenTotals[] loadTokenTotals(string initialDate, string finalDate)
		{
			IDbCommand _cmd = null;
			IDataReader _dr = null;
			List<TokenTotals> _tkTotals = new List<TokenTotals>();
			TokenTotals[] result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("GetTokenTotals", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@InitialDate", this._prepareUSADateFormat(initialDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				_cmd.Parameters.Add(base.AddParameter("@FinalDate", this._prepareUSADateFormat(finalDate, DateTime.Parse(DateTime.Now.ToShortDateString() + " 00:00:01"))));
				base.OpenConnection();
				_dr = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				while (_dr.Read())
				{
					_tkTotals.Add(TokenTotals.loadTokenTotals((string)_dr[0], (int)_dr[1], (int)_dr[2], (int)_dr[3], (int)_dr[4], (int)_dr[5], (int)_dr[6]));
				}
				result = _tkTotals.ToArray();
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
		public DataTable loadTanCodeLotMatrix(string lotID, string tokenUserApplicationID, string serialNumber, string tokenStatus, string initialCreationDate, string finalCreationDate, string firstItem, int pageSize, int pageDirection, out int totRows)
		{
			totRows = -1;
			IDbCommand _cmd = null;
			DataSet _ds = new DataSet();
			DataTable result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSecureBusiness();
				_cmd = base.CreateCommand("TokensTanGetCodeLotMatrix", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@LotId", lotID));
				_cmd.Parameters.Add(base.AddParameter("@UserId", tokenUserApplicationID));
				_cmd.Parameters.Add(base.AddParameter("@SerialNumber", serialNumber));
				_cmd.Parameters.Add(base.AddParameter("@ApplicationTokenStatus", tokenStatus));
				_cmd.Parameters.Add(base.AddParameter("@InitialCreationDate", initialCreationDate));
				_cmd.Parameters.Add(base.AddParameter("@FinalCreationDate", finalCreationDate));
				this._PrepPagination(_cmd, int.Parse(firstItem), pageSize, pageDirection);
				base.CreateDataAdapter(_cmd).Fill(_ds);
				this.GetTotRows(_cmd, out totRows);
				result = ((_ds.Tables.Count == 1) ? _ds.Tables[0] : null);
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
		public DataTable getEventsOperation()
		{
			return TokenBusinessDAO.BindToDataTable(typeof(TokenEventOperation));
		}
		public DataTable getTokensType()
		{
			return TokenBusinessDAO.BindToDataTable(typeof(TokenDeviceType));
		}
		public DataTable getAutenticationStatus()
		{
			return TokenBusinessDAO.BindToDataTable(typeof(AutenticationStatus));
		}
		public DataTable getTokenStatus()
		{
			return TokenBusinessDAO.BindToDataTable(typeof(TokenStatus));
		}
		public DataTable getOperationResult()
		{
			return TokenBusinessDAO.BindToDataTable(typeof(OperationResult));
		}
	}
}
