using System;

namespace Disney.Kelowna.Common
{
	public class ContentManifestException : Exception
	{
		public ContentManifestException(string message)
			: base(message)
		{
		}

		public ContentManifestException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
