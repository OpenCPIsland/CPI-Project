using System;

namespace DI.HTTP.Security.Pinning
{
	public interface IPinningInfo
	{
		IDigestSet getCertificate();

		IDigestSet getSubject();

		DateTime getExpiration();

		PinningMode getMode();
	}
}
