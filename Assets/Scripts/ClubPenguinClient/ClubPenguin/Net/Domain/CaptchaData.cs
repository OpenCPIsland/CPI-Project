using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct CaptchaData
	{
		public string id;

		public CaptchaType type;

		public long retryTimestamp;

		public string challengeImageData;

		public string captchaImageData;

		public CaptchaDimensions captchaDimensions;

		public string challengeToken;

		public int solutionSize;

		public byte[] ChallengeImageBytes
		{
			get
			{
				return Convert.FromBase64String(challengeImageData);
			}
		}

		public byte[] CaptchaImageBytes
		{
			get
			{
				return Convert.FromBase64String(captchaImageData);
			}
		}
	}
}
