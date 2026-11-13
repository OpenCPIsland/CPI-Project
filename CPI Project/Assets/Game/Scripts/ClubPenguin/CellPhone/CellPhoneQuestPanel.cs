using ClubPenguin.Adventure;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneQuestPanel : MonoBehaviour
	{
		public Text HeaderText;

		public QuestGameObjectSelector QuestTitleText;

		public QuestGameObjectSelector QuestStepText;

		private void Start()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(activeQuest.Mascot.Definition.i18nAdventureLogTitleText);
			QuestTitleText.CurrentSelectedObject.GetComponent<Text>().text = Service.Get<Localizer>().GetTokenTranslation(activeQuest.Definition.Title);
			QuestStepText.CurrentSelectedObject.GetComponent<Text>().text = Service.Get<QuestService>().CurrentObjectiveText;
		}
	}
}
