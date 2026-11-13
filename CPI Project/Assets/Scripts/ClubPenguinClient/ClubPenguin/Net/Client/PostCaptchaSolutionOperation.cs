using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpContentType("application/json")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/captcha/v1/solution")]
	[HttpAccept("application/json")]
	public class PostCaptchaSolutionOperation : CPAPIHttpOperation
	{
		[HttpQueryString("type")]
		public CaptchaType Type;

		[HttpRequestJsonBody]
		public CaptchaSolution CaptchaSolution;

		public PostCaptchaSolutionOperation(CaptchaType type, CaptchaSolution captchaSolution)
		{
			Type = type;
			CaptchaSolution = captchaSolution;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
