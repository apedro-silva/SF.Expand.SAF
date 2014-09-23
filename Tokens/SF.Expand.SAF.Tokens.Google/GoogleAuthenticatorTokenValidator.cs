using SF.Expand.SAF.Configuration;
using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using SF.Expand.SAF.Defs;
using System;
namespace SF.Expand.SAF.Tokens
{
    /// <summary>
    /// GoogleAuthenticatorTokenValidator : here we perform an OTP validation
    /// </summary>
	public class GoogleAuthenticatorTokenValidator : ITokenValidator
	{
		private string _masterKey = SAFConfiguration.readMasterKey();
		public OperationResult Synchronize(string tokenInternalID, string firstPwd, string secondPwd)
		{
			TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
			OperationResult result;
			if (_tkCryptoData.ID == null)
			{
				result = OperationResult.Error;
			}
			else
			{
				if (_tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.EventBase && _tkCryptoData.TokenBaseParams.SeedType == TokenSeedType.ActivactionKey)
				{
					throw new Exception("Function not implemented for this type of token!!");
				}
				long _MovingFactor;
				if (OperationResult.Success == HOTPPwdValidator.Synchronize(_tkCryptoData, new byte[0], this._masterKey, firstPwd, secondPwd, out _MovingFactor))
				{
					_tkCryptoData.ResetMovingFactor(_MovingFactor);
					if (new TokensDAO().updateMovingFactor(_tkCryptoData) != OperationResult.Success)
					{
						result = OperationResult.Success;
						return result;
					}
				}
				result = OperationResult.Error;
			}
			return result;
		}
		public AutenticationStatus Autenticate(string tokenInternalID, string password, string dataEntropy, out string newChallenge)
		{
			newChallenge = null;
			TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
			AutenticationStatus result;
			if (_tkCryptoData.ID == null)
			{
				result = AutenticationStatus.ErrorCheckTokenStatus;
			}
			else
			{
				if (_tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.EventBase && _tkCryptoData.TokenBaseParams.SeedType == TokenSeedType.ActivactionKey)
				{
					throw new Exception("Function not implemented for this type of token!!");
				}
				long _MovingFactor;
				AutenticationStatus _authStatus = HOTPPwdValidator.Validate(_tkCryptoData, (dataEntropy == null) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy), this._masterKey, password, out _MovingFactor);
				if (_authStatus == AutenticationStatus.Success || _authStatus == AutenticationStatus.SuccessButSynchronized)
				{
					_tkCryptoData.ResetMovingFactor(_MovingFactor);
					if (new TokensDAO().updateMovingFactor(_tkCryptoData) != OperationResult.Success)
					{
						result = AutenticationStatus.TokenOrPasswordInvalid;
						return result;
					}
				}
				result = _authStatus;
			}
			return result;
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, string dataEntropy, out string newPwd)
		{
			return this.StartServerAuthentication(tokenInternalID, -1L, dataEntropy, out newPwd);
		}
		public OperationResult StartServerAuthentication(string tokenInternalID, long movingFactorDrift, string dataEntropy, out string newPwd)
		{
			TokenCryptoData _tkCryptoData = new TokensDAO().loadTokenCryptoData(tokenInternalID);
			OperationResult result;
			if (_tkCryptoData.ID == null)
			{
				newPwd = null;
				result = OperationResult.Error;
			}
			else
			{
				if (_tkCryptoData.TokenBaseParams.MovingFactorType != TokenMovingFactorType.EventBase && _tkCryptoData.TokenBaseParams.SeedType == TokenSeedType.ActivactionKey)
				{
					throw new Exception("Function not implemented for this type of token!!");
				}
				if (movingFactorDrift > 0L)
				{
					_tkCryptoData.ResetMovingFactor(movingFactorDrift);
				}
				newPwd = HOTPPwdGenerator.generate(_tkCryptoData, this._masterKey, (dataEntropy == null) ? new byte[0] : BaseFunctions.convertStringToByteArray(dataEntropy));
				result = OperationResult.Success;
			}
			return result;
		}
		public OperationResult ChallengeRequest(string tokenInternalID, string dataEntropy, out string newChallenge)
		{
			throw new NotImplementedException();
		}
		public OperationResult ResetChallengeRequest(string tokenInternalID)
		{
			throw new NotImplementedException();
		}
		public OperationResult ResetMovingFactor(string tokenInternalID, long movingFactorValue)
		{
			throw new NotImplementedException();
		}
	}
}
