using System.Security.Cryptography;

namespace ClubPenguin.Net.Client
{
	public interface ICPKeyValueDatabase
	{
		RSAParameters? GetRsaParameters();

		void SetRsaParameters(RSAParameters rsaParameters);
	}
}
