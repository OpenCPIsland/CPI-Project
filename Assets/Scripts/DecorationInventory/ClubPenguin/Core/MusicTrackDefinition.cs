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
		public string InternalName;

		public PrefabContentKey Music;

		[Header("Genre the music falls under, used for sorting and coloring")]
		public MusicGenreDefinitionDefinitionKey MusicGenre;

		public override int GetId()
		{
			return Id;
		}
	}
}
