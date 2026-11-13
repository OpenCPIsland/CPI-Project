using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/captcha/v1")]
	[HttpGET]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class GetCaptchaOperation : CPAPIHttpOperation
	{
		[HttpQueryString("type")]
		public CaptchaType Type;

		[HttpQueryString("width")]
		public int? Width;

		[HttpQueryString("height")]
		public int? Height;

		[HttpResponseJsonBody]
		public CaptchaData ResponseBody;

		public GetCaptchaOperation(CaptchaType type)
		{
			Type = type;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			throw new NotImplementedException();
		}
	}
}
