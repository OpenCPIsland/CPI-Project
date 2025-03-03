using ClubPenguin.Core.StaticGameData;
using ClubPenguin.LOD;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Zone")]
	public class ZoneDefinition : StaticGameDataDefinition
	{
		public enum ZoneType
		{
			Interior,
			Exterior,
			Igloo,
			Game
		}

		[StaticGameDataDefinitionId]
		[Tooltip("The identifier of the zone")]
		public string ZoneName;

		[LocalizationToken]
		[Tooltip("The name as presented to users")]
		public string ZoneToken;

		public ZoneType Type;

		public int HardLimit;

		[Tooltip("The fraction of the hardlimit to start soft limiting")]
		[Range(0f, 1f)]
		public float SoftLimit;

		[Tooltip("The name of the scene file (without .unity)")]
		public string SceneName;

		[Tooltip("The full project path of the scene file (including .unity)")]
		public string SceneFilePath;

		public Vector3 DefaultAOI;

		public MapLimits MapLimits;

		public bool IsQuestOnly;

		[Space(10f)]
		public List<LODSystemDataReference> LODSystemData;

		[Header("Igloo only settings")]
		public SpriteContentKey IglooPreview;

		[Tooltip("One and only one igloo type zone definition must be the default igloo")]
		public bool DefaultIgloo;

		[Tooltip("Always spawn the player at the default location? For example in igloos we want this true.")]
		public bool AlwaysSpawnPlayerAtDefaultLocation = false;
	}
}
