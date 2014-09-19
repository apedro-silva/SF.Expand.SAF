using SF.Expand.SAF.Core;
using SF.Expand.SAF.CorePublicItf;
using SF.Expand.SAF.CryptoEngine;
using System;
namespace SF.Expand.SAF.Defs
{
	[Serializable]
	public struct TokenCryptoData
	{
		private string _tokenID;
		private string _tokenSupplierSerialNumber;
		private CryptoData _cryptoData;
		private TokenTypeBaseParams _tokenTypeBaseParams;
		public string ID
		{
			get
			{
				return this._tokenID;
			}
		}
		public string SupplierSerialNumber
		{
			get
			{
				return this._tokenSupplierSerialNumber;
			}
		}
		public CryptoData CryptoData
		{
			get
			{
				return this._cryptoData;
			}
			set
			{
				this._cryptoData = value;
			}
		}
		public TokenTypeBaseParams TokenBaseParams
		{
			get
			{
				return this._tokenTypeBaseParams;
			}
			set
			{
				this._tokenTypeBaseParams = value;
			}
		}
		public TokenCryptoData(string id, string supplierSerialNumber, CryptoData cryptoData, TokenTypeBaseParams tokenTypeBaseParams)
		{
			this._tokenID = id;
			this._tokenSupplierSerialNumber = supplierSerialNumber;
			this._cryptoData = cryptoData;
			this._tokenTypeBaseParams = tokenTypeBaseParams;
		}
		public TokenTimeFactory InitializeTimeMovingFactor()
		{
			TokenTimeFactory result;
			if (this._tokenTypeBaseParams.MovingFactorType != TokenMovingFactorType.TimeBase)
			{
				result = null;
			}
			else
			{
				result = new TokenTimeFactory(1, 1L, 1, 1);
			}
			return result;
		}
		public long InitializeEventMovingFactor()
		{
			if (this._tokenTypeBaseParams.MovingFactorType == TokenMovingFactorType.EventBase)
			{
				this._cryptoData.MovingFactor = (long)new Random().Next(199, 9999);
			}
			return this._cryptoData.MovingFactor;
		}
		public void ResetMovingFactor(long value)
		{
			this._cryptoData.MovingFactor = value;
		}
		public void ResetSupportCryptoData(string value)
		{
			this._cryptoData.SupportCryptoData = value;
		}
		public byte[] GetTokenSeed(string masterkey)
		{
			byte[] result;
			try
			{
				result = HOTPCipher.decryptData(BaseFunctions.HexDecoder(this._cryptoData.CryptoKey.Trim()), HOTPCipherInitialize.createCryptKey(BaseFunctions.HexDecoder(this._cryptoData.InternalSerialNumber.Trim()), (masterkey == null || masterkey.Length < 1) ? new byte[0] : BaseFunctions.convertStringToByteArray(masterkey)));
			}
			catch
			{
				result = null;
			}
			return result;
		}
	}
}
