using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
using System.Data;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public class TokensDAO : DALSqlServer
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.TokensDAO.softfinanca.com/";
		private const string spFREE_SEED_UPDATE_FOR_NEW_TOKEN = "FreeSeedUpdateForNewToken";
		private const string spGIVEN_SUPPLIER_SERIAL_NUMBER_UPDATE_FOR_NEW_TOKEN = "GivenSupplierSerialNumberUpdateForNewToken";
		private const string spUNDO_UPDATE_FOR_NEW_TOKEN = "UndoUpdateForNewToken";
		private const string spINSERT_NEW_TOKEN = "InsertNewToken";
		private const string spGET_TOKEN_BY_ID = "GetTokenByID";
		private const string spGET_TOKEN_INFO_BY_ID = "GetTokenInfoByID";
		private const string spPERSIST_TOKEN_STATUS = "PersistTokenStatus";
		private const string spPERSIST_TOKEN_MOVING_FACTOR = "PersistTokenMovingFactor";
		private const string spPERSIST_TOKEN_LAST_CHALLENGE_REQUEST = "PersistTokenLastChallengeRequest";
		private const string spTOKEN_GET_STATUS = "TokenGetStatus";
		private const string spTOKEN_BULK_INSERT = "TokenBulkInsert";
		private const string spCREATE_SUPPLIER_SUB_LOT = "CreateSupplierSubLot";
		private const string spCHECK_AVAILABLE_SEEDS_BY_TOKENPARAMSID = "CheckAvailableSeedsByTokenParamsID";
		private const string spGET_TOKENS_SUPPLIER_SERIALNUMBER_BY_SUPPLIERLOT = "GetTokensSupplierSerialnumberBySupplierLot";
		private const string spGET_TOKENS_SUPPLIER_SERIALNUMBER_BY_SUBLOTID = "GetTokensSupplierSerialnumberBySubLotID";
		private const string spTOKEN_GET_BY_LOT = "TokenGetByLot";
		private const string spTOKEN_GET_BY_SUBLOT = "TokenGetBySubLot";
		private const string spTOKEN_SUPPLIER_LOT = "TokenSupplierLot";
		private const string spTOKEN_SUPPLIER_SUBLOT = "TokenSubLot";
		private const string spTOKEN_SUPPLIER_LOT_COUNT_BY_TOKENPARAMSID = "TokenSupplierLotCountByTokenParamsID";
		private const string spTOKEN_SUBLOT = "TokenSubLot";
		private const string spTOKEN_SUBLOT_BY_SUPPLIERLOT = "TokenSubLotBySupplierLot";
		private const string spTOKEN_SEEDS_COUNT_BY_PARAMID = "TokenSeedsCountByParamID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUPPLIERLOTID = "TokenSeedsCountBySupplierLotID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUBLOTID = "TokenSeedsCountBySubLotID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUBLOTID_BY_SUPPLIERLOTID = "TokenSeedsCountBySubLotIDBySupplierLotID";
		private const string spGET_TOKEN_LOT_INFORMATION_BY_VENDOR = "GetTokenLotInformation";
		private const string spPERSIST_TOKEN_SUPPORT_CRYPTODATA = "PersistTokenSupportCryptodata";
		private OperationResult _updateTable(string tokenID, string commandText, object[] fieldValue)
		{
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand(commandText, CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				for (int _idx = 0; _idx < fieldValue.Length; _idx++)
				{
					_cmd.Parameters.Add(base.AddParameter("@Param" + _idx.ToString(), fieldValue[_idx]));
				}
				base.Connection.Open();
				int iRes = _cmd.ExecuteNonQuery();
				result = ((iRes == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult createSubLotByTokenVendor(long numberOfTokensInLot, string tokenParamsID, string subLotID, DateTime expirationDate, out long numberOfSeeds)
		{
			IDbCommand _cmdInfo = null;
			IDbCommand _cmdBuild = null;
			numberOfSeeds = -1L;
			string _lotNumber = BaseFunctions.GenerateSupplierLotNumber(numberOfTokensInLot.ToString(), null);
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmdInfo = base.CreateCommand("CheckAvailableSeedsByTokenParamsID", CommandType.StoredProcedure);
				_cmdInfo.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				base.Connection.Open();
				numberOfSeeds = (long)((int)_cmdInfo.ExecuteScalar());
				if (numberOfSeeds < numberOfTokensInLot)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/createSubLotByTokenVendor",
						OperationResult.TokenVendorSeedNotAvaliable.ToString()
					});
					result = OperationResult.TokenVendorSeedNotAvaliable;
				}
				else
				{
					_cmdBuild = base.CreateCommand("CreateSupplierSubLot", CommandType.StoredProcedure);
					_cmdBuild.Parameters.Add(base.AddParameter("@Param0", _lotNumber));
					_cmdBuild.Parameters.Add(base.AddParameter("@Param1", expirationDate));
					_cmdBuild.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
					_cmdBuild.Parameters.Add(base.AddParameter("@top", (int)numberOfTokensInLot));
					numberOfSeeds = (long)_cmdBuild.ExecuteNonQuery();
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmdInfo != null)
				{
					_cmdInfo.Dispose();
				}
				if (_cmdBuild != null)
				{
					_cmdBuild.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult newTokenFromGivenSupplierSerialNumber(string tokenParamsID, string SupplierSerialNumber, out TokenInfoCore tokenInfoCore)
		{
			IDbCommand _cmd = null;
			tokenInfoCore = new TokenInfoCore();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("GivenSupplierSerialNumberUpdateForNewToken", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", 1));
				_cmd.Parameters.Add(base.AddParameter("@Param1", 99));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				_cmd.Parameters.Add(base.AddParameter("@Param3", SupplierSerialNumber));
				base.Connection.Open();
				object _retID = _cmd.ExecuteScalar();
				if (_retID == DBNull.Value || (int)_retID == 0)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/newTokenFromGivenSupplierSerialNumber",
						OperationResult.TokenVendorSeedNotAvaliable.ToString()
					});
					result = OperationResult.TokenVendorSeedNotAvaliable;
				}
				else
				{
					tokenInfoCore = this.loadTokenInfoCore(_retID.ToString());
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult newTokenFromPreInsertedSeed(string tokenParamsID, out TokenInfoCore tokenInfoCore)
		{
			return this.newTokenFromPreInsertedSeed(tokenParamsID, TokenStatus.Enabled, out tokenInfoCore);
		}
		public OperationResult newTokenFromPreInsertedSeed(string tokenParamsID, TokenStatus newTokenStatus, out TokenInfoCore tokenInfoCore)
		{
			IDbCommand _cmd = null;
			tokenInfoCore = new TokenInfoCore();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("FreeSeedUpdateForNewToken", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", (byte)newTokenStatus));
				_cmd.Parameters.Add(base.AddParameter("@Param1", 99));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				base.Connection.Open();
				object _retID = _cmd.ExecuteScalar();
				if (_retID == DBNull.Value || (int)_retID == 0)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.WARNING, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/newTokenFromPreInsertedSeed",
						OperationResult.TokenVendorSeedNotAvaliable.ToString()
					});
					result = OperationResult.TokenVendorSeedNotAvaliable;
				}
				else
				{
					tokenInfoCore = this.loadTokenInfoCore(_retID.ToString());
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult undoUpdateForNewToken(string tokenID)
		{
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("UndoUpdateForNewToken", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", 99));
				_cmd.Parameters.Add(base.AddParameter("@Param1", 1));
				_cmd.Parameters.Add(base.AddParameter("@Param2", tokenID));
				_cmd.Parameters.Add(base.AddParameter("@Param3", tokenID));
				base.Connection.Open();
				int iRes = _cmd.ExecuteNonQuery();
				result = ((iRes == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult updateCryptoData(string tokenID, string cryptoData)
		{
			return this._updateTable(tokenID, "PersistTokenSupportCryptodata", new object[]
			{
				cryptoData
			});
		}
		public OperationResult updateMovingFactor(string tokenID, long movingFactor)
		{
			return this._updateTable(tokenID, "PersistTokenMovingFactor", new object[]
			{
				movingFactor
			});
		}
		public OperationResult updateMovingFactor(TokenCryptoData tkCryptoData)
		{
			return this._updateTable(tkCryptoData.ID, "PersistTokenMovingFactor", new object[]
			{
				tkCryptoData.CryptoData.MovingFactor
			});
		}
		public OperationResult updateTokenStatus(string tokenID, TokenStatus tokenStatus)
		{
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("PersistTokenStatus", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", (byte)tokenStatus));
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				int iRes = _cmd.ExecuteNonQuery();
				result = ((iRes == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public TokenInfoCore loadTokenInfoCore(string tokenID)
		{
			IDataReader _rd = null;
			IDbCommand _cmd = null;
			TokenInfoCore result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("GetTokenInfoByID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.OpenConnection();
				_rd = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				_rd.Read();
                result = TokenInfoCore.loadTokenInfoCore((byte)_rd[0], (int)_rd[1], (_rd[2] != DBNull.Value) ? ((string)_rd[2]) : "", (_rd[3] != DBNull.Value) ? ((string)_rd[3]) : "", (string)_rd[4], (_rd[5] != DBNull.Value) ? ((DateTime)_rd[5]) : DateTime.MinValue, (_rd[7] == DBNull.Value) ? null : _rd[7].ToString(), (TokenStatus)((byte)_rd[6]));
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = new TokenInfoCore();
			}
			finally
			{
				if (_rd != null)
				{
					_rd.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public TokenCryptoData loadTokenCryptoData(string tokenID)
		{
			IDataReader _rd = null;
			IDbCommand _cmd = null;
			TokenCryptoData result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("GetTokenByID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				_rd = _cmd.ExecuteReader(CommandBehavior.CloseConnection);
				_rd.Read();
				result = new TokenCryptoData(_rd[5].ToString(), _rd[0].ToString(), new CryptoData((long)_rd[1], _rd[2].ToString().Trim(), _rd[3].ToString().Trim(), (_rd[6] != DBNull.Value) ? _rd[6].ToString().Trim() : string.Empty), new TokenParamsDAO().loadTokenBaseParams(((byte)_rd[4]).ToString()));
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
			}
			finally
			{
				if (_rd != null)
				{
					_rd.Dispose();
				}
				if (_cmd != null)
				{
					_cmd.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
        public OperationResult createToken(string tokenParamsID, long movingFactor, DateTime expirationDate, string cryptoKey, string supplierSerialNumber, string internalSerialNumber, string creationLotID, string SupportCryptoData, out TokenInfoCore tokenInfoCore)
		{
			string tokenID = null;
			IDbCommand _cmd = null;
			OperationResult result;
            tokenInfoCore = new TokenInfoCore();

            try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("InsertNewToken", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkStatus", 0));
				_cmd.Parameters.Add(base.AddParameter("@tkParamsID", tokenParamsID));
				_cmd.Parameters.Add(base.AddParameter("@tkMovingFactor", movingFactor));
				_cmd.Parameters.Add(base.AddParameter("@tkExpirationDate", expirationDate));
				_cmd.Parameters.Add(base.AddParameter("@tkCryptoKey", cryptoKey));
				_cmd.Parameters.Add(base.AddParameter("@tkCreationLotID", creationLotID));
				_cmd.Parameters.Add(base.AddParameter("@tkSupplierSerialNumber", supplierSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@tkInternalSerialNumber", internalSerialNumber));
				_cmd.Parameters.Add(base.AddParameter("@tkSupportCriptoData", SupportCryptoData));
				_cmd.Parameters.Add(base.AddParameter("@tokenSubLotID", null));
				base.Connection.Open();
				long _hResult = long.Parse(_cmd.ExecuteScalar().ToString());
				tokenID = _hResult.ToString();
                tokenInfoCore = this.loadTokenInfoCore(tokenID);
                result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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

        public OperationResult createToken(string tokenParamsID, long movingFactor, DateTime expirationDate, string cryptoKey, string supplierSerialNumber, string internalSerialNumber, string creationLotID, string SupportCryptoData, out string tokenID)
        {
            tokenID = null;
            IDbCommand _cmd = null;
            OperationResult result;
            try
            {
                base.ConnectionString = DBConnectionString.ExpandSAFCore();
                _cmd = base.CreateCommand("InsertNewToken", CommandType.StoredProcedure);
                _cmd.Parameters.Add(base.AddParameter("@tkStatus", 0));
                _cmd.Parameters.Add(base.AddParameter("@tkParamsID", tokenParamsID));
                _cmd.Parameters.Add(base.AddParameter("@tkMovingFactor", movingFactor));
                _cmd.Parameters.Add(base.AddParameter("@tkExpirationDate", expirationDate));
                _cmd.Parameters.Add(base.AddParameter("@tkCryptoKey", cryptoKey));
                _cmd.Parameters.Add(base.AddParameter("@tkCreationLotID", creationLotID));
                _cmd.Parameters.Add(base.AddParameter("@tkSupplierSerialNumber", supplierSerialNumber));
                _cmd.Parameters.Add(base.AddParameter("@tkInternalSerialNumber", internalSerialNumber));
                _cmd.Parameters.Add(base.AddParameter("@tkSupportCriptoData", SupportCryptoData));
                _cmd.Parameters.Add(base.AddParameter("@tokenSubLotID", null));
                base.Connection.Open();
                long _hResult = long.Parse(_cmd.ExecuteScalar().ToString());
                if (_hResult == 1L)
                {
                    tokenID = _hResult.ToString();
                    result = OperationResult.Success;
                }
                else
                {
                    result = OperationResult.Error;
                }
            }
            catch (Exception ex)
            {
                SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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

		public OperationResult tokenStatus(string tokenID, out TokenStatus tokenStatus)
		{
			tokenStatus = TokenStatus.Undefined;
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenGetStatus", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				tokenStatus = (TokenStatus)((byte)_cmd.ExecuteScalar());
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public long tokensSeedsBulkInsert(string pathFileName, TokenStatus tokenStatus, DateTime tokenExpiration)
		{
			IDbCommand _cmd = null;
			long result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenBulkInsert", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@PathFileName", pathFileName));
				_cmd.Parameters.Add(base.AddParameter("@tkStatus", (int)tokenStatus));
				_cmd.Parameters.Add(base.AddParameter("@tkExpirationDate", tokenExpiration));
				base.Connection.Open();
				result = (long)_cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
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
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadSubLots(out DataTable dataTable)
		{
			return this.loadSubLots(null, null, out dataTable);
		}
		public OperationResult loadSubLots(string creationLotID, string tokenParamsID, out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSubLot", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@CreationLotID", creationLotID));
				_cmd.Parameters.Add(base.AddParameter("@TokenParamsID", tokenParamsID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadSupplierLots(out DataTable dataTable)
		{
			return this.loadSupplierLots(null, out dataTable);
		}
		public OperationResult loadSupplierLots(string tokenParamsID, out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSupplierLotCountByTokenParamsID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult seedsStatusByParamsID(out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSeedsCountByParamID", CommandType.StoredProcedure);
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult seedsStatusBySupplierLotID(out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSeedsCountBySubLotID", CommandType.StoredProcedure);
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult seedsStatusBySublotID(out DataTable dataTable)
		{
			return this.seedsStatusSublotBySupplierLotID(null, out dataTable);
		}
		public OperationResult seedsStatusSublotBySupplierLotID(string supplierLotID, out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSeedsCountBySubLotIDBySupplierLotID", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", supplierLotID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult tokenSeedsByParamIDWithNoSubLot(string tokenParamsID, out long numberOfSeeds)
		{
			numberOfSeeds = -1L;
			IDbCommand _cmdInfo = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmdInfo = base.CreateCommand("CheckAvailableSeedsByTokenParamsID", CommandType.StoredProcedure);
				_cmdInfo.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				base.Connection.Open();
				numberOfSeeds = Convert.ToInt64(_cmdInfo.ExecuteScalar());
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				if (_cmdInfo != null)
				{
					_cmdInfo.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult tokenSupplierSerialNumbersByLot(LoteType loteType, string loteId, out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				string cmdTxt = (loteType == LoteType.SupplierLot) ? "GetTokensSupplierSerialnumberBySupplierLot" : "GetTokensSupplierSerialnumberBySubLotID";
				_cmd = base.CreateCommand(cmdTxt, CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", loteId));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadTableWithTokensLot(LoteType loteType, string loteId, string TokenVendorID, TokenMovingFactorType tokenMovingFactorType, out DataTable dataTable)
		{
			DataSet _ds = new DataSet();
			IDbCommand _cmd = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand((loteType == LoteType.SupplierLot) ? "TokenGetByLot" : "TokenGetBySubLot", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@Param0", (byte)tokenMovingFactorType));
				_cmd.Parameters.Add(base.AddParameter("@Param1", TokenVendorID));
				_cmd.Parameters.Add(base.AddParameter("@Param2", loteId));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
				if (_ds != null)
				{
					_ds.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadTokenLotInformation(int tokenVendorID, string lotID, out DataTable dataTable)
		{
			IDbCommand _cmd = null;
			DataSet _ds = new DataSet();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("GetTokenLotInformation", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", tokenVendorID));
				_cmd.Parameters.Add(base.AddParameter("@param1", lotID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult loadTokenLotBySupplier(int tokenVendorID, out DataTable dataTable)
		{
			IDbCommand _cmd = null;
			DataSet _ds = new DataSet();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSupplierLot", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", tokenVendorID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult loadTokenSubLotBySupplier(int tokenVendorID, out DataTable dataTable)
		{
			IDbCommand _cmd = null;
			DataSet _ds = new DataSet();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSubLot", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", tokenVendorID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
		public OperationResult loadTokenSubLotBySupplierLot(string supplierLotID, out DataTable dataTable)
		{
			IDbCommand _cmd = null;
			DataSet _ds = new DataSet();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				_cmd = base.CreateCommand("TokenSubLotBySupplierLot", CommandType.StoredProcedure);
				_cmd.Parameters.Add(base.AddParameter("@param0", supplierLotID));
				base.CreateDataAdapter(_cmd).Fill(_ds);
				dataTable = _ds.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
				{
					"http://sfexpand.SAFCore.TokensDAO.softfinanca.com/",
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
	}
}
