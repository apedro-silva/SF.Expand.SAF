using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.Secure.Business;
using System;
namespace STTokenRules
{
	public class STTokenRules : ITokenRules
	{
		private const string cMODULE_NAME = "SMSCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.STTokenRules.softfinanca.com/";
		private const string SMS_NOTIF_TEMPLATE_ON_TOKENCREATE = "SMS.TEMPLATE.ON.CREATE";
		private const string SMS_NOTIF_TEMPLATE_ON_TOKENCREATEDEPLOY = "SMS.TEMPLATE.ON.CREATE.DEPLOY";
		private const string SMS_NOTIF_TEMPLATE_ON_STARTSERVERAUTH = "SMS.TEMPLATE.ON.STARTSERVERAUTH";
		public OperationResult BeforeCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage)
		{
			int _tokenVendorID = 0;
			int.TryParse(tokenVendorID, out _tokenVendorID);
			OperationResult result;
			if (_tokenVendorID == 3)
			{
				TokenInfo _tkInfo;
				if (OperationResult.TokenVendorSeedNotAvaliable == new TokenBusinessDAO().getTokenByUserBySupplierSNAndParamID(applicationUser, supplierSerialNumber, _tokenVendorID, out _tkInfo))
				{
					result = OperationResult.PreValidationRulesFail;
					return result;
				}
			}
			result = OperationResult.Success;
			return result;
		}
		public OperationResult AfterCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage, int tokenInternalID, long businessEventID, TokenStatus tokenStatus)
		{
			TokenInfo _lastInsertedToken = new TokenInfo();
			OperationResult _hResult = OperationResult.Error;
			TokenInfo[] _tkInfo = new TokenBusinessDAO().loadTokenUserByType(applicationUser, tokenVendorID);
			OperationResult result;
			if (_tkInfo == null)
			{
				result = OperationResult.PostValidationRulesFail;
			}
			else
			{
				for (int i = 0; i < _tkInfo.Length; i++)
				{
					if (tokenInternalID == _tkInfo[i].tokenInfoCore.InternalID)
					{
						_lastInsertedToken = _tkInfo[i];
						_hResult = OperationResult.Success;
					}
					else
					{
						switch (_tkInfo[i].tokenInfoCore.TypeID)
						{
						case 1:
							if (_tkInfo[i].Status == TokenStatus.Enabled)
							{
								_hResult = SAFBaseFunctions.tokenDisable(_tkInfo[i].ApplicationUser, _tkInfo[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						case 2:
							if (_tkInfo[i].Status == TokenStatus.Enabled || _tkInfo[i].Status == TokenStatus.Disabled || _tkInfo[i].Status == TokenStatus.ReadyToDeploy || _tkInfo[i].Status == TokenStatus.DeployCompleted)
							{
								_hResult = SAFBaseFunctions.tokenCancel(_tkInfo[i].ApplicationUser, _tkInfo[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						case 3:
							if (_tkInfo[i].Status == TokenStatus.Enabled || _tkInfo[i].Status == TokenStatus.Disabled)
							{
								_hResult = SAFBaseFunctions.tokenCancel(_tkInfo[i].ApplicationUser, _tkInfo[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						}
					}
				}
				if (_hResult != OperationResult.Success)
				{
					result = _hResult;
				}
				else
				{
					string[] _arrayNotifMsg = (baseNotifyMessage ?? "").Split(new char[]
					{
						'|'
					});
					string[] _params = new string[_arrayNotifMsg.Length - 1];
					Array.Copy(_arrayNotifMsg, 1, _params, 0, _params.Length);
					string notifMsg = ((_arrayNotifMsg[0] ?? "").Length > 1) ? _arrayNotifMsg[0] : SAFConfiguration.readParameterExternal((tokenStatus == TokenStatus.ReadyToDeploy) ? "SMS.TEMPLATE.ON.CREATE.DEPLOY" : "SMS.TEMPLATE.ON.CREATE");
					notifMsg = string.Format(notifMsg.Replace("{tm}", DateTime.Now.ToShortDateString()).Replace("{dt}", DateTime.Now.ToShortTimeString()), _params);
					result = SMSSender.Send(_lastInsertedToken.ApplicationUser, _lastInsertedToken.tokenInfoCore.InternalID.ToString(), _lastInsertedToken.PhoneNumberUser, notifMsg);
				}
			}
			return result;
		}
		public OperationResult BeforeCancel(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterCancel(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			return OperationResult.Success;
		}
		public OperationResult BeforeDisable(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterDisable(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			return OperationResult.Success;
		}
		public OperationResult BeforeEnable(string applicationUser, string tokenID, string baseNotifyMessage, TokenStatus tokenCurrentStatus)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterEnable(string applicationUser, string tokenID, string baseNotifyMessage)
		{
			return OperationResult.Success;
		}
		public OperationResult BeforeStartServerAuthentication(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterStartServerAuthentication(string applicationUser, string tokenID, string baseNotifyMessage, string newPassword, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			string[] _arrayNotifMsg = (baseNotifyMessage ?? "").Split(new char[]
			{
				'|'
			});
			string[] _params = new string[_arrayNotifMsg.Length - 1];
			Array.Copy(_arrayNotifMsg, 1, _params, 0, _params.Length);
			string notifMsg = ((_arrayNotifMsg[0] ?? "").Length > 1) ? _arrayNotifMsg[0] : SAFConfiguration.readParameterExternal("SMS.TEMPLATE.ON.STARTSERVERAUTH");
			notifMsg = string.Format(notifMsg.Replace("{tm}", DateTime.Now.ToShortDateString()).Replace("{dt}", DateTime.Now.ToShortTimeString()), _params);
			return SMSSender.Send(applicationUser, tokenID, null, notifMsg);
		}
		public OperationResult BeforeSynchronize(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterSynchronize(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return OperationResult.Success;
		}
		public AutenticationStatus BeforeAutenticate(string applicationUser, string tokenID, string baseNotifyMessage, bool onLoopValidation, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return AutenticationStatus.Success;
		}
		public AutenticationStatus AfterAutenticate(string applicationUser, string tokenID, string baseNotifyMessage, bool onLoopValidation, string newChallenge, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return AutenticationStatus.Success;
		}
		public OperationResult BeforeChallengeRequest(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return OperationResult.Success;
		}
		public OperationResult AfterChallengeRequest(string applicationUser, string tokenID, string baseNotifyMessage, TokenMovingFactorType tokenMovingFactorType, TokenSeedType tokenSeedType)
		{
			return OperationResult.Success;
		}
	}
}
