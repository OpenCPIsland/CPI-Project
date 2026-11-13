using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class SceneDefinition : StaticGameDataDefinition
	{
		[Tooltip("The identifier of the scene")]
		[StaticGameDataDefinitionId]
		public string SceneName;

		[Tooltip("List of scenes to load additively with the main scene")]
		public string[] AdditiveScenes = new string[0];

		[Tooltip("Use a number 0 or a negative number to use the default target fps")]
		public int TargetFrameRate = -1;

		[Tooltip("Use 0f to use use the default fixed delta time")]
		public float FixedDeltaTime = 0f;

		public PrefabContentKey SceneAudioContentKey;

		public AdditiveSceneOverride[] AdditiveSceneOverrides;
	}
}
