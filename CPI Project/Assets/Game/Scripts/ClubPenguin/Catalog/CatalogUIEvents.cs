using ClubPenguin.Net.Domain;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public static class CatalogUIEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct InvokeBackButtonClick
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ViewInCatalog
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCatalog
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BackButtonClicked
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SelectCategoryButtonAll
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowStatsPage
		{
		}

		public struct SetShopNav
		{
			public List<string> EnabledKeys;

			public SetShopNav(List<string> enabledKeys)
			{
				EnabledKeys = enabledKeys;
			}
		}

		public struct SetCatalogShopNavFilter
		{
			public CatalogShopNavEnum NavCategory;

			public string SubNavCategory;

			public SetCatalogShopNavFilter(CatalogShopNavEnum navCategory, string subNavCategory)
			{
				NavCategory = navCategory;
				SubNavCategory = subNavCategory;
			}
		}

		public struct AcceptChallengeClickedEvent
		{
			public readonly CatalogThemeDefinition Theme;

			public readonly Color[] ThemeColors;

			public AcceptChallengeClickedEvent(CatalogThemeDefinition theme, Color[] themeColors)
			{
				Theme = theme;
				ThemeColors = themeColors;
			}
		}

		public struct ShowItemsForThemeEvent
		{
			public CurrentThemeData Theme;

			public long ClothingCatalogItemId;

			public ShowItemsForThemeEvent(CurrentThemeData theme, long clothingCatalogItemId = -1L)
			{
				Theme = theme;
				ClothingCatalogItemId = clothingCatalogItemId;
			}
		}

		public struct ShopItemClickedEvent
		{
			public int ScrollIndex;

			public int RowIndex;

			public bool IsMemberUnlocked;

			public bool IsAlreadyOwned;

			public CatalogItemData ItemData;

			public ShopItemClickedEvent(int scrollIndex, int rowIndex, bool isMemberUnlocked, bool isAlreadyOwned, CatalogItemData itemData)
			{
				ScrollIndex = scrollIndex;
				RowIndex = rowIndex;
				IsMemberUnlocked = isMemberUnlocked;
				IsAlreadyOwned = isAlreadyOwned;
				ItemData = itemData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BuyPanelCloseButtonClickedEvent
		{
		}

		public struct BuyPanelPurchaseButtonClickedEvent
		{
			public CatalogItemData ItemData;

			public BuyPanelPurchaseButtonClickedEvent(CatalogItemData itemData)
			{
				ItemData = itemData;
			}
		}

		public struct BuyPanelWearItButtonClickedEvent
		{
			public CatalogItemData ItemData;

			public BuyPanelWearItButtonClickedEvent(CatalogItemData itemData)
			{
				ItemData = itemData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BuyPanelLearnMoreButtonClickedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BuyPanelRequiredLevelButtonClickedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SubNavAllButtonClickedEvent
		{
		}

		public struct SubNavCategoryButtonClickedEvent
		{
			public string Category;

			public SubNavCategoryButtonClickedEvent(string category)
			{
				Category = category;
			}
		}
	}
}
