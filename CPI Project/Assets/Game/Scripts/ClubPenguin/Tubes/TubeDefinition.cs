using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.Tubes
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Tube")]
	public class TubeDefinition : StaticGameDataDefinition, IMemberLocked
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public TagDefinitionKey[] Tags = new TagDefinitionKey[0];

		[JsonIgnore]
		public SpriteContentKey IconContentKey;

		[JsonIgnore]
		public SpriteContentKey ButtonIconKeyOff;

		[JsonIgnore]
		public SpriteContentKey ButtonIconKeyDisabled;

		[JsonIgnore]
		public SpriteContentKey ButtonIconKeyOn;

		[JsonIgnore]
		public SpriteContentKey ButtonIconKeyInactive;

		[JsonIgnore]
		public PrefabContentKey TubeAssetContentKey;

		[JsonProperty]
		[SerializeField]
		public string DisplayNameToken;

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
