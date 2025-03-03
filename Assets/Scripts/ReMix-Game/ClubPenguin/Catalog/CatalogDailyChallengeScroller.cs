using ClubPenguin.Avatar;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogDailyChallengeScroller : ACatalogController
	{
		public int HorizontalSpacing = 24;

		public float WidthProportionStandalone = 0.66f;

		public int MaxWidthStandalone = 1024;

		public GameObject TitlePanel;

		public GridLayoutGroup GridLayout;

		public GameObject ChallengeItem;

		public Transform scrollRectContent;

		private List<CurrentThemeData> themes;

		private bool isScrollerInitialized;

		private EventChannel eventChannel;

		private void Awake()
		{
			isScrollerInitialized = false;
			TitlePanel.SetActive(false);
			setupListeners();
		}

		private void Start()
		{
			GameObject gameObject = base.gameObject;
			while (gameObject.GetComponent<Canvas>() == null)
			{
				gameObject = gameObject.transform.parent.gameObject;
			}
			float width = gameObject.GetComponent<RectTransform>().rect.width;
			bool flag = Screen.height < Screen.width;
			Vector2 cellSize = new Vector2(width - (float)HorizontalSpacing, GridLayout.cellSize.y);
			if (flag)
			{
				cellSize.x = Math.Min(width * WidthProportionStandalone, MaxWidthStandalone);
			}
			GridLayout.cellSize = cellSize;
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			eventChannel.AddListener<CatalogServiceEvents.CurrentThemesErrorEvent>(onCurrentThemesErrorEvent);
		}

		private bool onThemesRetrieved(CatalogServiceProxyEvents.ChallengesReponse evt)
		{
			if (!isScrollerInitialized)
			{
				themes = evt.Themes;
				for (int i = 0; i < themes.Count; i++)
				{
					addChallengeItem(i);
				}
				TitlePanel.SetActive(true);
				isScrollerInitialized = true;
			}
			return false;
		}

		private bool onCurrentThemesErrorEvent(CatalogServiceEvents.CurrentThemesErrorEvent evt)
		{
			Service.Get<PromptManager>().ShowPrompt("CatalogItemRetrievalErrorPrompt", null);
			return false;
		}

		private void addChallengeItem(int currentThemeDataIndex)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ChallengeItem);
			gameObject.transform.SetParent(scrollRectContent, false);
			CatalogChallengeItem component = gameObject.GetComponent<CatalogChallengeItem>();
			component.SetChallengeTheme(themes[currentThemeDataIndex], currentThemeDataIndex);
			CatalogItemIcon component2 = gameObject.GetComponent<CatalogItemIcon>();
			CurrentThemeData currentThemeData = themes[currentThemeDataIndex];
			if (currentThemeData.mostPopularItem.HasValue)
			{
				DCustomEquipment equipment = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(currentThemeData.mostPopularItem.Value.equipment);
				Color[] colorsByIndex = Service.Get<CatalogServiceProxy>().themeColors.GetColorsByIndex(currentThemeDataIndex);
				AbstractImageBuilder.CallbackToken callbackToken = default(AbstractImageBuilder.CallbackToken);
				callbackToken.Id = ((equipment.Id == 0) ? currentThemeData.mostPopularItem.Value.clothingCatalogItemId : equipment.Id);
				callbackToken.DefinitionId = equipment.DefinitionId;
				base.itemImageBuilder.RequestImage(equipment, component2.SetIcon, callbackToken, Color.clear, colorsByIndex[0]);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
