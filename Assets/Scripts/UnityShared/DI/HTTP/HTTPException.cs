using System;

namespace DI.HTTP
{
	public class HTTPException : Exception
	{
		public HTTPException(string message)
			: base(message)
		{
		}
	}
}
