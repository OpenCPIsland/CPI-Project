namespace ICSharpCode.SharpZipLib.LZW
{
	public sealed class LzwConstants
	{
		public const int MAGIC = 8093;

		public const int MAX_BITS = 16;

		public const int BIT_MASK = 31;

		public const int EXTENDED_MASK = 32;

		public const int RESERVED_MASK = 96;

		public const int BLOCK_MODE_MASK = 128;

		public const int HDR_SIZE = 3;

		public const int INIT_BITS = 9;

		private LzwConstants()
		{
		}
	}
}
