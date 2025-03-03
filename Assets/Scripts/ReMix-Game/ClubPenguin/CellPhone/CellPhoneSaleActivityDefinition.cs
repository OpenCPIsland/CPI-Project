using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.DisneyStore;
using ClubPenguin.Marketplace;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneSaleActivityDefinition : CellPhoneActivityDefinition, ICellPhoneScheduledActivityDefinition
	{
		[Serializable]
		public class MarketPlaceSaleData
		{
			public MarketplaceDefinition MarketPlace;

			public SceneDefinition Scene;

			public Vector3 PositionInZone;
		}

		[StaticGameDataDefinitionId]
		public int Id;

		public ScheduledEventDateDefinition AvailableDates;

		public int DiscountPercentage;

		public MarketPlaceSaleData[] MarketPlaceData;

		public PropDefinition[] Consumables;

		public DisneyStoreItemDefinition[] DisneyStoreItems;

		public PrefabContentKey MarketBannerKey;

		public DateUnityWrapper GetStartingDate()
		{
			return AvailableDates.Dates.StartDate;
		}

		public DateUnityWrapper GetEndingDate()
		{
			return AvailableDates.Dates.EndDate;
		}
	}
}
