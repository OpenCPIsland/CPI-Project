using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Analytics
{
	[ActionCategory("Analytics")]
	public class StartAnalyticsTimerAction : AnalyticsGameActionAction
	{
		public FsmString TimerID;

		public override void OnEnter()
		{
			Service.Get<ICPSwrveService>().StartTimer(TimerID.Value, Context + "." + Action, Type);
			Finish();
		}
	}
}
