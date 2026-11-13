using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public static class CaptchaServiceEvents
	{
		public struct CaptchaLoaded
		{
			public readonly CaptchaData CaptchaData;

			public CaptchaLoaded(CaptchaData captchaData)
			{
				CaptchaData = captchaData;
			}
		}

		public struct CaptchaSolutionAccepted
		{
			public readonly string CaptchaId;

			public CaptchaSolutionAccepted(string captchaId)
			{
				CaptchaId = captchaId;
			}
		}

		public struct CaptchaSolutionDeclined
		{
			public readonly CaptchaData NewCaptchaData;

			public CaptchaSolutionDeclined(CaptchaData newCaptchaData)
			{
				NewCaptchaData = newCaptchaData;
			}
		}
	}
}
