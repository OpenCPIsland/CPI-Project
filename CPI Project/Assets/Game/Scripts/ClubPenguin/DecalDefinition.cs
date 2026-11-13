using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Decal")]
	public class DecalDefinition : StaticGameDataDefinition, IMemberLocked
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public string AssetName;

		public TagDefinition[] Tags;

		[JsonProperty]
		[SerializeField]
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
