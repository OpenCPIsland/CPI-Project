using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class DestroyQuestMarkerAction : FsmStateAction
	{
		[RequiredField]
		public string MarkerID;

		public override void OnEnter()
		{
			DActionIndicator dActionIndicator = new DActionIndicator();
			dActionIndicator.IndicatorId = MarkerID;
			Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.RemoveActionIndicator(dActionIndicator));
			Finish();
		}
	}
}
