using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneDetector : MonoBehaviour
	{
		[SerializeField]
		private string ItemType = "Guitar";

		private EventDispatcher dispatcher;

		private Prop prop;

		public void Start()
		{
			prop = GetComponent<Prop>();
			if (prop != null)
			{
				if (prop.PropUserRef != null)
				{
					prop.PropUserRef.EPropRetrieved += onPropRetrieved;
					prop.PropUserRef.EPropStored += onPropStored;
				}
			}
			else
			{
				Log.LogError(this, "Failed to attain the Prop component");
			}
		}

		public void onPropStored(Prop obj)
		{
			if (dispatcher != null)
			{
				dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.UnEquip));
			}
		}

		public void onPropRetrieved(Prop obj)
		{
		}

		public void OnDestroy()
		{
		}

		public void OnTriggerEnter(Collider other)
		{
			InteractiveZoneController componentInParent = other.GetComponentInParent<InteractiveZoneController>();
			if (componentInParent != null)
			{
				dispatcher = componentInParent.Dispatcher;
				if (dispatcher != null)
				{
					dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.Equip));
				}
			}
		}

		public void OnTriggerExit(Collider other)
		{
			InteractiveZoneController componentInParent = other.GetComponentInParent<InteractiveZoneController>();
			if (componentInParent != null)
			{
				if (dispatcher != null)
				{
					dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.UnEquip));
				}
				dispatcher = null;
			}
		}
	}
}
