using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokensEvents
	{
		public long eventID;
		public string RequestedDate;
		public string ExecutedDate;
		public string OperationDescription;
		public string OperationStatus;
		public int TokenID;
		public string ApplicationUser;
		public string TokenDescription;
		public static TokensEvents loadTokensEvents(long eventID, string eventRequestedDate, int tokenID, string tokenDescription, string applicationUser, string eventExecutedDate, string OperationDescription, string eventOperationStatus)
		{
			return new TokensEvents
			{
				eventID = eventID,
				RequestedDate = eventRequestedDate,
				ExecutedDate = eventExecutedDate,
				OperationStatus = eventOperationStatus,
				OperationDescription = OperationDescription,
				TokenID = tokenID,
				ApplicationUser = applicationUser,
				TokenDescription = tokenDescription
			};
		}
	}
}
