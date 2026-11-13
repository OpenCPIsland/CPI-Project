using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class ChangeObjectiveTextAction : FsmStateAction
	{
		public string Text;

		public string i18nText;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SetObjectiveText(Service.Get<Localizer>().GetTokenTranslation(i18nText)));
			Finish();
		}
	}
}
