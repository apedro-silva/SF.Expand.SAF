using SF.Expand.LOG;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using System;
using System.Reflection;
namespace SF.Expand.SAF.Core
{
	public static class HOTPPwdValidator
	{
		private const string cMODULE_NAME = "SAFCORE";
		private const string cBASE_NAME = "http://sfexpand.SAFCore.HOTPPwdValidator.softfinanca.com/";
		public static OperationResult Synchronize(TokenCryptoData tokenCryptoData, byte[] entropy, string masterKey, string firstPwd, string secondPwd, out long movingFactor)
		{
			long _movingFactor = tokenCryptoData.CryptoData.MovingFactor;
			int i = 0;
			OperationResult result;
			while ((long)i < tokenCryptoData.TokenBaseParams.HOTPValidationWindow4Sync)
			{
				try
				{
					if (firstPwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)))
					{
						AutenticationStatus _authStatus = (_movingFactor > tokenCryptoData.CryptoData.MovingFactor) ? AutenticationStatus.SuccessButSynchronized : AutenticationStatus.Success;
						if (_authStatus == AutenticationStatus.Success || _authStatus == AutenticationStatus.SuccessButSynchronized)
						{
							_movingFactor += tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
							if (firstPwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)))
							{
								_authStatus = ((_movingFactor > tokenCryptoData.CryptoData.MovingFactor) ? AutenticationStatus.SuccessButSynchronized : AutenticationStatus.Success);
								if (_authStatus == AutenticationStatus.Success)
								{
									_movingFactor = (movingFactor = _movingFactor + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift);
									result = OperationResult.Success;
									return result;
								}
							}
						}
					}
					_movingFactor += tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.HOTPPwdValidator.softfinanca.com/",
						Assembly.GetExecutingAssembly().FullName.ToString(),
						ex.ToString()
					});
				}
				i++;
			}
			movingFactor = tokenCryptoData.CryptoData.MovingFactor;
			result = OperationResult.Error;
			return result;
		}
		public static AutenticationStatus Validate(TokenCryptoData tokenCryptoData, byte[] entropy, string masterKey, string validatePwd, out long movingFactor)
		{
			long _initialMovingFactor = tokenCryptoData.CryptoData.MovingFactor;
			AutenticationStatus result;
			for (int i = 0; i < tokenCryptoData.TokenBaseParams.OTPValidationWindow; i++)
			{
				try
				{
					if (validatePwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)))
					{
						AutenticationStatus authStatus = (tokenCryptoData.CryptoData.MovingFactor > _initialMovingFactor) ? AutenticationStatus.SuccessButSynchronized : AutenticationStatus.Success;
						movingFactor = tokenCryptoData.CryptoData.MovingFactor + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
						result = authStatus;
						return result;
					}
					movingFactor = tokenCryptoData.CryptoData.MovingFactor + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
					tokenCryptoData.ResetMovingFactor(movingFactor);
				}
				catch (Exception ex)
				{
					SAFLOGGER.Write(SAFLOGGER.LOGGEREventID.EXCEPTION, "SAFCORE", new string[]
					{
						"http://sfexpand.SAFCore.HOTPPwdValidator.softfinanca.com/",
						ex.ToString()
					});
				}
			}
			tokenCryptoData.ResetMovingFactor(_initialMovingFactor);
			movingFactor = _initialMovingFactor;
			result = AutenticationStatus.TokenOrPasswordInvalid;
			return result;
		}
	}
}
