using System;

namespace Disney.MobileNetwork
{
	public class JsonParseException : ApplicationException
	{
		public JsonParseException()
		{
		}

		public JsonParseException(string message)
			: base(message)
		{
		}

		public JsonParseException(string message, Exception inner_exception)
			: base(message, inner_exception)
		{
		}
	}
}
