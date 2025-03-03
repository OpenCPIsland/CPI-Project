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
	public class DecalPanelSingleItems : MonoBehaviour
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

		private List<DecalDefinition> displayedDecals;

		private void Start()
		{
			displayedDecals = new List<DecalDefinition>();
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
			displayedDecals = buildVisibleDecalList(Service.Get<CatalogServiceProxy>().IsCatalogThemeActive());
			setupPooledScrollRect();
		}

		private List<DecalDefinition> buildVisibleDecalList(bool isFiltered = true)
		{
			List<DecalDefinition> list = new List<DecalDefinition>();
			Dictionary<int, DecalDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
			if (dictionary == null)
			{
				Log.LogError(this, "Unable to retrieve the decal definitions.");
				return new List<DecalDefinition>();
			}
			if (isFiltered)
			{
				TagDefinition[] filterTags = null;
				CatalogThemeDefinition catalogTheme = Service.Get<CatalogServiceProxy>().GetCatalogTheme();
				if (catalogTheme != null)
				{
					filterTags = catalogTheme.DecalTags;
				}
				if (filterTags != null && filterTags.Length > 0)
				{
					foreach (KeyValuePair<int, DecalDefinition> item in dictionary)
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
			foreach (KeyValuePair<int, DecalDefinition> item2 in dictionary)
			{
				list.Add(item2.Value);
			}
			return list;
		}

		private void setupPooledScrollRect()
		{
			PooledScrollRect.gameObject.SetActive(true);
			PooledScrollRect.ObjectAdded += onObjectAdded;
			PooledScrollRect.Init(displayedDecals.Count, itemPrefab);
		}

		private void onObjectAdded(RectTransform item, int index)
		{
			CustomizationButton component = item.GetComponent<CustomizationButton>();
			DecalDefinition decalDefinition = displayedDecals[index];
			component.gameObject.name = decalDefinition.AssetName + "_button";
			RectTransform rectTransform = component.transform as RectTransform;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			bool canDrag = isLocalPlayerMember;
			if (!isLocalPlayerMember)
			{
				canDrag = !decalDefinition.IsMemberOnly;
			}
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				canDrag = true;
			}
			Texture2DContentKey decalPath = EquipmentPathUtil.GetDecalPath(decalDefinition.AssetName);
			component.Init(decalPath, BreadcrumbType, decalDefinition.Id, canDrag);
			AccessibilitySettings component2 = component.GetComponent<AccessibilitySettings>();
			if (component2 != null)
			{
				component2.CustomToken = decalDefinition.AssetName;
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
