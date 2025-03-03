using System;

namespace DI.HTTP.Security.Pinning
{
	public class PinningException : Exception
	{
		public PinningException(string message)
			: base(message)
		{
		}
	}
}
