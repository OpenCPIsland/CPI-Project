using System;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarException : SharpZipBaseException
	{
		public TarException()
		{
		}

		public TarException(string message)
			: base(message)
		{
		}

		public TarException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
