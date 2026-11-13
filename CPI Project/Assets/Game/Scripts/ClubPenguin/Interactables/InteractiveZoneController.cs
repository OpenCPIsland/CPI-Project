using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InteractiveZoneController : MonoBehaviour
	{
		public HashSet<Collider> CollidersInZone = new HashSet<Collider>();

		public string ZoneId;

		public InteractiveZoneObjectType ObjectType = InteractiveZoneObjectType.AllPlayers;

		public EventDispatcher Dispatcher;

		public void Awake()
		{
			Dispatcher = new EventDispatcher();
		}

		public void OnDestroy()
		{
			CollidersInZone.Clear();
		}

		public void Start()
		{
		}

		public void OnTriggerEnter(Collider other)
		{
			if (IsTypeAllowed(other))
			{
				CustomDispatcher component = other.gameObject.GetComponent<CustomDispatcher>();
				if (component == null && CollidersInZone.Add(other))
				{
					component = other.gameObject.AddComponent<CustomDispatcher>();
					component.Dispatcher = Dispatcher;
					Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractionZoneEvent(ZoneId, other, base.gameObject, ZoneInteractionStateChange.EnterZone));
				}
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (!IsTypeAllowed(other))
			{
				return;
			}
			CustomDispatcher component = other.gameObject.GetComponent<CustomDispatcher>();
			if (!(component != null) || component.Dispatcher != Dispatcher)
			{
				return;
			}
			Prop componentInChildren = other.gameObject.GetComponentInChildren<Prop>();
			if (componentInChildren != null)
			{
				InteractiveZonePropEventHandler component2 = componentInChildren.GetComponent<InteractiveZonePropEventHandler>();
				if (component2 != null)
				{
					component2.UnEquipProp();
				}
			}
			Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractionZoneEvent(ZoneId, other, base.gameObject, ZoneInteractionStateChange.ExitZone));
			UnityEngine.Object.Destroy(component);
			CollidersInZone.Remove(other);
		}

		public void OnTriggerStay(Collider other)
		{
			if (!CollidersInZone.Contains(other))
			{
				OnTriggerEnter(other);
			}
		}

		private bool IsTypeAllowed(Collider other)
		{
			bool result = false;
			switch (ObjectType)
			{
			case InteractiveZoneObjectType.Everything:
				result = true;
				break;
			case InteractiveZoneObjectType.AllPlayers:
				if (other.CompareTag("Player") || other.CompareTag("RemotePlayer") || other.CompareTag("DummyPlayer"))
				{
					result = true;
				}
				break;
			case InteractiveZoneObjectType.RemotePlayersOnly:
				if (other.CompareTag("RemotePlayer"))
				{
					result = true;
				}
				break;
			case InteractiveZoneObjectType.LocalPlayerOnly:
				if (other.CompareTag("Player"))
				{
					result = true;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}
	}
}
