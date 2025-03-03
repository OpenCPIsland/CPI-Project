using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public static class InteractionZoneEvents
	{
		public struct InteractionZoneEvent
		{
			public ZoneInteractionStateChange StateChange;

			public string ZoneId;

			public Collider Collider;

			public GameObject ZoneGameObject;

			public InteractionZoneEvent(string zoneId, Collider collider, GameObject zoneGameObject, ZoneInteractionStateChange stateChange)
			{
				ZoneId = zoneId;
				Collider = collider;
				ZoneGameObject = zoneGameObject;
				StateChange = stateChange;
			}
		}

		public struct InteractivePropEquipEvent
		{
			public string ItemType;

			public InteractiveItemAction ItemAction;

			public InteractivePropEquipEvent(string itemType, InteractiveItemAction itemAction)
			{
				ItemType = itemType;
				ItemAction = itemAction;
			}
		}

		public struct IteractiveItemCountEvent
		{
			public Dictionary<string, int> InstrumentPlayCountDictionary;

			public IteractiveItemCountEvent(Dictionary<string, int> instrumentPlayCountDictionary)
			{
				InstrumentPlayCountDictionary = instrumentPlayCountDictionary;
			}
		}
	}
}
