using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace DI.HTTP.Security.Pinning
{
	public interface IPinset
	{
		IList<string> getSubjects();

		IList<string> getHosts();

		IList<IPinningInfo> getPinningInfo(string fqdn);

		bool validateCertificate(X509Certificate certificate);
	}
}
