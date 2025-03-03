using DI.HTTP.Security.Pinning;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DI.HTTP.Security
{
	public class CertificatePolicyHandler : ICertificatePolicy
	{
		private static CertificatePolicyHandler instance = null;

		private IPinset pinset = null;

		private CertificatePolicyHandler()
		{
			setPinset(DefaultPinsetFactory.getFactory().getPinset());
		}

		public IPinset getPinset()
		{
			return pinset;
		}

		public void setPinset(IPinset pinset)
		{
			this.pinset = pinset;
		}

		public static CertificatePolicyHandler getPolicyHandler()
		{
			if (instance == null)
			{
				instance = new CertificatePolicyHandler();
			}
			return instance;
		}

		public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
		{
			return true;
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Console.WriteLine("Certificate: {0}", certificate.GetCertHashString());
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
			return false;
		}
	}
}
