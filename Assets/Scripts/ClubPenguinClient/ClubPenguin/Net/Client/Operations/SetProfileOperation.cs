using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client.Operations
{
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/player/v1/profile")]
	public class SetProfileOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public ClubPenguin.Net.Domain.Profile RequestBody;

		[HttpResponseJsonBody]
		public SignedResponse<ClubPenguin.Net.Domain.Profile> ResponseBody;

		public SetProfileOperation(ClubPenguin.Net.Domain.Profile profile)
		{
			RequestBody = profile;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.Profile value = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			value.Colour = RequestBody.colour;
			offlineDatabase.Write(value);
			ResponseBody = new SignedResponse<ClubPenguin.Net.Domain.Profile>
			{
				Data = new ClubPenguin.Net.Domain.Profile
				{
					colour = value.Colour,
					daysOld = value.DaysOld
				}
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ClubPenguin.Net.Offline.Profile value = offlineDatabase.Read<ClubPenguin.Net.Offline.Profile>();
			value.Colour = ResponseBody.Data.colour;
			value.DateCreated = DateTime.UtcNow.AddDays(-1 * ResponseBody.Data.daysOld).GetTimeInMilliseconds();
			offlineDatabase.Write(value);
		}
	}
}
