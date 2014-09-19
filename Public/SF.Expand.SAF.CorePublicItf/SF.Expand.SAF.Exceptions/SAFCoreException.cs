using System;
using System.Runtime.Serialization;
namespace SF.Expand.SAF.Exceptions
{
	[Serializable]
	public class SAFCoreException : ApplicationException
	{
		public SAFCoreException()
		{
		}
		public SAFCoreException(string message) : base(message)
		{
		}
		public SAFCoreException(string message, Exception inner) : base(message, inner)
		{
		}
		protected SAFCoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
