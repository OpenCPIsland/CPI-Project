namespace DevonLocalization.Core
{
	public interface ICompressionStrategy
	{
		void UncompressTokenTranslations(byte[] compressedBytes, string destinationPath);
	}
}
