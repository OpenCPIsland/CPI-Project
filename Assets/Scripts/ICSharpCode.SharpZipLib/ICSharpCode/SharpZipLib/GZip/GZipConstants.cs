namespace ICSharpCode.SharpZipLib.GZip
{
	public sealed class GZipConstants
	{
		public const int GZIP_MAGIC = 8075;

		public const int FTEXT = 1;

		public const int FHCRC = 2;

		public const int FEXTRA = 4;

		public const int FNAME = 8;

		public const int FCOMMENT = 16;

		private GZipConstants()
		{
		}
	}
}
