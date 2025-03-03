using DI.HTTP.Security;
using DI.HTTP.Security.Pinning;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DI.HTTP.Threaded
{
	public class RequestDigestSet
	{
		public IDigestSet CertificateDigest
		{
			get;
			set;
		}

		public IDigestSet SubjectDigest
		{
			get;
			set;
		}

		public bool ValidateCertificate(X509Certificate certificate, SslPolicyErrors sslPolicyError)
		{
			if (certificate != null)
			{
				DigestSet digestSet = new DigestSet();
				byte[] rawCertData = certificate.GetRawCertData();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				CertificateDigest = digestSet;
				digestSet = new DigestSet();
				rawCertData = certificate.GetPublicKey();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				SubjectDigest = digestSet;
				return true;
			}
			return false;
		}
	}
}
