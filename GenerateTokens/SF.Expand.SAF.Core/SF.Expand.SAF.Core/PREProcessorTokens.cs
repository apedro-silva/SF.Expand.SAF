using SF.Expand.SAF.Core.Factory;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
namespace SF.Expand.SAF.Core
{
	public class PREProcessorTokens : ITokens, ITokenValidator
	{
		public OperationResult Create(string tokenVendorID, DateTime expirationDate, string supplierSerialNumber, string creationLotID, string pin, out TokenInfoCore tokenInfoCore)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenParamsID(tokenVendorID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.Create(tokenVendorID, expirationDate, supplierSerialNumber, creationLotID, pin, out tokenInfoCore);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core.PREProcessorTokens::Create[]", innerException);
			}
			return result;
		}
		public OperationResult UndoCreate(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.UndoCreate(tokenInternalID);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core.PREProcessorTokens::UndoCreate[]", innerException);
			}
			return result;
		}
		public OperationResult InhibitedUse(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.InhibitedUse(tokenInternalID);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core.PREProcessorTokens::InhibitedUse[]", innerException);
			}
			return result;
		}
		public OperationResult AllowedUse(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.AllowedUse(tokenInternalID);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core.PREProcessorTokens::AllowedUse[]", innerException);
			}
			return result;
		}
		public OperationResult Cancel(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.Cancel(tokenInternalID);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core.PREProcessorTokens::Cancel[]", innerException);
			}
			return result;
		}
		public OperationResult CheckStatus(string tokenInternalID, out TokenStatus tokenStatus)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().DeployAssemblyNameByTokenID(tokenInternalID);
				ITokens tokens = TokensFactory.LoadAssembly(typeName);
				result = tokens.CheckStatus(tokenInternalID, out tokenStatus);
			}
			catch (Exception)
			{
				tokenStatus = TokenStatus.Undefined;
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult ChallengeRequest(string tokenInternalID, string dataEntropy, out string newChallange)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator tokenValidator = TokenValidatorFactory.LoadAssembly(typeName);
				result = tokenValidator.ChallengeRequest(tokenInternalID, dataEntropy, out newChallange);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core::StartServerAuthentication[]", innerException);
			}
			return result;
		}
		public AutenticationStatus Autenticate(string tokenInternalID, string password, string dataentropy, out string Challenge)
		{
			AutenticationStatus result;
			try
			{
				string typeName = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator tokenValidator = TokenValidatorFactory.LoadAssembly(typeName);
				result = tokenValidator.Autenticate(tokenInternalID, password, dataentropy, out Challenge);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core::Autenticate[]", innerException);
			}
			return result;
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, string dataEntropy, out string newPwd)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator tokenValidator = TokenValidatorFactory.LoadAssembly(typeName);
				result = tokenValidator.StartServerAuthentication(tokenInternalID, dataEntropy, out newPwd);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core::StartServerAuthentication[]", innerException);
			}
			return result;
		}
		public OperationResult Synchronize(string tokenInternalID, string firstPwd, string secondPwd)
		{
			OperationResult result;
			try
			{
				string typeName = new TokensValidatorDAO().ValidatorAssemblyNameByTokenID(tokenInternalID);
				ITokenValidator tokenValidator = TokenValidatorFactory.LoadAssembly(typeName);
				result = tokenValidator.Synchronize(tokenInternalID, firstPwd, secondPwd);
			}
			catch (Exception innerException)
			{
				throw new Exception("SF.Expand.SAF.Core::Synchronize[]", innerException);
			}
			return result;
		}
		public OperationResult ResetChallengeRequest(string tokenInternalID)
		{
			OperationResult result;
			try
			{
				TokenCryptoData tokenCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
				if (tokenCryptoData.ID == null)
				{
					result = OperationResult.Error;
				}
				else
				{
					result = new TokensChallengeRequestDAO().resetChallengeRequest(tokenCryptoData.ID);
				}
			}
			catch
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public OperationResult ResetMovingFactor(string tokenInternalID, long movingFactorValue)
		{
			OperationResult result;
			try
			{
				TokenCryptoData tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
				if (tkCryptoData.ID == null)
				{
					result = OperationResult.Error;
				}
				else
				{
					tkCryptoData.ResetMovingFactor(movingFactorValue);
					result = new TokensDAO().updateMovingFactor(tkCryptoData);
				}
			}
			catch
			{
				result = OperationResult.Error;
			}
			return result;
		}
		public TokenInfoCore loadTokenInfoCore(string tokenInternalID)
		{
			return new TokensDAO().loadTokenInfoCore(tokenInternalID);
		}
	}
}
