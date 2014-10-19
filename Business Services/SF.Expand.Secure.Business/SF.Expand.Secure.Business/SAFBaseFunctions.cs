using SF.Expand.LOG;
using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
using System.Data;
using System.Reflection;
using System.Text;
namespace SF.Expand.Secure.Business
{
	public static class SAFBaseFunctions
	{
		private const string cMODULE_NAME = "SAFBUSINESS";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/";
		private static bool _checkStatusConsistency(string tokenID, string applicationUser, out TokenStatus _tokenStatusCore)
		{
			_tokenStatusCore = TokenStatus.Undefined;
			string __mapError = null;
			bool result;
			try
			{
				if (OperationResult.Error == new PREProcessorTokens().CheckStatus(tokenID, out _tokenStatusCore))
				{
					__mapError = string.Concat(new string[]
					{
						"Error loading token info.core::[",
						applicationUser,
						"/",
						tokenID.ToString(),
						" ]"
					});
					SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
						__mapError
					});
					result = false;
				}
				else
				{
					TokenStatus _tokenStatusBusiness;
					if (OperationResult.Error == new TokenBusinessDAO().getTokenStatus(tokenID, applicationUser, out _tokenStatusBusiness))
					{
						__mapError = string.Concat(new string[]
						{
							"Error loading token info.business::[",
							applicationUser,
							"/",
							tokenID.ToString(),
							"]"
						});
						SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
							__mapError
						});
						result = false;
					}
					else
					{
						if (_tokenStatusCore != _tokenStatusBusiness)
						{
							__mapError = string.Concat(new string[]
							{
								"Token inconsistency detected::[",
								applicationUser,
								"/",
								tokenID.ToString(),
								"][core:",
								_tokenStatusCore.ToString(),
								"][business:",
								_tokenStatusBusiness.ToString(),
								"]"
							});
							SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESS", new string[]
							{
								"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
								__mapError
							});
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				__mapError = "Exception :: UserToken ::[" + applicationUser + "]";
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					__mapError,
					ex.ToString()
				});
				_tokenStatusCore = TokenStatus.Undefined;
				result = false;
			}
			finally
			{
				if (null != __mapError)
				{
					SAFInternalEvents.Export(APPEVENTSDeff.OPERATIONS_EXECUTED, 9999, "SAFBUSINESS", new string[]
					{
						"_checkStatusConsistency",
						__mapError
					});
				}
			}
			return result;
		}
		private static AutenticationStatus _tokenPasswordValidation(string applicationUser, string tokenID, string tokenPassword, string dataEntropy, out string newChallenge)
		{
			newChallenge = null;
			AutenticationStatus result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					result = AutenticationStatus.ErrorCheckTokenStatus;
				}
				else
				{
					if (_oldCoreStatus != TokenStatus.Enabled)
					{
						result = AutenticationStatus.TokenNotFoundOrCanceled;
					}
					else
					{
						result = new PREProcessorTokens().Autenticate(tokenID, tokenPassword, dataEntropy, out newChallenge);
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"userToken ::[" + applicationUser + "]",
					ex.ToString()
				});
				result = AutenticationStatus.AutenticationProcessFail;
			}
			return result;
		}
		private static void _logger(SAFLOGGER.LOGGEREventID loggerEventID, string modName, string[] arrayMessages)
		{
			SAFLOGGER.Write(loggerEventID, modName, arrayMessages);
			SAFInternalEvents.Export(APPEVENTSDeff.GENERIC_EVENTS, (int)loggerEventID, "SAFBUSINESS", arrayMessages);
		}
		public static OperationResult tokenGetStatus(string applicationUser, string SupplierSerialNumber, out TokenStatus tokenStatus)
		{
			int tkID = 0;
			int _tokenParamsID = 0;
			string _tokenID = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByUserAndSupplierSN(applicationUser, SupplierSerialNumber, out _tokenParamsID, out _tokenID))
			{
				if (int.TryParse(_tokenID, out tkID))
				{
					result = SAFBaseFunctions.tokenGetStatus(applicationUser, tkID, out tokenStatus);
					return result;
				}
			}
			tokenStatus = TokenStatus.Undefined;
			result = OperationResult.Error;
			return result;
		}
		public static OperationResult tokenGetStatus(int tokenVendorID, string internalSerialNumber, out TokenStatus tokenStatus)
		{
			int tkID = 0;
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				if (int.TryParse(_tokenID, out tkID))
				{
					result = SAFBaseFunctions.tokenGetStatus(_applicationUser, tkID, out tokenStatus);
					return result;
				}
			}
			tokenStatus = TokenStatus.Undefined;
			result = OperationResult.Error;
			return result;
		}
		public static OperationResult tokenGetStatus(string applicationUser, int tokenID, out TokenStatus tokenStatus)
		{
			OperationResult result;
			try
			{
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID.ToString(), applicationUser, out tokenStatus))
				{
					result = OperationResult.WrongStatusForRequestedOperation;
				}
				else
				{
					tokenStatus = TokenStatus.Undefined;
					result = OperationResult.Error;
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				tokenStatus = TokenStatus.Undefined;
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenCancel(string applicationUser, string SupplierSerialNumber)
		{
			int _tokenParamsID = 0;
			string _tokenID = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByUserAndSupplierSN(applicationUser, SupplierSerialNumber, out _tokenParamsID, out _tokenID))
			{
				result = SAFBaseFunctions.tokenCancel(applicationUser, _tokenID, null);
			}
			else
			{
				long tokenEventID;
				new TokensBusinessEventsDAO().insertTokenEvent("0", 9, -1, applicationUser, out tokenEventID);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenCancel(int tokenVendorID, string internalSerialNumber, string baseNotifyMessage)
		{
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenCancel(_applicationUser, _tokenID, baseNotifyMessage);
			}
			else
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenCancel(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			long tokenEventID = 0L;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 9, 83, applicationUser, out tokenEventID);
					result = OperationResult.WrongStatusForRequestedOperation;
				}
				else
				{
					if (_TKRules != null)
					{
						OperationResult _hResult;
						if (OperationResult.Success != (_hResult = _TKRules.BeforeCancel(applicationUser, tokenID, baseNotifyMessage, _oldCoreStatus)))
						{
							new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 9, (int)_hResult, applicationUser, out tokenEventID);
							result = _hResult;
							return result;
						}
					}
					if (OperationResult.Success == new PREProcessorTokens().Cancel(tokenID))
					{
						if (OperationResult.Success == new TokenBusinessDAO().tokenCancel(applicationUser, tokenID, out tokenEventID))
						{
							if (_TKRules != null)
							{
								result = _TKRules.AfterCancel(applicationUser, tokenID, baseNotifyMessage);
								return result;
							}
							result = OperationResult.Success;
							return result;
						}
						else
						{
							if (_oldCoreStatus == TokenStatus.Enabled)
							{
								new PREProcessorTokens().InhibitedUse(tokenID);
							}
							if (_oldCoreStatus == TokenStatus.Disabled)
							{
								new PREProcessorTokens().AllowedUse(tokenID);
							}
						}
					}
					SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						"unable to cancel token::[" + applicationUser + "]"
					});
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 9, -1, applicationUser, out tokenEventID);
					result = OperationResult.Error;
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"unable to cancel token::[" + applicationUser + "]",
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			finally
			{
				SAFInternalEvents.Export(APPEVENTSDeff.OPERATIONS_EXECUTED, 9, "SAFBUSINESS", new string[]
				{
					TokenEventOperation.cTOKEN_CANCEL.ToString(),
					"-1".ToString()
				});
			}
			return result;
		}
		public static OperationResult tokenDisable(string applicationUser, string SupplierSerialNumber)
		{
			int _tokenParamsID = 0;
			string _tokenID = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByUserAndSupplierSN(applicationUser, SupplierSerialNumber, out _tokenParamsID, out _tokenID))
			{
				result = SAFBaseFunctions.tokenDisable(applicationUser, _tokenID, null);
			}
			else
			{
				long tokenEventID;
				new TokensBusinessEventsDAO().insertTokenEvent("0", 0, -1, applicationUser, out tokenEventID);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenDisable(int tokenVendorID, string internalSerialNumber, string baseNotifyMessage)
		{
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenDisable(_applicationUser, _tokenID, baseNotifyMessage);
			}
			else
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenDisable(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 0, 83, applicationUser, out tokenEventID);
					_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
				}
				else
				{
					if (_oldCoreStatus == TokenStatus.Canceled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 0, 83, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
					}
					else
					{
						if (_TKRules != null)
						{
							if (OperationResult.Success != (_hResult = _TKRules.BeforeDisable(applicationUser, tokenID, baseNotifyMessage, _oldCoreStatus)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 0, (int)_hResult, applicationUser, out tokenEventID);
								result = _hResult;
								return result;
							}
						}
						if (OperationResult.Success == (_hResult = new PREProcessorTokens().InhibitedUse(tokenID)))
						{
							if (OperationResult.Success == (_hResult = new TokenBusinessDAO().tokenInhibitedUse(applicationUser, tokenID, out tokenEventID)))
							{
								if (_TKRules != null)
								{
									_hResult = (result = _TKRules.AfterDisable(applicationUser, tokenID, baseNotifyMessage));
									return result;
								}
								_hResult = (result = OperationResult.Success);
								return result;
							}
							else
							{
								if (_oldCoreStatus == TokenStatus.Enabled)
								{
									new PREProcessorTokens().InhibitedUse(tokenID);
								}
								if (_oldCoreStatus == TokenStatus.Disabled)
								{
									new PREProcessorTokens().AllowedUse(tokenID);
								}
							}
						}
						SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
							Assembly.GetExecutingAssembly().FullName.ToString(),
							"unable to disable token::[" + applicationUser + "]"
						});
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 0, (int)_hResult, applicationUser, out tokenEventID);
						result = _hResult;
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"unable to disable token::[" + applicationUser + "]",
					ex.ToString()
				});
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_225_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_225_1 = 0;
				string arg_225_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_DISABLE.ToString();
				string[] arg_222_0 = array;
				int arg_222_1 = 1;
				int num = (int)_hResult;
				arg_222_0[arg_222_1] = num.ToString();
				SAFInternalEvents.Export(arg_225_0, arg_225_1, arg_225_2, array);
			}
			return result;
		}
		public static OperationResult tokenEnable(string applicationUser, string SupplierSerialNumber)
		{
			int _tokenParamsID = 0;
			string _tokenID = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByUserAndSupplierSN(applicationUser, SupplierSerialNumber, out _tokenParamsID, out _tokenID))
			{
				result = SAFBaseFunctions.tokenEnable(applicationUser, _tokenID, null);
			}
			else
			{
				long tokenEventID;
				new TokensBusinessEventsDAO().insertTokenEvent("0", 1, -1, applicationUser, out tokenEventID);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenEnable(int tokenVendorID, string internalSerialNumber, string baseNotifyMessage)
		{
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenEnable(_applicationUser, _tokenID, baseNotifyMessage);
			}
			else
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenEnable(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 1, 83, applicationUser, out tokenEventID);
					_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
				}
				else
				{
					if (_oldCoreStatus == TokenStatus.Canceled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 1, 83, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
					}
					else
					{
						if (_TKRules != null)
						{
							if (OperationResult.Success != (_hResult = _TKRules.BeforeEnable(applicationUser, tokenID, baseNotifyMessage, _oldCoreStatus)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 1, (int)_hResult, applicationUser, out tokenEventID);
								result = _hResult;
								return result;
							}
						}
						if (OperationResult.Success == (_hResult = new PREProcessorTokens().AllowedUse(tokenID)))
						{
							if (OperationResult.Success == (_hResult = new TokenBusinessDAO().tokenAllowedUse(applicationUser, tokenID, out tokenEventID)))
							{
								if (_TKRules != null)
								{
									result = _TKRules.AfterEnable(applicationUser, tokenID, baseNotifyMessage);
									return result;
								}
								_hResult = (result = OperationResult.Success);
								return result;
							}
							else
							{
								if (_oldCoreStatus == TokenStatus.Enabled)
								{
									new PREProcessorTokens().InhibitedUse(tokenID);
								}
								if (_oldCoreStatus == TokenStatus.Disabled)
								{
									new PREProcessorTokens().AllowedUse(tokenID);
								}
							}
						}
						SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
							Assembly.GetExecutingAssembly().FullName.ToString(),
							"unable to enable token::[" + applicationUser + "]"
						});
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 1, -1, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.Error);
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"unable to enable token::[" + applicationUser + "]",
					ex.ToString()
				});
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_225_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_225_1 = 1;
				string arg_225_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_ENABLE.ToString();
				string[] arg_222_0 = array;
				int arg_222_1 = 1;
				int num = (int)_hResult;
				arg_222_0[arg_222_1] = num.ToString();
				SAFInternalEvents.Export(arg_225_0, arg_225_1, arg_225_2, array);
			}
			return result;
		}
		public static OperationResult tokenCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage, out TokenInfo tokenInfo)
		{
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			DateTime _dtExpiration;
			if (!DateTime.TryParse(expirationDate, out _dtExpiration))
			{
				_dtExpiration = DateTime.MaxValue;
			}
			OperationResult result;
			try
			{
				if (_TKRules != null)
				{
					if (OperationResult.Success != (_hResult = _TKRules.BeforeCreate(applicationUser, applicationUseruserPhone, applicationEmail, tokenVendorID, expirationDate, supplierSerialNumber, creationLotID, pin, baseNotifyMessage)))
					{
						new TokensBusinessEventsDAO().insertTokenEvent("0", 100, (int)_hResult, applicationUser, out tokenEventID);
						tokenInfo = new TokenInfo();
						result = _hResult;
						return result;
					}
				}
				TokenInfoCore _tkInfoCore;
				if (OperationResult.Success == (_hResult = new PREProcessorTokens().Create(tokenVendorID, _dtExpiration, supplierSerialNumber, creationLotID, pin, out _tkInfoCore)))
				{
					if (OperationResult.Success != (_hResult = new TokenBusinessDAO().insertTokenUser(_tkInfoCore, applicationUser, applicationUseruserPhone, applicationEmail, out tokenInfo, out tokenEventID)))
					{
						SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
						{
							"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
							Assembly.GetExecutingAssembly().FullName.ToString(),
							"unable to create token on [business] DB"
						});
						if (OperationResult.Success != (_hResult = new PREProcessorTokens().UndoCreate(_tkInfoCore.InternalID.ToString())))
						{
							SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.WARNING, "SAFBUSINESS", new string[]
							{
								"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
								"unable to undoCreate token on [core] DB"
							});
						}
					}
					else
					{
						if (_TKRules != null)
						{
							result = _TKRules.AfterCreate(applicationUser, applicationUseruserPhone, applicationEmail, tokenVendorID, expirationDate, supplierSerialNumber, creationLotID, pin, baseNotifyMessage + "|" + tokenEventID.ToString(), _tkInfoCore.InternalID, tokenEventID, _tkInfoCore.InternalStatus);
							return result;
						}
						_hResult = OperationResult.Success;
						result = _hResult;
						return result;
					}
				}
				else
				{
					SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.ERROR, "SAFBUSINESS", new string[]
					{
						"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						"unable to create token on [core] DB"
					});
				}
				new TokensBusinessEventsDAO().insertTokenEvent("0", 100, (int)_hResult, applicationUser, out tokenEventID);
				tokenInfo = new TokenInfo();
				_hResult = (result = OperationResult.Error);
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					"unable to create token!",
					ex.ToString()
				});
				tokenInfo = new TokenInfo();
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_294_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_294_1 = 100;
				string arg_294_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_CREATED.ToString();
				string[] arg_291_0 = array;
				int arg_291_1 = 1;
				int num = (int)_hResult;
				arg_291_0[arg_291_1] = num.ToString();
				SAFInternalEvents.Export(arg_294_0, arg_294_1, arg_294_2, array);
			}
			return result;
		}
		public static OperationResult tokenSynchronize(int tokenVendorID, string internalSerialNumber, string firstPassword, string secondPassword, string baseNotifyMessage)
		{
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenSynchronize(_applicationUser, _tokenID, firstPassword, secondPassword, baseNotifyMessage);
			}
			else
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenSynchronize(string applicationUser, string tokenID, string firstPassword, string secondPassword, string baseNotifyMessage)
		{
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			TokenCryptoData _tkCryptoData = default(TokenCryptoData);
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 104, 83, applicationUser, out tokenEventID);
					_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
				}
				else
				{
					if (_oldCoreStatus != TokenStatus.Enabled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 104, 83, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
					}
					else
					{
						if (_TKRules != null)
						{
							_tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenID);
							if (OperationResult.Success != (_hResult = _TKRules.BeforeSynchronize(applicationUser, tokenID, baseNotifyMessage, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 104, (int)_hResult, applicationUser, out tokenEventID);
								result = _hResult;
								return result;
							}
						}
						_hResult = new PREProcessorTokens().Synchronize(tokenID, firstPassword, secondPassword);
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 104, (int)_hResult, applicationUser, out tokenEventID);
						if (_hResult == OperationResult.Success)
						{
							if (_TKRules != null)
							{
								_hResult = _TKRules.AfterSynchronize(applicationUser, tokenID, baseNotifyMessage, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType);
							}
						}
						result = _hResult;
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_1C9_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_1C9_1 = 104;
				string arg_1C9_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_REQUEST_SYNCHRONIZATION.ToString();
				string[] arg_1C6_0 = array;
				int arg_1C6_1 = 1;
				int num = (int)_hResult;
				arg_1C6_0[arg_1C6_1] = num.ToString();
				SAFInternalEvents.Export(arg_1C9_0, arg_1C9_1, arg_1C9_2, array);
			}
			return result;
		}
		public static AutenticationStatus tokenPasswordValidation(int tokenParamsID, string supplierSerialNumber, string tokenPassword, string dataEntropy, string baseNotifyMessage, out string newChallenge)
		{
			newChallenge = null;
			string _tokenID = null;
			string _applicationUser = null;
			AutenticationStatus result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDBySupplierSNAndType(supplierSerialNumber, tokenParamsID, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenPasswordValidation(_applicationUser, _tokenID, tokenPassword, dataEntropy, baseNotifyMessage, out newChallenge);
			}
			else
			{
				result = AutenticationStatus.TokenNotFoundOrCanceled;
			}
			return result;
		}
		public static AutenticationStatus tokenPasswordValidation(string applicationUser, string tokenPassword, string dataEntropy, string baseNotifyMessage, out string newChallenge)
		{
			newChallenge = null;
			int _totRows = 0;
			long tokenEventID = 0L;
			bool _pwdValidStat = false;
			AutenticationStatus _autenticationStatus = AutenticationStatus.AutenticationProcessFail;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			AutenticationStatus result;
			try
			{
				TokenInfo[] _arrayTkInfo = new TokenBusinessDAO().loadActiveTokensByUser(applicationUser, "1", 9999, 1, out _totRows);
				if (_arrayTkInfo.Length < 1)
				{
					new TokensBusinessEventsDAO().insertTokenEvent("0", 205, 204, applicationUser, out tokenEventID);
					_autenticationStatus = (result = AutenticationStatus.TokenNotFoundOrCanceled);
				}
				else
				{
					for (int i = 0; i < _arrayTkInfo.Length; i++)
					{
						TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(_arrayTkInfo[i].tokenInfoCore.InternalID.ToString());
						if (_TKRules != null)
						{
							if (AutenticationStatus.Success != (_autenticationStatus = _TKRules.BeforeAutenticate(applicationUser, _arrayTkInfo[i].tokenInfoCore.InternalID.ToString(), baseNotifyMessage, true, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(_arrayTkInfo[i].tokenInfoCore.InternalID.ToString(), 102, (int)_autenticationStatus, applicationUser, out tokenEventID);
								result = _autenticationStatus;
								return result;
							}
						}
						_autenticationStatus = SAFBaseFunctions._tokenPasswordValidation(_arrayTkInfo[i].ApplicationUser, _arrayTkInfo[i].tokenInfoCore.InternalID.ToString(), tokenPassword, dataEntropy, out newChallenge);
						if (_autenticationStatus == AutenticationStatus.Success || _autenticationStatus == AutenticationStatus.SuccessButSynchronized)
						{
							_pwdValidStat = true;
							new TokensBusinessEventsDAO().insertTokenEvent(_arrayTkInfo[i].tokenInfoCore.InternalID.ToString(), 102, (int)_autenticationStatus, applicationUser, out tokenEventID);
							if (_TKRules != null)
							{
								_autenticationStatus = _TKRules.AfterAutenticate(applicationUser, _arrayTkInfo[i].tokenInfoCore.InternalID.ToString(), baseNotifyMessage, true, newChallenge, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType);
							}
							break;
						}
					}
					if (!_pwdValidStat)
					{
						new TokensBusinessEventsDAO().insertTokenEvent("0", 205, -1, applicationUser, out tokenEventID);
					}
					result = _autenticationStatus;
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_autenticationStatus = (result = AutenticationStatus.AutenticationProcessFail);
			}
			finally
			{
				APPEVENTSDeff arg_295_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_295_1 = 102;
				string arg_295_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_REQUEST_VALIDATION.ToString();
				string[] arg_292_0 = array;
				int arg_292_1 = 1;
				int num = (int)_autenticationStatus;
				arg_292_0[arg_292_1] = num.ToString();
				SAFInternalEvents.Export(arg_295_0, arg_295_1, arg_295_2, array);
			}
			return result;
		}
		public static AutenticationStatus tokenPasswordValidation(string applicationUser, string tokenID, string tokenPassword, string dataEntropy, string baseNotifyMessage, out string newChallenge)
		{
			newChallenge = null;
			long tokenEventID = 0L;
			TokenCryptoData _tkCryptoData = default(TokenCryptoData);
			AutenticationStatus _autenticationStatus = AutenticationStatus.AutenticationProcessFail;
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			AutenticationStatus result;
			try
			{
				if (_TKRules != null)
				{
					_tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenID);
					if (AutenticationStatus.Success != (_autenticationStatus = _TKRules.BeforeAutenticate(applicationUser, tokenID, baseNotifyMessage, false, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType)))
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 102, (int)_autenticationStatus, applicationUser, out tokenEventID);
						result = _autenticationStatus;
						return result;
					}
				}
				_autenticationStatus = SAFBaseFunctions._tokenPasswordValidation(applicationUser, tokenID, tokenPassword, dataEntropy, out newChallenge);
				new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 102, (int)_autenticationStatus, applicationUser, out tokenEventID);
				if (_autenticationStatus == AutenticationStatus.Success || _autenticationStatus == AutenticationStatus.SuccessButSynchronized)
				{
					if (_TKRules != null)
					{
						_autenticationStatus = _TKRules.AfterAutenticate(applicationUser, tokenID, baseNotifyMessage, false, newChallenge, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType);
					}
				}
				result = _autenticationStatus;
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_autenticationStatus = (result = AutenticationStatus.AutenticationProcessFail);
			}
			finally
			{
				APPEVENTSDeff arg_17E_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_17E_1 = 102;
				string arg_17E_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_REQUEST_VALIDATION.ToString();
				string[] arg_17B_0 = array;
				int arg_17B_1 = 1;
				int num = (int)_autenticationStatus;
				arg_17B_0[arg_17B_1] = num.ToString();
				SAFInternalEvents.Export(arg_17E_0, arg_17E_1, arg_17E_2, array);
			}
			return result;
		}
		public static OperationResult tokenStartServerAuthentication(string applicationUser, string dataEntropy, int tokenVendorID, string baseNotifyMessage, out string requestedPassword, out bool possuiToken, out bool possuiTokenActivo)
		{
			possuiToken = false;
			possuiTokenActivo = false;
			requestedPassword = null;
			string _tokenID = null;
			string _internalSerialNumber = null;
			OperationResult _hResult;
			OperationResult result;
			if (OperationResult.Success == (_hResult = new TokenBusinessDAO().getUserActiveTokenByVendorID(tokenVendorID, applicationUser, out _tokenID, out _internalSerialNumber)))
			{
				possuiToken = true;
				possuiTokenActivo = true;
				result = SAFBaseFunctions.tokenStartServerAuthentication(applicationUser, _tokenID, dataEntropy, baseNotifyMessage, out requestedPassword);
			}
			else
			{
				if (_hResult == OperationResult.WrongStatusForRequestedOperation)
				{
					possuiToken = true;
				}
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenStartServerAuthentication(string applicationUser, string tokenID, string dataEntropy, string baseNotifyMessage, out string requestedPassword)
		{
			requestedPassword = null;
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			TokenCryptoData _tkCryptoData = default(TokenCryptoData);
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 103, 83, applicationUser, out tokenEventID);
					_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
				}
				else
				{
					if (_oldCoreStatus != TokenStatus.Enabled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 103, 83, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
					}
					else
					{
						if (_TKRules != null)
						{
							_tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenID);
							if (OperationResult.Success != (_hResult = _TKRules.BeforeStartServerAuthentication(applicationUser, tokenID, baseNotifyMessage, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 103, (int)_hResult, applicationUser, out tokenEventID);
								result = _hResult;
								return result;
							}
						}
						_hResult = new PREProcessorTokens().StartServerAuthentication(tokenID, dataEntropy, out requestedPassword);
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 103, (int)_hResult, applicationUser, out tokenEventID);
						if (_hResult == OperationResult.Success)
						{
							if (_TKRules != null)
							{
								_hResult = _TKRules.AfterStartServerAuthentication(applicationUser, tokenID, baseNotifyMessage + "|" + requestedPassword, requestedPassword, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType);
							}
						}
						if (_hResult != OperationResult.Success)
						{
							new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 103, (int)_hResult, applicationUser, out tokenEventID);
						}
						result = _hResult;
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_1FA_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_1FA_1 = 103;
				string arg_1FA_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_START_SERVER_AUTHENTICATION.ToString();
				string[] arg_1F7_0 = array;
				int arg_1F7_1 = 1;
				int num = (int)_hResult;
				arg_1F7_0[arg_1F7_1] = num.ToString();
				SAFInternalEvents.Export(arg_1FA_0, arg_1FA_1, arg_1FA_2, array);
			}
			return result;
		}
		public static OperationResult tokenRequestChallenge(string applicationUser, string SupplierSerialNumber, string dataEntropy, out string requestedPassword)
		{
			requestedPassword = null;
			string _tokenID = null;
			int _tokenParamsID = 0;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByUserAndSupplierSN(applicationUser, SupplierSerialNumber, out _tokenParamsID, out _tokenID))
			{
				result = SAFBaseFunctions.tokenRequestChallenge(applicationUser, _tokenID, dataEntropy, null, out requestedPassword);
			}
			else
			{
				long tokenEventID;
				new TokensBusinessEventsDAO().insertTokenEvent("0", 101, -1, applicationUser, out tokenEventID);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenRequestChallenge(string applicationUser, string dataEntropy, int tokenVendorID, string baseNotifyMessage, out string requestedPassword)
		{
			requestedPassword = null;
			string _tokenID = null;
			string _internalSerialNumber = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getUserActiveTokenByVendorID(tokenVendorID, applicationUser, out _tokenID, out _internalSerialNumber))
			{
				result = SAFBaseFunctions.tokenRequestChallenge(applicationUser, _tokenID, dataEntropy, baseNotifyMessage, out requestedPassword);
			}
			else
			{
				long tokenEventID;
				new TokensBusinessEventsDAO().insertTokenEvent("0", 101, -1, applicationUser, out tokenEventID);
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenRequestChallenge(string applicationUser, string tokenID, string dataEntropy, string baseNotifyMessage, out string requestedChallenge)
		{
			requestedChallenge = null;
			long tokenEventID = 0L;
			OperationResult _hResult = OperationResult.Error;
			TokenCryptoData _tkCryptoData = default(TokenCryptoData);
			ITokenRules _TKRules = TokenRulesFactory.LoadAssembly(SAFConfiguration.readParameterExternal("SAFClientBusinessRules"));
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 101, 83, applicationUser, out tokenEventID);
					_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
				}
				else
				{
					if (_oldCoreStatus != TokenStatus.Enabled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 101, 83, applicationUser, out tokenEventID);
						_hResult = (result = OperationResult.WrongStatusForRequestedOperation);
					}
					else
					{
						if (_TKRules != null)
						{
							_tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenID);
							if (OperationResult.Success != (_hResult = _TKRules.BeforeChallengeRequest(applicationUser, tokenID, baseNotifyMessage, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType)))
							{
								new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 101, (int)_hResult, applicationUser, out tokenEventID);
								result = _hResult;
								return result;
							}
						}
						_hResult = new PREProcessorTokens().ChallengeRequest(tokenID, dataEntropy, out requestedChallenge);
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 101, (int)_hResult, applicationUser, out tokenEventID);
						if (_hResult == OperationResult.Success)
						{
							if (_TKRules != null)
							{
								_hResult = _TKRules.AfterChallengeRequest(applicationUser, tokenID, baseNotifyMessage + "|" + requestedChallenge, _tkCryptoData.TokenBaseParams.MovingFactorType, _tkCryptoData.TokenBaseParams.SeedType);
							}
						}
						result = _hResult;
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				_hResult = (result = OperationResult.Error);
			}
			finally
			{
				APPEVENTSDeff arg_1D9_0 = APPEVENTSDeff.OPERATIONS_EXECUTED;
				int arg_1D9_1 = 101;
				string arg_1D9_2 = "SAFBUSINESS";
				string[] array = new string[2];
				array[0] = TokenEventOperation.cTOKEN_REQUEST_CHALLENGE.ToString();
				string[] arg_1D6_0 = array;
				int arg_1D6_1 = 1;
				int num = (int)_hResult;
				arg_1D6_0[arg_1D6_1] = num.ToString();
				SAFInternalEvents.Export(arg_1D9_0, arg_1D9_1, arg_1D9_2, array);
			}
			return result;
		}
		public static OperationResult tokenResetChallenge(int tokenVendorID, string internalSerialNumber, string baseNotifyMessage)
		{
			string _tokenID = null;
			string _applicationUser = null;
			OperationResult result;
			if (OperationResult.Success == new TokenBusinessDAO().getTokenIDByVendorIDAndIntSerial(tokenVendorID, internalSerialNumber, out _applicationUser, out _tokenID))
			{
				result = SAFBaseFunctions.tokenResetChallenge(_applicationUser, _tokenID, baseNotifyMessage);
			}
			else
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public static OperationResult tokenResetChallenge(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			long tokenEventID = 0L;
			OperationResult result;
			try
			{
				TokenStatus _oldCoreStatus;
				if (!SAFBaseFunctions._checkStatusConsistency(tokenID, applicationUser, out _oldCoreStatus))
				{
					new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 105, 83, applicationUser, out tokenEventID);
					result = OperationResult.WrongStatusForRequestedOperation;
				}
				else
				{
					if (_oldCoreStatus != TokenStatus.Enabled)
					{
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 105, 83, applicationUser, out tokenEventID);
						result = OperationResult.WrongStatusForRequestedOperation;
					}
					else
					{
						OperationResult _hResult = new PREProcessorTokens().ResetChallengeRequest(tokenID);
						new TokensBusinessEventsDAO().insertTokenEvent(tokenID, 105, (int)_hResult, applicationUser, out tokenEventID);
						result = _hResult;
					}
				}
			}
			catch (Exception ex)
			{
				SAFBaseFunctions._logger(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFBUSINESS", new string[]
				{
					"http://sfexpand.SAFBusiness.DBConnectionString.softfinanca.com/",
					Assembly.GetExecutingAssembly().FullName.ToString(),
					ex.ToString()
				});
				result = OperationResult.Error;
			}
			return result;
		}
		public static long tokensSeedsLoadFile(string pathFileName, string tokensExpirationDate)
		{
			DateTime _datetime;
			if (!DateTime.TryParse(tokensExpirationDate, out _datetime))
			{
				_datetime = DateTime.MaxValue;
			}
			return BaseImportExportTokens.tokensSeedsBulkInsert(pathFileName, TokenStatus.Undefined, _datetime);
		}
		public static OperationResult tokenTANExportMatrixPositionsBySubLotID(string subLotID, string tokenVendorID)
		{
			return TokensBaseFunctions.tokenTANFetchMatrixValues(LoteType.SubLot, subLotID, tokenVendorID);
		}
		public static OperationResult tokenTANExportMatrixPositionsBySupplierLot(string supplierLotID, string tokenVendorID)
		{
			return TokensBaseFunctions.tokenTANFetchMatrixValues(LoteType.SupplierLot, supplierLotID, tokenVendorID);
		}
		public static OperationResult tokenTANExportSupplierSerialNumberBySubLot(string subLotID, string tokenVendorID)
		{
			return TokensBaseFunctions.tokenTANFetchSupplierSerialNumber(LoteType.SubLot, subLotID, tokenVendorID);
		}
		public static OperationResult tokenTANExportSupplierSerialNumberBySupplierLot(string supplierLotID, string tokenVendorID)
		{
			return TokensBaseFunctions.tokenTANFetchSupplierSerialNumber(LoteType.SupplierLot, supplierLotID, tokenVendorID);
		}
		public static OperationResult createSubLot(long numberOfTokensInLot, string tokenParamsID, string subLotID, DateTime expirationDate, out long numberOfSeeds)
		{
			return new TokensDAO().createSubLotByTokenVendor(numberOfTokensInLot, tokenParamsID, subLotID, expirationDate, out numberOfSeeds);
		}
		public static OperationResult seedsLoadSubLots(string supplierLotId, string paramsID, out DataTable dataTable)
		{
			return new TokensDAO().loadSubLots(supplierLotId, paramsID, out dataTable);
		}
		public static OperationResult seedsLoadSubLots(out DataTable dataTable)
		{
			return new TokensDAO().loadSubLots(out dataTable);
		}
		public static OperationResult loadSupplierLots(string tokenParamsID, out DataTable dataTable)
		{
			return new TokensDAO().loadSupplierLots(tokenParamsID, out dataTable);
		}
		public static OperationResult loadSupplierLots(out DataTable dataTable)
		{
			return new TokensDAO().loadSupplierLots(out dataTable);
		}
		public static OperationResult loadSeedsStatusByParamsID(out DataTable dataTable)
		{
			return new TokensDAO().seedsStatusByParamsID(out dataTable);
		}
		public static OperationResult loadSeedsStatusBySublotID(out DataTable dataTable)
		{
			return new TokensDAO().seedsStatusBySublotID(out dataTable);
		}
		public static OperationResult loadSeedsStatusBySupplierLotID(out DataTable dataTable)
		{
			return new TokensDAO().seedsStatusBySupplierLotID(out dataTable);
		}
		public static OperationResult seedsStatusSublotBySupplierLotID(string supplierLotID, out DataTable dataTable)
		{
			return new TokensDAO().seedsStatusSublotBySupplierLotID(supplierLotID, out dataTable);
		}
		public static OperationResult loadTokenLotInformation(int tokenVendorID, string lotID, out DataTable dataTable)
		{
			return new TokensDAO().loadTokenLotInformation(tokenVendorID, lotID, out dataTable);
		}
		public static OperationResult loadTokenLotBySupplier(int tokenVendorID, out DataTable dataTable)
		{
			return new TokensDAO().loadTokenLotBySupplier(tokenVendorID, out dataTable);
		}
		public static OperationResult loadTokenSubLotBySupplier(int tokenVendorID, out DataTable dataTable)
		{
			return new TokensDAO().loadTokenSubLotBySupplier(tokenVendorID, out dataTable);
		}
		public static OperationResult loadTokenSubLotBySupplierLot(string supplierLotID, out DataTable dataTable)
		{
			return new TokensDAO().loadTokenSubLotBySupplierLot(supplierLotID, out dataTable);
		}
		public static OperationResult tokenSeedsByParamIDWithNoSubLot(string supplierLotID, out long seedsAvailable)
		{
			return new TokensDAO().tokenSeedsByParamIDWithNoSubLot(supplierLotID, out seedsAvailable);
		}

        public static OperationResult loadTokenKeyInformation(string tokenID, out string tokenKey)
        {
            OperationResult result = OperationResult.Error;
            tokenKey = null;
            TokenCryptoData tokenCryptoData2 = new TokensDAO().loadTokenCryptoData(tokenID);

            string masterKey = SF.Expand.SAF.Configuration.SAFConfiguration.readMasterKey();
            byte[] tokenSeed = tokenCryptoData2.GetTokenSeed(masterKey);
            if (tokenSeed != null)
            {
                Base32Encoder enc = new Base32Encoder();
                tokenKey = enc.Encode(tokenSeed);
                result = OperationResult.Success;
            }

            return result;
        }
    }
}
