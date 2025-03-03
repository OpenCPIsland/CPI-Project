namespace Disney.Kelowna.Common
{
	public class GcsAccessTokenResponse
	{
		public string AccessToken
		{
			get;
			set;
		}

		public string TokenType
		{
			get;
			set;
		}

		public int ExpiresIn
		{
			get;
			set;
		}

		public long TimeOfExpiration
		{
			get;
			set;
		}

		public GcsAccessTokenResponse()
		{
			Clear();
		}

		public GcsAccessTokenResponse(RawGcsAuthResponse rawGcsAuthResponse, long assertionTime)
		{
			AccessToken = rawGcsAuthResponse.access_token;
			TokenType = rawGcsAuthResponse.token_type;
			ExpiresIn = rawGcsAuthResponse.expires_in;
			TimeOfExpiration = assertionTime + rawGcsAuthResponse.expires_in;
		}

		public void Clear()
		{
			AccessToken = "";
			TokenType = "";
			ExpiresIn = 0;
			TimeOfExpiration = 0L;
		}

		public void Copy(GcsAccessTokenResponse copyFrom)
		{
			if (copyFrom == null)
			{
				AccessToken = "";
				TokenType = "";
				ExpiresIn = 0;
				TimeOfExpiration = 0L;
			}
			else
			{
				AccessToken = copyFrom.AccessToken;
				TokenType = copyFrom.TokenType;
				ExpiresIn = copyFrom.ExpiresIn;
				TimeOfExpiration = copyFrom.TimeOfExpiration;
			}
		}

		public override string ToString()
		{
			return string.Format("AccessToken={0}, TokenType={1}, ExpiresIn={2}, TimeOfExpiration={3}", AccessToken, TokenType, ExpiresIn, TimeOfExpiration);
		}
	}
}
