using DevonLocalization.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ObjectiveAction : FsmStateAction
	{
		public string Description;

		public string i18nDescription;

		public bool NotifyPlayer = true;

		public int DefinitionRewardIndex = -1;

		public override void OnEnter()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(i18nDescription);
			Service.Get<QuestService>().ActiveQuest.StartObjective(base.State.Name, tokenTranslation, NotifyPlayer);
			Finish();
		}

		public override void OnExit()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(i18nDescription);
			if (base.Fsm.IsSwitchingState)
			{
				Service.Get<QuestService>().ActiveQuest.CompleteObjective(base.State.Name, tokenTranslation);
			}
		}
	}
}
