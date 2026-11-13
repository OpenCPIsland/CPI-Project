using System;
using System.Runtime.Serialization;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public class NUnitException : ApplicationException
	{
		public NUnitException()
		{
		}

		public NUnitException(string message)
			: base(message)
		{
		}

		public NUnitException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NUnitException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
