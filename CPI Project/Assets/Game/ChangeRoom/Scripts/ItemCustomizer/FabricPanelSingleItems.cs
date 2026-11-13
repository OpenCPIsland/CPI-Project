using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class FabricPanelSingleItems : MonoBehaviour
	{
		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		public PooledCellsScrollRect PooledScrollRect;

		public PrefabContentKey ItemContentKey;

		private GameObject itemPrefab;

		private ScrollRect scrollRect;

		private bool scrollRectDefaultHorizontal;

		private bool scrollRectDefaultVertical;

		private EventChannel eventChannel;

		private bool isLocalPlayerMember;

		private List<FabricDefinition> displayedFabrics;

		private void Start()
		{
			displayedFabrics = new List<FabricDefinition>();
			setScrollBar();
			setupListeners();
			isLocalPlayerMember = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			Content.LoadAsync(onItemLoaded, ItemContentKey);
		}

		private void setScrollBar()
		{
			scrollRect = PooledScrollRect.GetComponent<ScrollRect>();
			scrollRectDefaultHorizontal = scrollRect.horizontal;
			scrollRectDefaultVertical = scrollRect.vertical;
			Scrollbar scrollbar = base.transform.parent.GetComponentsInChildren<Scrollbar>(true)[0];
			if (scrollbar != null)
			{
				scrollRect.horizontalScrollbar = scrollbar;
				scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
			}
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerUIEvents.EnableScrollRect>(onEnableScrollRect);
			eventChannel.AddListener<CustomizerUIEvents.DisableScrollRect>(onDisableScrollRect);
		}

		private void onItemLoaded(string path, GameObject loadedItemPrefab)
		{
			itemPrefab = loadedItemPrefab;
			displayedFabrics = buildVisibleFabricList(Service.Get<CatalogServiceProxy>().IsCatalogThemeActive());
			setupPooledScrollRect();
		}

		private List<FabricDefinition> buildVisibleFabricList(bool isFiltered = true)
		{
			Dictionary<int, FabricDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			List<FabricDefinition> list = new List<FabricDefinition>();
			if (dictionary == null)
			{
				Log.LogError(this, "Unable to retrieve the fabric definitions.");
				return new List<FabricDefinition>();
			}
			if (isFiltered)
			{
				TagDefinition[] filterTags = null;
				CatalogThemeDefinition catalogTheme = Service.Get<CatalogServiceProxy>().GetCatalogTheme();
				if (catalogTheme != null)
				{
					filterTags = catalogTheme.FabricTags;
				}
				if (filterTags != null && filterTags.Length > 0)
				{
					foreach (KeyValuePair<int, FabricDefinition> item in dictionary)
					{
						List<TagDefinition> list2 = item.Value.Tags.ToList();
						int tagIndex;
						for (tagIndex = 0; tagIndex < filterTags.Length; tagIndex++)
						{
							if (list2.Exists((TagDefinition x) => x.Tag == filterTags[tagIndex].Tag) && !list.Contains(item.Value))
							{
								list.Add(item.Value);
								break;
							}
						}
					}
					return list;
				}
			}
			foreach (KeyValuePair<int, FabricDefinition> item2 in dictionary)
			{
				list.Add(item2.Value);
			}
			return list;
		}

		private void setupPooledScrollRect()
		{
			PooledScrollRect.gameObject.SetActive(true);
			PooledScrollRect.ObjectAdded += onObjectAdded;
			PooledScrollRect.Init(displayedFabrics.Count, itemPrefab);
		}

		private void onObjectAdded(RectTransform item, int index)
		{
			CustomizationButton component = item.GetComponent<CustomizationButton>();
			FabricDefinition fabricDefinition = displayedFabrics[index];
			component.gameObject.name = fabricDefinition.AssetName + "_button";
			RectTransform rectTransform = component.transform as RectTransform;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			bool canDrag = isLocalPlayerMember;
			if (!isLocalPlayerMember)
			{
				canDrag = !fabricDefinition.IsMemberOnly;
			}
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				canDrag = true;
			}
			Texture2DContentKey fabricPath = EquipmentPathUtil.GetFabricPath(fabricDefinition.AssetName);
			component.Init(fabricPath, BreadcrumbType, fabricDefinition.Id, canDrag);
			AccessibilitySettings component2 = component.GetComponent<AccessibilitySettings>();
			if (component2 != null)
			{
				component2.CustomToken = fabricDefinition.AssetName;
			}
		}

		private bool onEnableScrollRect(CustomizerUIEvents.EnableScrollRect evt)
		{
			scrollRect.horizontal = scrollRectDefaultHorizontal;
			scrollRect.vertical = scrollRectDefaultVertical;
			return false;
		}

		private bool onDisableScrollRect(CustomizerUIEvents.DisableScrollRect evt)
		{
			scrollRect.horizontal = false;
			scrollRect.vertical = false;
			return false;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (PooledScrollRect != null)
			{
				PooledScrollRect.ObjectAdded -= onObjectAdded;
			}
		}
	}
}
