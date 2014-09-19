using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.Secure.Business;
using System;
namespace STTokenCreationRules
{
	public class STTokenRules : ITokenRules
	{
		private const string cMODULE_NAME = "SMSCONNECTOR";
		private const string cBASE_NAME = "http://sfexpand.SAFBusiness.STTokenCreationRules.softfinanca.com/";
		private const string SMS_NOTIF_TEMPLATE_ON_TOKENCREATE = "OP.SMS.NOTIFY.ON.CREATE";
		private const string SMS_NOTIF_TEMPLATE_ON_TOKENCREATEDEPLOY = "OP.SMS.NOTIFY.ON.CREATE.DEPLOY";
		public OperationResult BeforeCreate(string applicationUser, string applicationUseruserPhone, string applicationEmail, string tokenVendorID, string expirationDate, string supplierSerialNumber, string creationLotID, string pin, string baseNotifyMessage)
		{
			int num = 0;
			int.TryParse(tokenVendorID, out num);
			OperationResult result;
			if (num == 3)
			{
				TokenInfo tokenInfo;
				if (OperationResult.TokenVendorSeedNotAvaliable == new TokenBusinessDAO().getTokenByUserBySupplierSNAndParamID(applicationUser, supplierSerialNumber, num, out tokenInfo))
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
			TokenInfo tokenInfo = new TokenInfo();
			OperationResult operationResult = OperationResult.Error;
			TokenInfo[] array = new TokenBusinessDAO().loadTokenUserByType(applicationUser, tokenVendorID);
			OperationResult result;
			if (array == null)
			{
				result = OperationResult.PostValidationRulesFail;
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (tokenInternalID == array[i].tokenInfoCore.InternalID)
					{
						tokenInfo = array[i];
						operationResult = OperationResult.Success;
					}
					else
					{
						switch (array[i].tokenInfoCore.TypeID)
						{
						case 1:
							if (array[i].Status == TokenStatus.Enabled)
							{
								operationResult = SAFBaseFunctions.tokenDisable(array[i].ApplicationUser, array[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						case 2:
							if (array[i].Status == TokenStatus.Enabled || array[i].Status == TokenStatus.Disabled)
							{
								operationResult = SAFBaseFunctions.tokenCancel(array[i].ApplicationUser, array[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						case 3:
							if (array[i].Status == TokenStatus.Enabled || array[i].Status == TokenStatus.Disabled)
							{
								operationResult = SAFBaseFunctions.tokenCancel(array[i].ApplicationUser, array[i].tokenInfoCore.InternalID.ToString(), string.Empty);
							}
							break;
						}
					}
				}
				if (operationResult != OperationResult.Success)
				{
					result = operationResult;
				}
				else
				{
					string text = SAFConfiguration.readParameterExternal((tokenStatus == TokenStatus.ReadyToDeploy) ? "OP.SMS.NOTIFY.ON.CREATE.DEPLOY" : "OP.SMS.NOTIFY.ON.CREATE");
					text = ((text.Trim().Length < 1) ? null : text.Trim());
					string smsMessage;
					if (0 >= (text ?? "").IndexOf("[0]"))
					{
						smsMessage = (((baseNotifyMessage ?? "").Length > 1) ? baseNotifyMessage : text).Replace("{dt}", DateTime.Now.ToShortDateString()).Replace("{tm}", DateTime.Now.ToShortTimeString()).Replace("{dpl}", businessEventID.ToString().Trim());
					}
					else
					{
						smsMessage = ((text != null) ? string.Format(text, baseNotifyMessage.Split(new char[]
						{
							'|'
						})) : string.Join("", baseNotifyMessage.Split(new char[]
						{
							'|'
						})).Trim());
					}
					result = SMSSender.Send(tokenInfo.ApplicationUser, tokenInfo.tokenInfoCore.InternalID.ToString(), tokenInfo.PhoneNumberUser, smsMessage);
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
			return OperationResult.Success;
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
