using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Marketplace
{
	[Serializable]
	[CreateAssetMenu]
	public class MarketplaceDefinition : StaticGameDataDefinition
	{
		public string Name;

		[LocalizationToken]
		[Tooltip("The name that is displayed on the marketplace screen")]
		public string DisplayName;

		[Tooltip("The name that is displayed on the list of the items")]
		[LocalizationToken]
		public string ItemListDisplayName;

		[Tooltip("The name that is used in the asset paths")]
		public string NameInAssets;

		[Tooltip("Hex value of the text color for the items")]
		public string TextColorHex;

		[LocalizationToken]
		[Tooltip("Name of the marketplace used in descriptions")]
		public string DescriptionName;

		[LocalizationToken]
		[Tooltip("Name of the location the marketplace is in, used in descriptions")]
		public string LocationName;

		[Tooltip("The ad text for this maketplace")]
		[LocalizationToken]
		public string AdText;

		[LocalizationToken]
		[Tooltip("The ad popup description text for this maketplace")]
		public string AdDescriptionText;

		[Tooltip("The item to be shown in the marketplace ad")]
		public PropDefinition AdItem;

		[Tooltip("The marketplace that the ad item is located at")]
		public MarketplaceDefinition AdMarket;

		[Tooltip("An optional panel that appears above the list of the items")]
		public PrefabContentKey TopPanel;

		[Tooltip("Only the following list of consumables are shown in this marketplace")]
		public PropDefinition[] ShowOnlyTheseItems;

		[Tooltip("A list of special consumables that are allowed in this marketplace")]
		public PropDefinition[] SpecialItems;

		[Tooltip("If true, show items that are not set as HasSpecialMarket")]
		public bool ShowDefaultItems = true;
	}
}
