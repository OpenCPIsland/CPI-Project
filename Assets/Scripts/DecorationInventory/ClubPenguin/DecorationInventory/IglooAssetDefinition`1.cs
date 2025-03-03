using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	public abstract class IglooAssetDefinition<IdType> : StaticGameDataDefinition, IMemberLocked
	{
		public const int INVALID_ID = -1;

		[LocalizationToken]
		[Tooltip("The name as presented to users")]
		public string Name;

		[Tooltip("The icon loaded in the inventory tray in Edit mode.")]
		public Texture2DContentKey Icon;

		[JsonProperty]
		[SerializeField]
		[Header("Memberbership Lock")]
		private bool isMemberOnly;

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
		}

		public virtual Type GetIdType()
		{
			return typeof(IdType);
		}

		public abstract IdType GetId();
	}
}
