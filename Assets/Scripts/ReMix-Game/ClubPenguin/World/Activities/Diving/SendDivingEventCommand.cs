using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.World.Activities.Diving
{
	[ActionCategory("Activities")]
	public class SendDivingEventCommand : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public DivingEvents.EventTypes EventType;

		public FsmFloat FloatVariable;

		public bool EveryFrame;

		public override void Reset()
		{
			EventType = DivingEvents.EventTypes.AirSupplyUpdated;
			FloatVariable = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			sendEventData();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		private void sendEventData()
		{
			switch (EventType)
			{
			case DivingEvents.EventTypes.AirSupplyUpdated:
				Service.Get<EventDispatcher>().DispatchEvent(new DivingEvents.AirSupplyUpdated(FloatVariable.Value));
				break;
			case DivingEvents.EventTypes.DepthUpdated:
				Service.Get<EventDispatcher>().DispatchEvent(new DivingEvents.DepthUpdated((int)FloatVariable.Value));
				break;
			case DivingEvents.EventTypes.ShowHud:
				Service.Get<EventDispatcher>().DispatchEvent(default(DivingEvents.ShowHud));
				break;
			case DivingEvents.EventTypes.HideHud:
				Service.Get<EventDispatcher>().DispatchEvent(default(DivingEvents.HideHud));
				break;
			default:
				throw new UnityException("Unknown event to dispatch");
			}
		}

		public override void OnUpdate()
		{
			sendEventData();
		}
	}
}
