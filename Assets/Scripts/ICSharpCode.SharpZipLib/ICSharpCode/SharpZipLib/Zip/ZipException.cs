using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipException : SharpZipBaseException
	{
		public ZipException()
		{
		}

		public ZipException(string message)
			: base(message)
		{
		}

		public ZipException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
