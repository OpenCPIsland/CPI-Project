using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/MusicTrack")]
	public class MusicTrackDefinition : IglooAssetDefinition<int>
	{
		public const int NO_MUSIC_ID = 0;

		[StaticGameDataDefinitionId]
		public int Id;

		[Header("The name used to identify the item in Axis and other internal tools")]
        [Tooltip("The name that you use in the MusicTrack prefab Play/ Fabric event.\n" +
            "Example: \n" +
            "Play/RainbowMigration2018 would = RainbowMigration2018")]
        public string InternalName;

		public PrefabContentKey Music;

        [Header("Genre the music falls under, used for sorting and coloring")]
        [Tooltip(
            "Music Genre IDs:\n" +
            "Adventure = 0\n" +
            "Cozy = 9\n" +
            "Dance = 1\n" +
            "Dubstep = 12\n" +
            "Edgy = 2\n" +
            "Feel Good = 3\n" +
            "Holiday = 4\n" +
            "Jazzy = 5\n" +
            "Mellow = 6\n" +
            "Pop = 10\n" +
            "Rock = 11\n" +
            "Silly = 7\n" +
            "Spooky = 8"
        )]
        public MusicGenreDefinitionDefinitionKey MusicGenre;

		public override int GetId()
		{
			return Id;
		}
	}
}
