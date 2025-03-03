using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Prop))]
	[RequireComponent(typeof(PropCancel))]
	public class InteractiveZonePropEventHandler : MonoBehaviour
	{
		[SerializeField]
		private string ItemType;

		public EventDispatcher Dispatcher;

		private Prop prop;

		private PropCancel propCancel;

		private EventDispatcher getDispatcher()
		{
			if (Dispatcher == null && prop != null && prop.PropUserRef != null)
			{
				CustomDispatcher component = prop.PropUserRef.GetComponent<CustomDispatcher>();
				if (component != null)
				{
					Dispatcher = component.Dispatcher;
				}
			}
			return Dispatcher;
		}

		public void Awake()
		{
			prop = GetComponent<Prop>();
			propCancel = GetComponent<PropCancel>();
		}

		public void Start()
		{
			getDispatcher();
			if (prop.PropUserRef != null)
			{
				prop.PropUserRef.EPropRetrieved += OnPropRetrieved;
				prop.PropUserRef.EPropStored += OnPropStored;
			}
		}

		private void OnPropStored(Prop obj)
		{
			if (prop != null && prop.PropUserRef != null && prop.PropUserRef.Prop != null && prop.PropUserRef.Prop.IsOwnerLocalPlayer)
			{
				Service.Get<iOSHapticFeedback>().TriggerSelectionFeedback();
			}
		}

		private void OnPropRetrieved(Prop obj)
		{
			if (getDispatcher() != null)
			{
				Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.Equip));
			}
		}

		public void OnDestroy()
		{
			if (prop.PropUserRef != null)
			{
				if (getDispatcher() != null)
				{
					Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.UnEquip));
				}
				prop.PropUserRef.EPropRetrieved -= OnPropRetrieved;
				prop.PropUserRef.EPropStored -= OnPropStored;
			}
		}

		public void UnEquipProp()
		{
			if (prop != null && prop.PropUserRef != null && prop.PropUserRef.Prop != null)
			{
				if (prop.PropUserRef.Prop.IsOwnerLocalPlayer)
				{
					Service.Get<iOSHapticFeedback>().TriggerSelectionFeedback();
				}
				propCancel.UnequipProp();
			}
		}
	}
}
