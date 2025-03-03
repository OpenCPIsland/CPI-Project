using System;

namespace ICSharpCode.SharpZipLib
{
	public class SharpZipBaseException : ApplicationException
	{
		public SharpZipBaseException()
		{
		}

		public SharpZipBaseException(string message)
			: base(message)
		{
		}

		public SharpZipBaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
