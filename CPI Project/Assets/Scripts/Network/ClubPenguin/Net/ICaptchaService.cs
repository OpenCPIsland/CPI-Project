using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface ICaptchaService : INetworkService
	{
		bool BypassCaptcha
		{
			get;
			set;
		}

		void GetCaptcha(CaptchaType type, int? width = null, int? height = null);

		void PostCaptchaSolution(CaptchaType type, CaptchaSolution solution);
	}
}
