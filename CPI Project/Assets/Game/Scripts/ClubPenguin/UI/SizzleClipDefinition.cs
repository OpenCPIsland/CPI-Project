using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Props;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Chat/SizzleClip")]
	public class SizzleClipDefinition : StaticGameDataDefinition, IMemberLocked
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public string AnimationHash;

		public float IconAnimationTime;

		public PropDefinition Prop;

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
