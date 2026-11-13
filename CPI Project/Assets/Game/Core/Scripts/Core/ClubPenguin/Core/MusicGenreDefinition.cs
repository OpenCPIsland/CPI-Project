using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/MusicGenreDefinition")]
	public class MusicGenreDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		public string DisplayName;

		public Color GenreColor;
	}
}
