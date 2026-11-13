using ClubPenguin.Net;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;

namespace ClubPenguin.ScheduledWorldObjects
{
	[Tooltip("Helps to synchronize the geyser timing by checking the active state time against and waiting up to a maximum delay. Use with an FSM attached to a GameObject that is a child of a GameObject with a ScheduledWorldObjectController. Uses a StatefulWorldObjectMonobehaviour to determine when the last active update was made. Based on Wait. ")]
	[ActionCategory("Utilities")]
	public class FastForwardStateAction : Wait
	{
		[Tooltip("Abandon event for when the wait time is too short and the time of oppertunity has passed")]
		public FsmEvent AbandonEvent;

		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				StatefulWorldObjectMonobehaviour component = base.Owner.GetComponent<StatefulWorldObjectMonobehaviour>();
				long gameTimeMilliseconds = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
				if (gameTimeMilliseconds != 0 && component != null && component.StatefulWorldObject != null && component.StatefulWorldObject.Timestamp != 0)
				{
					DateTime d = gameTimeMilliseconds.MsToDateTime();
					TimeSpan timeSpan = d - component.StatefulWorldObject.DateTime;
					time.Value -= (float)(timeSpan.TotalMilliseconds / 1000.0);
					if (time.Value < 0f)
					{
						Abandon();
					}
					else
					{
						base.OnEnter();
					}
				}
				else
				{
					Abandon();
				}
			}
			else
			{
				Abandon();
			}
		}

		private void Abandon()
		{
			base.Fsm.Event(AbandonEvent);
			Finish();
		}
	}
}
