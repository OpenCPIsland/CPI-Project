using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DisneyStore
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/DisneyStoreFranchise")]
	public class DisneyStoreFranchiseDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public bool IsEnabled;

		public List<DisneyStoreItemDefinition> Items;

		[HideInInspector]
		public long StartTimeInSeconds = 0L;

		[HideInInspector]
		public long EndTimeInSeconds = 86400L;

		[JsonIgnore]
		public PrefabContentKey HomePrefabKey;

		[JsonIgnore]
		public LocalizedSpriteAssetContentKey FranchiseIconPath;

		[JsonIgnore]
		public SpriteContentKey FranchiseHeaderPath;

		[JsonIgnore]
		public Color FranchiseBackgroundColor;

		[JsonIgnore]
		public int SortingIdDesc;
	}
}
