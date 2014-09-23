using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokensEvents
	{
		private long ID;
		private DateTime RequestedDate;
		private DateTime ExecutedDate;
		private string OperationDescription;
		private bool OperationStatus;
		public static TokensEvents loadTokensEvents(long eventID, DateTime eventRequestedDate, DateTime eventExecutedDate, string OperationDescription, bool eventOperationStatus)
		{
			return new TokensEvents
			{
				ID = eventID,
				RequestedDate = eventRequestedDate,
				ExecutedDate = eventExecutedDate,
				OperationStatus = eventOperationStatus,
				OperationDescription = OperationDescription
			};
		}
	}
}
