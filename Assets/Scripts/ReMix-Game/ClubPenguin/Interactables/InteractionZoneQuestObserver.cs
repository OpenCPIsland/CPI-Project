using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneQuestObserver : InteractionZoneObserver
	{
		protected override bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			if (evt.Collider.gameObject.CompareTag("Player"))
			{
				switch (evt.StateChange)
				{
				case ZoneInteractionStateChange.EnterZone:
					Service.Get<QuestService>().SendEvent("EnteredInteractionZone");
					break;
				case ZoneInteractionStateChange.ExitZone:
					Service.Get<QuestService>().SendEvent("LeftInteractionZone");
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			return false;
		}
	}
}
