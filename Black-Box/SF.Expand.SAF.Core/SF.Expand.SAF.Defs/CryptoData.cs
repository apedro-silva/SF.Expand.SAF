using System;
namespace SF.Expand.SAF.Defs
{
	[Serializable]
	public struct CryptoData
	{
		private long _tokenMovingFactor;
		private string _tokenCryptoKey;
		private string _tokenInternalSerialNumber;
		private string _tokenSupportCryptoData;
		public long MovingFactor
		{
			get
			{
				return this._tokenMovingFactor;
			}
			set
			{
				this._tokenMovingFactor = value;
			}
		}
		public string SupportCryptoData
		{
			get
			{
				return this._tokenSupportCryptoData;
			}
			set
			{
				this._tokenSupportCryptoData = value;
			}
		}
		public string InternalSerialNumber
		{
			get
			{
				return this._tokenInternalSerialNumber;
			}
		}
		public string CryptoKey
		{
			get
			{
				return this._tokenCryptoKey;
			}
		}
		public CryptoData(long movingFactor, string cryptoKey, string internalSerialNumber, string tokenSupportCryptoData)
		{
			this._tokenMovingFactor = movingFactor;
			this._tokenCryptoKey = cryptoKey;
			this._tokenInternalSerialNumber = internalSerialNumber;
			this._tokenSupportCryptoData = tokenSupportCryptoData;
		}
	}
}
