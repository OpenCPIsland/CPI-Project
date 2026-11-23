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

		private PropUser user;

		private bool equipped;

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
			user = prop.PropUserRef;
			if (user != null)
			{
				user.EPropRetrieved += OnPropRetrieved;
				user.EPropStored += OnPropStored;
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
			if (obj != prop || equipped || !base.isActiveAndEnabled)
			{
				return;
			}
			if (getDispatcher() != null)
			{
				equipped = true;
				Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.Equip));
			}
		}

		private void ReleaseEquip()
		{
			if (equipped && Dispatcher != null)
			{
				Dispatcher.DispatchEvent(new InteractionZoneEvents.InteractivePropEquipEvent(ItemType, InteractiveItemAction.UnEquip));
			}
			equipped = false;
		}

		public void OnDisable()
		{
			ReleaseEquip();
		}

		public void OnDestroy()
		{
			ReleaseEquip();
			if (user != null)
			{
				user.EPropRetrieved -= OnPropRetrieved;
				user.EPropStored -= OnPropStored;
				user = null;
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
