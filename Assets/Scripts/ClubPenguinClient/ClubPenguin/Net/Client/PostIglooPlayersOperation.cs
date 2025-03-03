using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using DevonLocalization.Core;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/game/v1/igloos/language/{$language}/players")]
	[RequestQueue("Quest")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpAccept("application/json")]
	public class PostIglooPlayersOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public ZoneId IglooId;

		[HttpUriSegment("language")]
		public string Language;

		[HttpQueryString("bypassCaptcha")]
		public bool BypassCaptcha = false;

		[HttpResponseJsonBody]
		public SignedResponse<JoinRoomData> SignedJoinRoomData;

		public PostIglooPlayersOperation(ZoneId igloo, string language)
		{
			IglooId = igloo;
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			string world = "Igloo";
			switch (LocalizationLanguage.GetLanguageFromLanguageString(Language))
			{
			case DevonLocalization.Core.Language.es_LA:
				world = "Iglú";
				break;
			case DevonLocalization.Core.Language.fr_FR:
				world = "Iglou";
				break;
			case DevonLocalization.Core.Language.pt_BR:
				world = "Iglu";
				break;
			}
			SignedJoinRoomData = PostRoomPlayersOperation.JoinRoom(world, Language, IglooId, offlineDatabase, offlineDefinitions);
			SignedJoinRoomData.Data.extraLayoutData = offlineDatabase.Read<IglooEntity>().Data.activeLayout;
			if (SignedJoinRoomData.Data.extraLayoutData == null)
			{
				SignedJoinRoomData.Data.extraLayoutData = new SceneLayout
				{
					zoneId = IglooId.name
				};
			}
			RegistrationProfile registrationProfile = offlineDatabase.Read<RegistrationProfile>();
			SignedJoinRoomData.Data.roomOwnerName = registrationProfile.displayName;
			if (string.IsNullOrEmpty(SignedJoinRoomData.Data.roomOwnerName))
			{
				SignedJoinRoomData.Data.roomOwnerName = registrationProfile.userName;
			}
			SignedJoinRoomData.Data.roomOwner = true;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			if (offlineDefinitions.IsOwnIgloo(IglooId))
			{
				IglooEntity value = offlineDatabase.Read<IglooEntity>();
				value.Data.activeLayout = SignedJoinRoomData.Data.extraLayoutData;
				offlineDatabase.Write(value);
			}
		}
	}
}
