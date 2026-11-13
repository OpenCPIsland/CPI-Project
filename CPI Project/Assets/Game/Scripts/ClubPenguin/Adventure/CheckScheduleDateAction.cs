using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckScheduleDateAction : FsmStateAction
	{
		public int StartYear;

		public int StartMonth;

		public int StartDay;

		public int EndYear;

		public int EndMonth;

		public int EndDay;

		public FsmEvent MatchSuccess;

		public FsmEvent MatchFail;

		public override void OnEnter()
		{
			DateTime dateTime = new DateTime(StartYear, StartMonth, StartDay);
			DateTime dateTime2 = new DateTime(EndYear, EndMonth, EndDay);
			DateTime dateTime3 = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			if (dateTime3.Date >= dateTime.Date && dateTime3.Date <= dateTime2.Date)
			{
				base.Fsm.Event(MatchSuccess);
			}
			else
			{
				base.Fsm.Event(MatchFail);
			}
		}
	}
}
