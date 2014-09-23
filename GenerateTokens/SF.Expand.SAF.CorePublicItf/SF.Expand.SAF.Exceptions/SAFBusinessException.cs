using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
namespace SF.Expand.SAF.Exceptions
{
	[Serializable]
	public class SAFBusinessException : ApplicationException
	{
		public SAFBusinessException()
		{
		}
		public SAFBusinessException(string message) : base(message)
		{
		}
		public SAFBusinessException(string message, Exception inner) : base(message, inner)
		{
		}
		protected SAFBusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
