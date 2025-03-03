using System;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class InvalidHeaderException : TarException
	{
		public InvalidHeaderException()
		{
		}

		public InvalidHeaderException(string message)
			: base(message)
		{
		}

		public InvalidHeaderException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
