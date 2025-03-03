using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/Lot")]
	public class LotDefinition : IglooAssetDefinition<string>
	{
		[StaticGameDataDefinitionId]
		[Tooltip("The identifier of the lot. By convention, should match the name of the associated zone.")]
		public string LotName;

		[Tooltip("The ZoneDefinition asset to use when joining a zone with this lot")]
		public ZoneDefinitionKey ZoneDefintion;

		[Tooltip("The image used in the Manage Igloo Popup to show the igloo Lot")]
		public Texture2DContentKey PreviewImageLarge;

		[Tooltip("The maximum number of items allowed in this lot")]
		public int MaxItems;

		public override string GetId()
		{
			return LotName;
		}
	}
}
