using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Chat
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Chat/Emote")]
	public class EmoteDefinition : StaticGameDataDefinition, IMemberLocked
	{
		public enum ECategory
		{
			FACES,
			MASCOTS,
			FOOD,
			ANIMALS,
			THINGS,
			MISC
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MyECategoryComparer : IEqualityComparer<ECategory>
		{
			public bool Equals(ECategory x, ECategory y)
			{
				return x == y;
			}

			public int GetHashCode(ECategory obj)
			{
				return (int)obj;
			}
		}

		[StaticGameDataDefinitionId]
		public string Id;

		public int CharacterCode;

		public string Token;

		[Header("Sound event name:")]
		public string Sound = "";

		[JsonProperty]
		[SerializeField]
		private bool isMemberOnly = false;

		public ECategory Category = ECategory.MISC;

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
		}
	}
}
