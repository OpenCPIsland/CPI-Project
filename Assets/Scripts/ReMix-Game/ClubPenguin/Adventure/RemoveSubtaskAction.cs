using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class RemoveSubtaskAction : FsmStateAction
	{
		public FsmString SubtaskId;

		public FsmString i18nContents;

		public override void Reset()
		{
			SubtaskId = new FsmString
			{
				UseVariable = true
			};
			i18nContents = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			string text = i18nContents.IsNone ? SubtaskId.Value : i18nContents.Value;
			if (!string.IsNullOrEmpty(text))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.RemoveSubtaskText(text));
			}
			Finish();
		}
	}
}
