using System.Collections;

namespace Disney.Kelowna.Common
{
	public interface IGcsAccessTokenService
	{
		GcsAccessType AccessType
		{
			set;
		}

		IEnumerator GetAccessToken(GcsAccessTokenResponse gcsAccessTokenResponseBuffer);
	}
}
