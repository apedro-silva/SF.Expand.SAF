using SF.Expand.SAF.CorePublicItf;
using System;
namespace SF.Expand.SAF.Defs
{
	[Serializable]
	public struct TokenTypeBaseParams
	{
		private string _TokenTypeBaseParamsID;
		private int _hotpTotalDigits;
		private int _hotpOffSet;
		private int _hotpValidationWindow;
		private long _hotpMovingFactorDrift;
		private long _hotpValidationWindow4Sync;
		private int _tokenChallengeRequestValidUntil;
		private TokenSeedType _tokenSeedType;
		private TokenMovingFactorType _tokenMovingFactorType;
		public string TokenTypeBaseParamsID
		{
			get
			{
				return this._TokenTypeBaseParamsID;
			}
		}
		public long HOTPMovingFactorDrift
		{
			get
			{
				return this._hotpMovingFactorDrift;
			}
		}
		public long HOTPValidationWindow4Sync
		{
			get
			{
				return this._hotpValidationWindow4Sync;
			}
		}
		public int ChallengeRequestValidUntil
		{
			get
			{
				return this._tokenChallengeRequestValidUntil;
			}
		}
		public int OTPTotalDigits
		{
			get
			{
				return this._hotpTotalDigits;
			}
		}
		public int OTPOffSet
		{
			get
			{
				return this._hotpOffSet;
			}
		}
		public int OTPValidationWindow
		{
			get
			{
				return this._hotpValidationWindow;
			}
		}
		public TokenSeedType SeedType
		{
			get
			{
				return this._tokenSeedType;
			}
		}
		public TokenMovingFactorType MovingFactorType
		{
			get
			{
				return this._tokenMovingFactorType;
			}
		}
		public TokenTypeBaseParams(int hotpTotalDigits, int hotpOffSet, int hotpValidationWindow, long hotpMovingFactorDrift, TokenSeedType tokenSeedType, TokenMovingFactorType tokenMovingFactorType, long hotpValidationWindow4Sync, string tokenTypeBaseParamsID, int challengeRequestValidUntil)
		{
			this._TokenTypeBaseParamsID = tokenTypeBaseParamsID;
			this._hotpTotalDigits = hotpTotalDigits;
			this._hotpOffSet = hotpOffSet;
			this._hotpValidationWindow = hotpValidationWindow;
			this._hotpMovingFactorDrift = hotpMovingFactorDrift;
			this._tokenSeedType = tokenSeedType;
			this._tokenMovingFactorType = tokenMovingFactorType;
			this._hotpValidationWindow4Sync = hotpValidationWindow4Sync;
			this._tokenChallengeRequestValidUntil = challengeRequestValidUntil;
		}
	}
}
