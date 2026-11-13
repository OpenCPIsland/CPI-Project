using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class CaptchaApi
	{
		private ClubPenguinClient clubPenguinClient;

		public CaptchaApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetCaptchaOperation> GetCaptcha(CaptchaType type, int? width, int? height)
		{
			GetCaptchaOperation getCaptchaOperation = new GetCaptchaOperation(type);
			if (width.HasValue)
			{
				getCaptchaOperation.Width = width;
			}
			if (height.HasValue)
			{
				getCaptchaOperation.Height = height;
			}
			return new APICall<GetCaptchaOperation>(clubPenguinClient, getCaptchaOperation);
		}

		public APICall<PostCaptchaSolutionOperation> PostCaptchaSolution(CaptchaType type, CaptchaSolution captchaSolution)
		{
			PostCaptchaSolutionOperation operation = new PostCaptchaSolutionOperation(type, captchaSolution);
			return new APICall<PostCaptchaSolutionOperation>(clubPenguinClient, operation);
		}
	}
}
