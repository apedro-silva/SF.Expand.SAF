using System;
namespace SF.Expand.SAF.CorePublicItf
{
	[Serializable]
	public class TokensEventFields
	{
		private long Sequence;
		private DateTime BuildDate;
		private string FieldName;
		private string fieldValue;
		public static TokensEventFields loadTokensEventFields(long eventFieldSequence, DateTime eventFieldBuildDate, string eventFieldName, string eventFieldValue)
		{
			return new TokensEventFields
			{
				Sequence = eventFieldSequence,
				BuildDate = eventFieldBuildDate,
				FieldName = eventFieldName,
				fieldValue = eventFieldValue
			};
		}
	}
}
