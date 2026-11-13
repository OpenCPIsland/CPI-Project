using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestLogChapterItem : MonoBehaviour
	{
		private const string LOCKED_LOCALIZED_TOKEN = "Quest.Chapter.CompletePrevious";

		public Text TitleText;

		public Text LockedText;

		public GameObject AvailablePanel;

		public GameObject LockedPanel;

		public NotificationBreadcrumb Breadcrumb;

		public FeatureLabelBreadcrumb FeatureLabel;

		private MascotDefinition.QuestChapterData chapterData;

		private MascotDefinition mascotData;

		public void LoadChapterData(MascotDefinition.QuestChapterData data, MascotDefinition mascotData)
		{
			chapterData = data;
			this.mascotData = mascotData;
			setBreadcrumbId();
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation(chapterData.Name);
			setAvailability(isChapterAvailable(chapterData.Number));
			if (FeatureLabel != null)
			{
				FeatureLabel.TypeId = string.Format("{0}{1}", mascotData.name, chapterData.Number);
				FeatureLabel.SetBreadcrumbVisibility();
			}
		}

		public void OnOpenButtonPressed()
		{
			GetComponentInParent<QuestsScreenController>().CurrentChapterData = chapterData;
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent("ScreenQuestsAdventures", "logQuests"));
			removeBreadcrumb();
		}

		private bool isChapterAvailable(int chapterNum)
		{
			bool result = true;
			bool flag = false;
			MascotService mascotService = Service.Get<MascotService>();
			Mascot mascot = mascotService.GetMascot(mascotData.name);
			for (int i = 0; i < mascot.KnownQuests.Length; i++)
			{
				QuestDefinition questDefinition = mascot.KnownQuests[i];
				if (questDefinition.ChapterNumber != chapterNum || questDefinition.QuestNumber != 1)
				{
					continue;
				}
				if (questDefinition.CompletedQuestRequirement.Length == 0)
				{
					flag = true;
					continue;
				}
				for (int j = 0; j < questDefinition.CompletedQuestRequirement.Length; j++)
				{
					QuestDefinition questDefinition2 = questDefinition.CompletedQuestRequirement[j];
					mascot = mascotService.GetMascot(questDefinition2.Mascot.name);
					foreach (Quest availableQuest in mascot.AvailableQuests)
					{
						if (availableQuest.Definition.name == questDefinition2.name)
						{
							flag = true;
							if (availableQuest.TimesCompleted == 0)
							{
								result = false;
								break;
							}
						}
					}
				}
			}
			if (!flag)
			{
				result = false;
			}
			return result;
		}

		private void setAvailability(bool isAvailable)
		{
			if (AvailablePanel != null)
			{
				AvailablePanel.SetActive(isAvailable);
			}
			if (LockedPanel != null)
			{
				LockedPanel.SetActive(!isAvailable);
			}
			if (LockedText != null && !isAvailable)
			{
				LockedText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("Quest.Chapter.CompletePrevious"), (chapterData.Number - 1).ToString());
			}
		}

		private void setBreadcrumbId()
		{
			Breadcrumb.SetBreadcrumbId(string.Format("{0}{1}", mascotData.AbbreviatedName, chapterData.Number));
		}

		private void removeBreadcrumb()
		{
			Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(string.Format("{0}{1}", mascotData.AbbreviatedName, chapterData.Number));
		}
	}
}
