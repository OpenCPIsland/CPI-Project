using ClubPenguin.Adventure;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestsAdventuresStateHandler : MonoBehaviour
	{
		private enum QuestsAdventuresState
		{
			Log,
			LogQuests
		}

		public const string EXIT_BUTTON_ID = "QuestLogAdventureExit";

		private MainNavStateHandler mainNav;

		private bool firstTimeOpened = true;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<ButtonEvents.ClickEvent>(onCloseButtonClicked);
			mainNav = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponentInChildren<MainNavStateHandler>(true);
		}

		public void OnStateChanged(string newStateString)
		{
			QuestsScreenController componentInParent = GetComponentInParent<QuestsScreenController>();
			Mascot mascot = Service.Get<MascotService>().ActiveMascot;
			QuestsAdventuresState questsAdventuresState = (QuestsAdventuresState)Enum.Parse(typeof(QuestsAdventuresState), newStateString);
			MascotDefinition.QuestChapterData currentChapterData = componentInParent.CurrentChapterData;
			if (mascot == null)
			{
				string currentMascotID = componentInParent.CurrentMascotID;
				if (!string.IsNullOrEmpty(currentMascotID))
				{
					mascot = Service.Get<MascotService>().GetMascot(currentMascotID);
				}
				mainNav.SetBackButtonVisible(true);
			}
			else if (questsAdventuresState == QuestsAdventuresState.Log)
			{
				if (firstTimeOpened)
				{
					QuestDefinition nextAvailableQuest = mascot.GetNextAvailableQuest();
					int num = 0;
					if (nextAvailableQuest != null)
					{
						num = nextAvailableQuest.ChapterNumber - 1;
					}
					else
					{
						for (int num2 = mascot.Definition.ChapterData.Length - 1; num2 >= 0; num2--)
						{
							if (!mascot.Definition.ChapterData[num2].IsPreviewChapter)
							{
								num = num2;
								break;
							}
						}
					}
					componentInParent.CurrentChapterData = mascot.Definition.ChapterData[num];
					firstTimeOpened = false;
					GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
					StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
					component.SendEvent(new ExternalEvent("ScreenQuestsAdventures", "logQuests"));
					return;
				}
				mainNav.SetBackButtonVisible(false);
			}
			else
			{
				mainNav.SetBackButtonVisible(true);
			}
			Localizer localizer = Service.Get<Localizer>();
			switch (questsAdventuresState)
			{
			case QuestsAdventuresState.Log:
				if (mascot != null)
				{
					mainNav.SetTitleText(localizer.GetTokenTranslation(mascot.Definition.i18nAdventureLogTitleText));
				}
				break;
			case QuestsAdventuresState.LogQuests:
				mainNav.SetTitleText(localizer.GetTokenTranslation(currentChapterData.Name));
				break;
			}
			firstTimeOpened = false;
		}

		private void closeQuests()
		{
			DButton dButton = new DButton();
			dButton.Id = "QuestLogAdventureExit";
			Service.Get<EventDispatcher>().DispatchEvent(new ButtonEvents.ClickEvent(dButton));
		}

		private bool onCloseButtonClicked(ButtonEvents.ClickEvent evt)
		{
			if (evt.ButtonData.Id == "MainNavControl" && Service.Get<MascotService>().ActiveMascot != null)
			{
				closeQuests();
				return true;
			}
			return false;
		}

		private void OnDestroy()
		{
			if (Service.Get<MascotService>().ActiveMascot != null)
			{
				closeQuests();
			}
			Service.Get<EventDispatcher>().RemoveListener<ButtonEvents.ClickEvent>(onCloseButtonClicked);
		}
	}
}
