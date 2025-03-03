using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Task;
using ClubPenguin.Tutorial;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogHomePageController : ACatalogController
	{
		public const string MEMBERSHIP_FLOW_TAG = "Catalog";

		public GameObject BillboardPanel;

		public GameObject GoButton;

		public GameObject SubmittedButton;

		public GameObject Preloader;

		public GameObject CatalogDailyChallengeScrollerPrefab;

		public GameObject ScrollerContainer;

		public Image BackgroundImage;

		public Image TodaysChallengeBackground;

		public Text ThemeTitle;

		public Text ThemeDescription;

		public Text ThemeEndingText;

		public Text TimeText;

		public GameObject ChallengeMemberLock;

		public GameObject ChallengeProgressionLock;

		public Text ChallengeProgressionLockText;

		public Text CoinReward;

		public TutorialDefinitionKey Catalog1TutorialDefinition;

		public TutorialDefinitionKey Catalog2TutorialDefinition;

		private GameObject scrollerGameObject;

		private DateTime endTime;

		private bool isTimerRunning = false;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			CoroutineRunner.StopAllForOwner(this);
		}

		public override void Hide()
		{
			base.Hide();
			ScrollerContainer.SetActive(false);
			destroyScroller();
			SetChallengeVisiblity(false);
			CoroutineRunner.StopAllForOwner(this);
		}

		private void destroyScroller()
		{
			if (scrollerGameObject != null)
			{
				UnityEngine.Object.Destroy(scrollerGameObject);
				scrollerGameObject = null;
			}
		}

		public override void Show()
		{
			base.Show();
			ScrollerContainer.SetActive(true);
			destroyScroller();
			if (Model.State == CatalogState.Homepage)
			{
				scrollerGameObject = UnityEngine.Object.Instantiate(CatalogDailyChallengeScrollerPrefab);
				scrollerGameObject.transform.SetParent(ScrollerContainer.transform, false);
			}
			Preloader.SetActive(true);
			BillboardPanel.SetActive(false);
			CurrentThemeData currentThemeData = Service.Get<CatalogServiceProxy>().currentThemeData;
			bool flag = (currentThemeData.userSubmissionClothingCatalogId != 0) ? true : false;
			GoButton.SetActive(!flag);
			SubmittedButton.SetActive(flag);
			Button component = GoButton.transform.parent.GetComponent<Button>();
			if (component != null)
			{
				component.enabled = false;
			}
			TaskService taskService = Service.Get<TaskService>();
			List<CoinRewardableDefinition> definitions = taskService.ClothingCatalogChallenge.Reward.GetDefinitions<CoinRewardableDefinition>();
			if (definitions != null && definitions.Count > 0)
			{
				if (definitions[0].Amount < 1)
				{
					CoinReward.text = "";
				}
				else
				{
					CoinReward.text = definitions[0].Amount.ToString();
				}
			}
			Service.Get<TutorialManager>().SetTutorial(Catalog1TutorialDefinition.Id, true);
			Service.Get<TutorialManager>().TryStartTutorial(Catalog2TutorialDefinition.Id);
			CoroutineRunner.Start(waitAFrame(), this, "waitAFrame");
		}

		private IEnumerator waitAFrame()
		{
			yield return new WaitForEndOfFrame();
			Service.Get<CatalogServiceProxy>().GetCurrentThemes();
		}

		public void onMemberLockClicked()
		{
			Service.Get<GameStateController>().ShowAccountSystemMembership("Catalog");
		}

		public void SetChallengeVisiblity(bool isActive)
		{
			BillboardPanel.SetActive(isActive);
		}

		private bool onThemesRetrieved(CatalogServiceProxyEvents.ChallengesReponse evt)
		{
			if (Model.State == CatalogState.Homepage)
			{
				BillboardPanel.SetActive(true);
				if (evt.Themes.Count > 0)
				{
					PopulateChallengeData(evt.Themes[0]);
				}
				Preloader.SetActive(false);
			}
			return false;
		}

		private void PopulateChallengeData(CurrentThemeData currentTheme)
		{
			bool flag = (currentTheme.userSubmissionClothingCatalogId != 0) ? true : false;
			GoButton.SetActive(!flag);
			SubmittedButton.SetActive(flag);
			Button component = GoButton.transform.parent.GetComponent<Button>();
			if (component != null)
			{
				component.enabled = !flag;
			}
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentTheme.scheduledThemeChallengeId);
			bool flag2 = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			if (themeByScheduelId == null)
			{
				ChallengeMemberLock.transform.parent.gameObject.SetActive(false);
				return;
			}
			if (flag2)
			{
				ProgressionService progressionService = Service.Get<ProgressionService>();
				int level = progressionService.Level;
				int num = 0;
				Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
				TagDefinition[] templateTags = themeByScheduelId.TemplateTags;
				List<TemplateDefinition> list = new List<TemplateDefinition>();
				foreach (TagDefinition value in templateTags)
				{
					for (int j = 0; j < dictionary.Values.Count; j++)
					{
						TemplateDefinition templateDefinition = dictionary.Values.ToList()[j];
						if (templateDefinition.Tags.Contains(value) && list.IndexOf(templateDefinition) < 0)
						{
							list.Add(templateDefinition);
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					int unlockLevelFromDefinition = progressionService.GetUnlockLevelFromDefinition(list[i], ProgressionUnlockCategory.equipmentTemplates);
					if (unlockLevelFromDefinition < num)
					{
						num = unlockLevelFromDefinition;
					}
				}
				ChallengeProgressionLock.SetActive(false);
				if (num > level)
				{
					ChallengeProgressionLock.SetActive(true);
					if (ChallengeProgressionLockText != null)
					{
						ChallengeProgressionLockText.text = Service.Get<Localizer>().GetTokenTranslation(num.ToString());
					}
				}
			}
			else
			{
				ChallengeMemberLock.SetActive(false);
				if (ChallengeMemberLock != null)
				{
					ChallengeMemberLock.SetActive(true);
				}
			}
			setBillboardTheme(themeByScheduelId);
			DateTime dateTime = Service.Get<ContentSchedulerService>().PresentTime();
			int hours = 24 - dateTime.Hour - 1;
			int minutes = 60 - dateTime.Minute - 1;
			int seconds = 60 - dateTime.Second - 1;
			TimeSpan t = new TimeSpan(hours, minutes, seconds);
			endTime = DateTime.Now + t;
			isTimerRunning = true;
		}

		private void setBillboardTheme(CatalogThemeDefinition theme)
		{
			Color[] colorsByIndex = Service.Get<CatalogServiceProxy>().themeColors.GetColorsByIndex(0);
			GoButton.GetComponent<Image>().color = colorsByIndex[0];
			BackgroundImage.color = colorsByIndex[1];
			TodaysChallengeBackground.color = colorsByIndex[0];
			Localizer localizer = Service.Get<Localizer>();
			ThemeTitle.text = localizer.GetTokenTranslation(theme.Title);
			ThemeDescription.text = localizer.GetTokenTranslation(theme.Description);
		}

		private void Update()
		{
			if (TimeText != null && isTimerRunning)
			{
				TimeSpan timeSpan = endTime - DateTime.Now;
				if (timeSpan.Hours <= 0 && timeSpan.Minutes <= 0 && timeSpan.Seconds <= 0)
				{
					TimeText.text = ThemeEndingText.text;
				}
				else
				{
					TimeText.text = timeSpan.Hours + "hr " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s";
				}
			}
		}

		public void OnAcceptChallengeClicked()
		{
			Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "enter");
			Color[] colorsByIndex = Service.Get<CatalogServiceProxy>().themeColors.GetColorsByIndex(0);
			Model.State = CatalogState.Homepage;
			Service.Get<CatalogServiceProxy>().ActivateCatalogTheme();
			CatalogThemeDefinition catalogTheme = Service.Get<CatalogServiceProxy>().GetCatalogTheme();
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.AcceptChallengeClickedEvent(catalogTheme, colorsByIndex));
		}
	}
}
