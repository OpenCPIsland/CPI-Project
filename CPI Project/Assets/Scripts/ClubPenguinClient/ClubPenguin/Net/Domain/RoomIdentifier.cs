using DevonLocalization.Core;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class RoomIdentifier
	{
		public string world;

		public Language language;

		public ZoneId zoneId;

		public string contentIdentifier;

		public override string ToString()
		{
			return world + ":" + LocalizationLanguage.GetLanguageString(language) + ":" + zoneId.name + ":" + zoneId.instanceId + ":" + contentIdentifier;
		}

		public RoomIdentifier()
		{
			world = "";
			language = Language.none;
			zoneId = new ZoneId();
			contentIdentifier = "";
		}

		public RoomIdentifier(string world, Language language, ZoneId zoneId, string contentIdentifier)
		{
			this.world = world;
			this.language = language;
			this.zoneId = zoneId;
			this.contentIdentifier = contentIdentifier;
		}

		public RoomIdentifier(string roomIdentifierString)
		{
			string[] array = roomIdentifierString.Split(':');
			world = array[0];
			language = LocalizationLanguage.GetLanguageFromLanguageString(array[1]);
			zoneId = new ZoneId();
			zoneId.name = array[2];
			zoneId.instanceId = array[3];
			contentIdentifier = array[4];
		}

		public static bool EqualsIgnoreInstanceId(string roomIdentifierString1, string roomIdentifierString2)
		{
			RoomIdentifier roomIdentifier = new RoomIdentifier(roomIdentifierString1);
			RoomIdentifier roomIdentifier2 = new RoomIdentifier(roomIdentifierString2);
			return roomIdentifier.world == roomIdentifier2.world && roomIdentifier.language == roomIdentifier2.language && roomIdentifier.zoneId.name == roomIdentifier2.zoneId.name && roomIdentifier.contentIdentifier == roomIdentifier2.contentIdentifier;
		}
	}
}
