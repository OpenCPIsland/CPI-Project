using ClubPenguin.Core.StaticGameData;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using System;

namespace ClubPenguin.Game.PartyGames
{
	[Serializable]
	public class PartyGameLauncherDefinition : StaticGameDataDefinition
	{
		public enum LauncherMethods
		{
			PropTrigger,
			ZoneSchedule
		}

		[StaticGameDataDefinitionId]
		public int Id;

		[Obsolete("Roscs001 - Party game lobby data moved to the party game definition's lobby definition. This can be removed once all clients are on 1.7 or later.")]
		public PropDefinition LobbyProp;

		public PartyGameDefinition PartyGame;

		public LauncherMethods LauncherMethod = LauncherMethods.PropTrigger;

		public PropDefinition TriggerProp;

		public ZoneDefinition ScheduledZone;

		public int EveryXMinutesAfterTheHour;
	}
}
