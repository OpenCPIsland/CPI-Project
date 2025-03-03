using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/game/v1/rooms")]
	public class GetRoomsOperation : CPAPIHttpOperation
	{
		[HttpQueryString("language")]
		public string Language;

		[HttpQueryString("worlds")]
		public string Worlds;

		[HttpQueryString("rooms")]
		public string Rooms;

		[HttpQueryString("limit")]
		public int Limit = 0;

		[HttpResponseJsonBody]
		public List<RoomIdentifier> RoomsFound;

		public GetRoomsOperation(string language)
		{
			Language = language;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			RoomsFound = new List<RoomIdentifier>();
			string[] array = string.IsNullOrEmpty(Worlds) ? new string[1]
			{
				"Alpine"
			} : Worlds.Split(',');
			string[] array2 = string.IsNullOrEmpty(Rooms) ? new string[1]
			{
				"Boardwalk"
			} : Rooms.Split(',');
			string[] array3 = array;
			foreach (string world in array3)
			{
				string[] array4 = array2;
				foreach (string name in array4)
				{
					RoomsFound.Add(new RoomIdentifier
					{
						contentIdentifier = "",
						language = LocalizationLanguage.GetLanguageFromLanguageString(Language),
						world = world,
						zoneId = new ZoneId
						{
							name = name
						}
					});
				}
			}
		}
	}
}
