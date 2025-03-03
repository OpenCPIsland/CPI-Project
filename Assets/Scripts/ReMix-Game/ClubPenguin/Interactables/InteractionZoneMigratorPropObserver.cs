using Fabric;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneMigratorPropObserver : InteractionZoneObserver
	{
		public string[] FabricEventNames;

		public string[] InstrumentNames;

		private readonly Dictionary<string, int> instrumentPlayCountDictionary = new Dictionary<string, int>();

		public new void Start()
		{
			base.Start();
			dispatcher.AddListener<InteractionZoneEvents.InteractivePropEquipEvent>(OnInteractiveItemLoop);
		}

		private bool OnInteractiveItemLoop(InteractionZoneEvents.InteractivePropEquipEvent evt)
		{
			EnsureTypeInDictionary(evt.ItemType);
			switch (evt.ItemAction)
			{
			case InteractiveItemAction.Equip:
				StartLoop(evt.ItemType);
				break;
			case InteractiveItemAction.UnEquip:
				StopLoop(evt.ItemType);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			dispatcher.DispatchEvent(new InteractionZoneEvents.IteractiveItemCountEvent(instrumentPlayCountDictionary));
			return false;
		}

		private void EnsureTypeInDictionary(string itemType)
		{
			if (!instrumentPlayCountDictionary.ContainsKey(itemType))
			{
				instrumentPlayCountDictionary[itemType] = 0;
			}
		}

		private void StartLoop(string itemType)
		{
			instrumentPlayCountDictionary[itemType]++;
			int num = FindEventIndex(itemType);
			if (num > -1)
			{
				EventManager.Instance.PostEvent(FabricEventNames[num], EventAction.SetVolume, 1f, null);
			}
		}

		private void StopLoop(string itemType)
		{
			int num = instrumentPlayCountDictionary[itemType] - 1;
			if (num <= 0)
			{
				num = 0;
				int num2 = FindEventIndex(itemType);
				if (num2 > -1)
				{
					EventManager.Instance.PostEvent(FabricEventNames[num2], EventAction.SetVolume, 0f, null);
				}
			}
			instrumentPlayCountDictionary[itemType] = num;
		}

		protected override bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			switch (evt.StateChange)
			{
			case ZoneInteractionStateChange.ExitZone:
				OnPenguinExitInteractionZone(evt);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case ZoneInteractionStateChange.EnterZone:
				break;
			}
			return false;
		}

		private void OnPenguinExitInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
		}

		public new void OnDestroy()
		{
			base.OnDestroy();
			dispatcher.RemoveListener<InteractionZoneEvents.InteractivePropEquipEvent>(OnInteractiveItemLoop);
			instrumentPlayCountDictionary.Clear();
		}

		private int FindEventIndex(string instrumentName)
		{
			if (InstrumentNames != null)
			{
				for (int i = 0; i < InstrumentNames.Length; i++)
				{
					if (InstrumentNames[i] == instrumentName)
					{
						return i;
					}
				}
			}
			return -1;
		}
	}
}
