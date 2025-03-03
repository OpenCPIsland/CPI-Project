using ClubPenguin.Adventure;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestsDetailsStateHandler : MonoBehaviour
	{
		private enum QuestsDetailsState
		{
			Inventory,
			Pause
		}

		[LocalizationToken(InitialText = "{0}'s Adventures")]
		private const string INVENTORY_TITLE_TOKEN = "Quest.Text.QuestDetailsStateHandler.INVENTORY_TITLE";

		[LocalizationToken(InitialText = "{0}'s Adventures")]
		private const string PAUSE_TITLE_TOKEN = "Quest.Text.QuestDetailsStateHandler.PAUSE_TITLE";

		public GameObject TitleBar;

		public Text TitleText;

		private QuestsDetailsState currentState;

		public void OnStateChanged(string newStateString)
		{
			Mascot mascot = Service.Get<QuestService>().ActiveQuest.Mascot;
			QuestsDetailsState questsDetailsState = (QuestsDetailsState)Enum.Parse(typeof(QuestsDetailsState), newStateString);
			Localizer localizer = Service.Get<Localizer>();
			switch (questsDetailsState)
			{
			case QuestsDetailsState.Inventory:
				TitleText.text = localizer.GetTokenTranslationFormatted("Quest.Text.QuestDetailsStateHandler.INVENTORY_TITLE", mascot.Definition.i18nDisplayTitle);
				break;
			case QuestsDetailsState.Pause:
				TitleText.text = localizer.GetTokenTranslationFormatted("Quest.Text.QuestDetailsStateHandler.PAUSE_TITLE", mascot.Definition.i18nDisplayTitle);
				break;
			}
			currentState = questsDetailsState;
		}

		public void OnBackButtonPressed()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
			if (currentState == QuestsDetailsState.Inventory)
			{
				component.SendEvent(new ExternalEvent("ScreenQuestsDetails", "pause"));
			}
			else
			{
				component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
			}
		}
	}
}
