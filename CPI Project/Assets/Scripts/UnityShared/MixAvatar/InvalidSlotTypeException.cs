using System;

namespace MixAvatar
{
	public class InvalidSlotTypeException : Exception
	{
		public InvalidSlotTypeException(string message)
			: base(message)
		{
		}

		public InvalidSlotTypeException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
