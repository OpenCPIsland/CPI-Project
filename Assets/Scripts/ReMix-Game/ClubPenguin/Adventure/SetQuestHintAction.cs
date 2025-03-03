using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SetQuestHintAction : FsmStateAction
	{
		[LocalizationToken]
		public string i18nHintText;

		public string MascotName;

		public QuestHintWaitType WaitType;

		public float WaitTime;

		public bool Repeat = true;

		public override void OnEnter()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(i18nHintText);
			QuestHint hintData = new QuestHint(tokenTranslation, MascotName, WaitType, WaitTime, Repeat);
			Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SetQuestHint(hintData));
			Finish();
		}
	}
}
