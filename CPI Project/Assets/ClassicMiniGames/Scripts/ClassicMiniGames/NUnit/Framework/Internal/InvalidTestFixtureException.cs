using System;
using System.Runtime.Serialization;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public class InvalidTestFixtureException : ApplicationException
	{
		public InvalidTestFixtureException()
		{
		}

		public InvalidTestFixtureException(string message)
			: base(message)
		{
		}

		public InvalidTestFixtureException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidTestFixtureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
