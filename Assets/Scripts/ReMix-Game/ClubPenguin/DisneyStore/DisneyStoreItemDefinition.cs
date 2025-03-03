using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.DisneyStore
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/DisneyStoreItem")]
	public class DisneyStoreItemDefinition : ScriptableObject
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public RewardThemeDefinition ThemeDefinition;

		public int Cost;

		public bool IsMemberOnly;

		public RewardDefinition Reward;

		[JsonIgnore]
		public string DescriptionToken;

		[JsonIgnore]
		public string TitleToken;

		[JsonIgnore]
		public SpriteContentKey Icon;
	}
}
