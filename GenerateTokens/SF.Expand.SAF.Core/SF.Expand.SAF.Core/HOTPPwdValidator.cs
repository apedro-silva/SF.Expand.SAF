using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.Defs;
using SF.Expand.SAF.Utils;
using System;
namespace SF.Expand.SAF.Core
{
	public static class HOTPPwdValidator
	{
		public static OperationResult Synchronize(TokenCryptoData tokenCryptoData, byte[] entropy, string masterKey, string firstPwd, string secondPwd, out long movingFactor)
		{
			long num = tokenCryptoData.CryptoData.MovingFactor;
			int num2 = 0;
			while ((long)num2 < tokenCryptoData.TokenBaseParams.HOTPValidationWindow4Sync)
			{
				try
				{
					if (firstPwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)))
					{
						AutenticationStatus autenticationStatus = (num > tokenCryptoData.CryptoData.MovingFactor) ? AutenticationStatus.SuccessButSynchronized : AutenticationStatus.Success;
						if (autenticationStatus == AutenticationStatus.Success || autenticationStatus == AutenticationStatus.SuccessButSynchronized)
						{
							num += tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
							if (firstPwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)) && num <= tokenCryptoData.CryptoData.MovingFactor)
							{
								num = (movingFactor = num + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift);
								return OperationResult.Success;
							}
						}
					}
					num += tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
				}
				catch (Exception)
				{
					LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.HOTPPwdValidator::Synchronize[]", null);
				}
				num2++;
			}
			movingFactor = tokenCryptoData.CryptoData.MovingFactor;
			return OperationResult.Error;
		}
		public static AutenticationStatus Validate(TokenCryptoData tokenCryptoData, byte[] entropy, string masterKey, string validatePwd, out long movingFactor)
		{
			long num = tokenCryptoData.CryptoData.MovingFactor;
			for (int i = 0; i < tokenCryptoData.TokenBaseParams.OTPValidationWindow; i++)
			{
				try
				{
					if (validatePwd.Equals(HOTPPwdGenerator.generate(tokenCryptoData, masterKey, entropy)))
					{
						AutenticationStatus autenticationStatus = (num > tokenCryptoData.CryptoData.MovingFactor) ? AutenticationStatus.SuccessButSynchronized : AutenticationStatus.Success;
						if (autenticationStatus == AutenticationStatus.Success || autenticationStatus == AutenticationStatus.SuccessButSynchronized)
						{
							num = (movingFactor = num + tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift);
							return autenticationStatus;
						}
					}
					num += tokenCryptoData.TokenBaseParams.HOTPMovingFactorDrift;
				}
				catch (Exception)
				{
					LOGGER.Write(LOGGER.LogCategory.ERROR, "SF.Expand.SAF.Core.HOTPPwdValidator::validate[]", null);
				}
			}
			movingFactor = tokenCryptoData.CryptoData.MovingFactor;
			return AutenticationStatus.TokenOrPasswordInvalid;
		}
	}
}
