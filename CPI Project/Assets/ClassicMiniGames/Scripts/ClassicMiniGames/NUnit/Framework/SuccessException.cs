using System;
using System.Runtime.Serialization;

namespace NUnit.Framework
{
	[Serializable]
	public class SuccessException : Exception
	{
		public SuccessException(string message)
			: base(message)
		{
		}

		public SuccessException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected SuccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
