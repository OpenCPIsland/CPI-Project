using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneDetector : MonoBehaviour
	{
		[SerializeField]
		private string ItemType = "Guitar";

		private EventDispatcher dispatcher;

		private Prop prop;

		private PropUser user;

		private bool counted;

		private bool stored;

		private readonly HashSet<Collider> inside = new HashSet<Collider>();

		public void Start()
		{
			prop = GetComponent<Prop>();
			if (prop != null)
			{
				user = prop.PropUserRef;
				if (user != null)
				{
					user.EPropRetrieved += onPropRetrieved;
					user.EPropStored += onPropStored;
				}
			}
			else
			{
				Log.LogError(this, "Failed to attain the Prop component");
			}
		}

		public void onPropStored(Prop obj)
		{
			if (obj != prop)
			{
				return;
			}
			stored = true;
			Release();
		}

		public void onPropRetrieved(Prop obj)
		{
		}

		private void Release()
		{
			if (counted && dispatcher != null)
			{
				dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.UnEquip));
			}
			counted = false;
			dispatcher = null;
		}

		public void OnDisable()
		{
			inside.Clear();
			Release();
		}

		public void OnDestroy()
		{
			inside.Clear();
			Release();
			if (user != null)
			{
				user.EPropRetrieved -= onPropRetrieved;
				user.EPropStored -= onPropStored;
				user = null;
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			InteractiveZoneController componentInParent = other.GetComponentInParent<InteractiveZoneController>();
			if (componentInParent == null)
			{
				return;
			}
			inside.Add(other);
			if (counted || stored || componentInParent.Dispatcher == null)
			{
				return;
			}
			dispatcher = componentInParent.Dispatcher;
			counted = true;
			dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.Equip));
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.GetComponentInParent<InteractiveZoneController>() == null)
			{
				return;
			}
			inside.Remove(other);
			inside.RemoveWhere((Collider c) => c == null);
			if (inside.Count == 0)
			{
				Release();
			}
		}
	}
}
