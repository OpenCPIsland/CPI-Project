using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Fabric")]
	public class FabricDefinition : StaticGameDataDefinition, IMemberLocked
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public string AssetName;

		public TagDefinition[] Tags;

		public bool allowRotationAndScale = true;

		[SerializeField]
		[JsonProperty]
		private bool isMemberOnly;

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
		}
	}
}
