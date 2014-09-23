using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokenTotalsByType
	{
		public DateTime CreationDate;
		public int TotalSMS;
		public int TotalMobile;
		public int TotalTanCode;
		public int Total;
		public static TokenTotalsByType tokenTotalsByType(DateTime CreationDate, int TotalSMS, int TotalMobile, int TotalTanCode, int Total)
		{
			return new TokenTotalsByType
			{
				CreationDate = CreationDate,
				TotalSMS = TotalSMS,
				TotalMobile = TotalMobile,
				TotalTanCode = TotalTanCode,
				Total = Total
			};
		}
	}
}
