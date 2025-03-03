namespace DI.HTTP.Security.Pinning
{
	public interface IDigestSet
	{
		string getSha1();

		string getSha256();

		bool compareDigests(IDigestSet other);
	}
}
