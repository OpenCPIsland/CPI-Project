using System.Collections.Generic;

namespace DI.HTTP.Security.Pinning
{
	public class DigestSet : IDigestSet
	{
		private string sha1;

		private string sha256;

		public DigestSet()
		{
		}

		public DigestSet(IDictionary<string, object> blob)
		{
			setSha1((string)blob["sha1"]);
			setSha256((string)blob["sha256"]);
		}

		public string getSha1()
		{
			return sha1;
		}

		public void setSha1(string sha1)
		{
			this.sha1 = sha1;
		}

		public string getSha256()
		{
			return sha256;
		}

		public void setSha256(string sha256)
		{
			this.sha256 = sha256;
		}

		public bool compareDigests(IDigestSet other)
		{
			if (getSha256() == null)
			{
				return other.getSha256() == null;
			}
			return getSha256().CompareTo(other.getSha256()) == 0 || getSha1().CompareTo(other.getSha1()) == 0;
		}
	}
}
