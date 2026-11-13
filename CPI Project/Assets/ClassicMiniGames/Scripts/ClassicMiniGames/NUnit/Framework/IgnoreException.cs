using System;
using System.Runtime.Serialization;

namespace NUnit.Framework
{
	[Serializable]
	public class IgnoreException : Exception
	{
		public IgnoreException(string message)
			: base(message)
		{
		}

		public IgnoreException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected IgnoreException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
