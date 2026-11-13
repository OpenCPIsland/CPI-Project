using System;
using System.Runtime.Serialization;

namespace NUnit.Framework
{
	[Serializable]
	public class InconclusiveException : Exception
	{
		public InconclusiveException(string message)
			: base(message)
		{
		}

		public InconclusiveException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InconclusiveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
