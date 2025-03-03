using System;

namespace ICSharpCode.SharpZipLib.GZip
{
	public class GZipException : SharpZipBaseException
	{
		public GZipException()
		{
		}

		public GZipException(string message)
			: base(message)
		{
		}

		public GZipException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
