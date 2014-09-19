using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenInfoCore
	{
		public int InternalID;
		public byte TypeID;
		public TokenStatus InternalStatus;
		public string SupplierLotID;
		public string SupplierSerialNumber;
		public string InternalSerialNumber;
		public DateTime ExpirationTimeStamp;
		public string SubLotID;
		public static TokenInfoCore loadTokenInfoCore(byte typeID, int internalID, string supplierLotID, string supplierSerialNumber, string internalSerialNumber, DateTime expirationTimeStamp, string subLotID, TokenStatus internalStatus)
		{
			return new TokenInfoCore
			{
				TypeID = typeID,
				InternalID = internalID,
				SupplierLotID = (supplierLotID == null) ? null : supplierLotID.Trim(),
				SupplierSerialNumber = (supplierSerialNumber == null) ? null : supplierSerialNumber.Trim(),
				InternalSerialNumber = (internalSerialNumber == null) ? null : internalSerialNumber.Trim(),
				ExpirationTimeStamp = expirationTimeStamp,
				InternalStatus = internalStatus,
				SubLotID = subLotID
			};
		}
	}
}
