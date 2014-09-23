using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using System;
using System.Data;
namespace SF.Expand.SAF.Core
{
	public class TokensDAO : DALSqlServer
	{
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
		private const string spTOKEN_SUPPLIER_LOT_COUNT_BY_TOKENPARAMSID = "TokenSupplierLotCountByTokenParamsID";
		private const string spTOKEN_SUBLOT = "TokenSubLot";
		private const string spTOKEN_SUBLOT_BY_SUPPLIERLOT = "TokenSubLotBySupplierLot";
		private const string spTOKEN_SEEDS_COUNT_BY_PARAMID = "TokenSeedsCountByParamID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUPPLIERLOTID = "TokenSeedsCountBySupplierLotID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUBLOTID = "TokenSeedsCountBySubLotID";
		private const string spTOKEN_SEEDS_COUNT_BY_SUBLOTID_BY_SUPPLIERLOTID = "TokenSeedsCountBySubLotIDBySupplierLotID";
		private OperationResult _updateTable(string tokenID, string commandText, object[] fieldValue)
		{
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand(commandText, CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				for (int i = 0; i < fieldValue.Length; i++)
				{
					dbCommand.Parameters.Add(base.AddParameter("@Param" + i.ToString(), fieldValue[i]));
				}
				base.Connection.Open();
				int num = dbCommand.ExecuteNonQuery();
				result = ((num == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::_updateTable[" + tokenID + "]", null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult createSubLotByTokenVendor(long numberOfTokensInLot, string tokenParamsID, string subLotID, DateTime expirationDate, out long numberOfSeeds)
		{
			IDbCommand dbCommand = null;
			IDbCommand dbCommand2 = null;
			numberOfSeeds = -1L;
			string value = BaseFunctions.GenerateSupplierLotNumber(numberOfTokensInLot.ToString(), null);
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				base.Connection.Open();
				dbCommand = base.CreateCommand("CheckAvailableSeedsByTokenParamsID", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				numberOfSeeds = (long)((int)dbCommand.ExecuteScalar());
				if (numberOfSeeds < numberOfTokensInLot)
				{
					result = OperationResult.TokenVendorSeedNotAvaliable;
				}
				else
				{
					dbCommand2 = base.CreateCommand("CreateSupplierSubLot", CommandType.StoredProcedure);
					dbCommand2.Parameters.Add(base.AddParameter("@Param0", value));
					dbCommand2.Parameters.Add(base.AddParameter("@Param1", expirationDate));
					dbCommand2.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
					dbCommand2.Parameters.Add(base.AddParameter("@top", (int)numberOfTokensInLot));
					numberOfSeeds = (long)dbCommand2.ExecuteNonQuery();
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::createNewTokenVendorSubLot[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dbCommand2 != null)
				{
					dbCommand2.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult newTokenFromGivenSupplierSerialNumber(string tokenParamsID, string SupplierSerialNumber, out TokenInfoCore tokenInfoCore)
		{
			IDbCommand dbCommand = null;
			tokenInfoCore = new TokenInfoCore();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("GivenSupplierSerialNumberUpdateForNewToken", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", 1));
				dbCommand.Parameters.Add(base.AddParameter("@Param1", 99));
				dbCommand.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				dbCommand.Parameters.Add(base.AddParameter("@Param3", SupplierSerialNumber));
				base.Connection.Open();
				object obj = dbCommand.ExecuteScalar();
				if (obj == DBNull.Value)
				{
					result = OperationResult.TokenVendorSeedNotAvaliable;
				}
				else
				{
					tokenInfoCore = this.loadTokenInfoCore(obj.ToString());
					result = OperationResult.Success;
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::newTokenFromGivvenSupplierSerialNumber[",
					tokenParamsID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult newTokenFromPreInsertedSeed(string tokenParamsID, out TokenInfoCore tokenInfoCore)
		{
			IDbCommand dbCommand = null;
			tokenInfoCore = new TokenInfoCore();
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("FreeSeedUpdateForNewToken", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", 1));
				dbCommand.Parameters.Add(base.AddParameter("@Param1", 99));
				dbCommand.Parameters.Add(base.AddParameter("@Param2", tokenParamsID));
				base.Connection.Open();
				object obj = dbCommand.ExecuteScalar();
				if (obj == DBNull.Value)
				{
					result = OperationResult.Error;
				}
				else
				{
					if (obj.ToString() == "0")
					{
						result = OperationResult.TokenVendorSeedNotAvaliable;
					}
					else
					{
						tokenInfoCore = this.loadTokenInfoCore(obj.ToString());
						result = OperationResult.Success;
					}
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::newTokenFromPreInsertedSeed[",
					tokenParamsID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult undoUpdateForNewToken(string tokenID)
		{
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("UndoUpdateForNewToken", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", 99));
				dbCommand.Parameters.Add(base.AddParameter("@Param1", 1));
				dbCommand.Parameters.Add(base.AddParameter("@Param2", tokenID));
				dbCommand.Parameters.Add(base.AddParameter("@Param3", tokenID));
				base.Connection.Open();
				int num = dbCommand.ExecuteNonQuery();
				result = ((num == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::undoUpdateForNewToken[",
					tokenID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
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
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("PersistTokenStatus", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", (byte)tokenStatus));
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				int num = dbCommand.ExecuteNonQuery();
				result = ((num == 1) ? OperationResult.Success : OperationResult.Error);
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::updateTokenStatus[",
					tokenID,
					";",
					tokenStatus.ToString(),
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public TokenInfoCore loadTokenInfoCore(string tokenID)
		{
			IDataReader dataReader = null;
			IDbCommand dbCommand = null;
			TokenInfoCore result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("GetTokenInfoByID", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.OpenConnection();
				dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
				dataReader.Read();
				result = TokenInfoCore.loadTokenInfoCore((byte)dataReader[0], (int)dataReader[1], (string)dataReader[2], (string)dataReader[3], (string)dataReader[4], (dataReader[5] != DBNull.Value) ? ((DateTime)dataReader[5]) : DateTime.MinValue, (string)dataReader[7], (TokenStatus)((byte)dataReader[6]));
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::loadTokenInfo[",
					tokenID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = new TokenInfoCore();
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
		public TokenCryptoData loadTokenCryptoData(string tokenID)
		{
			IDataReader dataReader = null;
			IDbCommand dbCommand = null;
			TokenCryptoData result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("GetTokenByID", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
				dataReader.Read();
				result = new TokenCryptoData(dataReader[5].ToString(), dataReader[0].ToString(), new CryptoData((long)dataReader[1], dataReader[2].ToString().Trim(), dataReader[3].ToString().Trim(), (dataReader[6] != DBNull.Value) ? dataReader[6].ToString().Trim() : string.Empty), new TokenParamsDAO().loadTokenBaseParams(((byte)dataReader[4]).ToString()));
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::loadTokenCryptoData[",
					tokenID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = new TokenCryptoData(null, null, default(CryptoData), default(TokenTypeBaseParams));
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
		public OperationResult createToken(string tokenParamsID, long movingFactor, DateTime expirationDate, string cryptoKey, string supplierSerialNumber, string internalSerialNumber, string creationLotID, string SupportCryptoData, out string tokenID)
		{
			tokenID = null;
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("InsertNewToken", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkStatus", 0));
				dbCommand.Parameters.Add(base.AddParameter("@tkParamsID", tokenParamsID));
				dbCommand.Parameters.Add(base.AddParameter("@tkMovingFactor", movingFactor));
				dbCommand.Parameters.Add(base.AddParameter("@tkExpirationDate", expirationDate));
				dbCommand.Parameters.Add(base.AddParameter("@tkCryptoKey", cryptoKey));
				dbCommand.Parameters.Add(base.AddParameter("@tkCreationLotID", creationLotID));
				dbCommand.Parameters.Add(base.AddParameter("@tkSupplierSerialNumber", supplierSerialNumber));
				dbCommand.Parameters.Add(base.AddParameter("@tkInternalSerialNumber", internalSerialNumber));
				dbCommand.Parameters.Add(base.AddParameter("@tkSupportCriptoData", SupportCryptoData));
				dbCommand.Parameters.Add(base.AddParameter("@tokenSubLotID", null));
				base.Connection.Open();
				long num = long.Parse(dbCommand.ExecuteScalar().ToString());
				if (num == 1L)
				{
					tokenID = num.ToString();
					result = OperationResult.Success;
				}
				else
				{
					result = OperationResult.Error;
				}
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::createToken[",
					tokenParamsID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult tokenStatus(string tokenID, out TokenStatus tokenStatus)
		{
			tokenStatus = TokenStatus.Undefined;
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("TokenGetStatus", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@tkID", tokenID));
				base.Connection.Open();
				tokenStatus = (TokenStatus)((byte)dbCommand.ExecuteScalar());
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::tokenStatus[",
					tokenID,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public long tokensSeedsBulkInsert(string pathFileName, TokenStatus tokenStatus, DateTime tokenExpiration)
		{
			IDbCommand dbCommand = null;
			long result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("TokenBulkInsert", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@param0", pathFileName));
				dbCommand.Parameters.Add(base.AddParameter("@param1", (int)tokenStatus));
				dbCommand.Parameters.Add(base.AddParameter("@param2", tokenExpiration));
				base.Connection.Open();
				result = (long)dbCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, string.Concat(new string[]
				{
					"SF.Expand.SAF.Core.TokensDAO::tokensSeedsBulkInsert[",
					pathFileName,
					"]",
					Environment.NewLine,
					ex.ToString()
				}), null);
				result = -1L;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadSubLots(out DataTable dataTable)
		{
			return this.loadSubLots(null, out dataTable);
		}
		public OperationResult loadSubLots(string creationLotID, out DataTable dataTable)
		{
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				if (creationLotID != null || creationLotID.Trim().Length > 1)
				{
					dbCommand = base.CreateCommand("TokenSubLotBySupplierLot", CommandType.StoredProcedure);
					dbCommand.Parameters.Add(base.AddParameter("@Param0", creationLotID));
				}
				else
				{
					dbCommand = base.CreateCommand("TokenSubLot", CommandType.StoredProcedure);
				}
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::loadSubLots[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
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
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				if (tokenParamsID != null || tokenParamsID.Trim().Length > 1)
				{
					dbCommand = base.CreateCommand("TokenSupplierLotCountByTokenParamsID", CommandType.StoredProcedure);
					dbCommand.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				}
				else
				{
					dbCommand = base.CreateCommand("TokenSupplierLot", CommandType.StoredProcedure);
				}
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::loadSupplierLots[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult seedsStatusByParamsID(out DataTable dataTable)
		{
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("TokenSeedsCountByParamID", CommandType.StoredProcedure);
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::seedsStatusByParamsID[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult seedsStatusBySupplierlLotID(out DataTable dataTable)
		{
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("TokenSeedsCountBySubLotID", CommandType.StoredProcedure);
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::seedsStatusBySupplierlLotID[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
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
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				if (supplierLotID != null || supplierLotID.Trim().Length > 1)
				{
					dbCommand = base.CreateCommand("TokenSeedsCountBySubLotIDBySupplierLotID", CommandType.StoredProcedure);
					dbCommand.Parameters.Add(base.AddParameter("@Param0", supplierLotID));
				}
				else
				{
					dbCommand = base.CreateCommand("TokenSeedsCountBySupplierLotID", CommandType.StoredProcedure);
				}
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				dataTable = null;
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::seedsStatusSublotBySupplierLotID[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult tokenSeedsByParamIDWithNoSubLot(string tokenParamsID, out long numberOfSeeds)
		{
			numberOfSeeds = -1L;
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand("CheckAvailableSeedsByTokenParamsID", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", tokenParamsID));
				base.Connection.Open();
				numberOfSeeds = (long)dbCommand.ExecuteNonQuery();
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::tokenSeedsByParamIDWithNoSubLot[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult tokenSupplierSerialNumbersByLot(LoteType loteType, string loteId, out DataTable dataTable)
		{
			dataTable = null;
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				string commandText = (loteType == LoteType.SupplierLot) ? "GetTokensSupplierSerialnumberBySupplierLot" : "GetTokensSupplierSerialnumberBySubLotID";
				dbCommand = base.CreateCommand(commandText, CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", loteId));
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::tokenSupplierSerialNumbersByLot[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
		public OperationResult loadTableWithTokensLot(LoteType loteType, string loteId, string TokenVendorID, TokenMovingFactorType tokenMovingFactorType, out DataTable dataTable)
		{
			dataTable = null;
			DataSet dataSet = new DataSet();
			IDbCommand dbCommand = null;
			OperationResult result;
			try
			{
				base.ConnectionString = DBConnectionString.ExpandSAFCore();
				dbCommand = base.CreateCommand((loteType == LoteType.SupplierLot) ? "TokenGetByLot" : "TokenGetBySubLot", CommandType.StoredProcedure);
				dbCommand.Parameters.Add(base.AddParameter("@Param0", (byte)tokenMovingFactorType));
				dbCommand.Parameters.Add(base.AddParameter("@Param1", TokenVendorID));
				dbCommand.Parameters.Add(base.AddParameter("@Param2", loteId));
				base.CreateDataAdapter(dbCommand).Fill(dataSet);
				dataTable = dataSet.Tables[0];
				result = OperationResult.Success;
			}
			catch (Exception ex)
			{
				LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.TokensDAO::loadTableWithTokensLot[]" + Environment.NewLine + ex.ToString(), null);
				result = OperationResult.Error;
			}
			finally
			{
				if (dbCommand != null)
				{
					dbCommand.Dispose();
				}
				if (dataSet != null)
				{
					dataSet.Dispose();
				}
				base.CloseConnection();
			}
			return result;
		}
	}
}
