using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Switches
{
	public class PropTypeSwitch : Switch
	{
		public HeldObjectType[] HeldTypes = new HeldObjectType[0];

		private DataEntityHandle localPlayerHandle;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull && dataEntityCollection.HasComponent<PresenceData>(localPlayerHandle))
			{
				dataEntityCollection.GetComponent<HeldObjectsData>(localPlayerHandle).PlayerHeldObjectChanged += onHeldObjectChanged;
			}
			else
			{
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			}
		}

		public void OnDestroy()
		{
			HeldObjectsData component;
			if (localPlayerHandle.IsNull)
			{
				eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			}
			else if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				component.PlayerHeldObjectChanged -= onHeldObjectChanged;
			}
		}

		private bool onLocalPlayerAdded(NetworkControllerEvents.LocalPlayerJoinedRoomEvent evt)
		{
			eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			localPlayerHandle = evt.Handle;
			dataEntityCollection.GetComponent<HeldObjectsData>(localPlayerHandle).PlayerHeldObjectChanged += onHeldObjectChanged;
			return false;
		}

		private void onHeldObjectChanged(DHeldObject obj)
		{
			if (obj == null)
			{
				Change(false);
				return;
			}
			bool onoff = false;
			if (HeldTypes != null)
			{
				for (int i = 0; i < HeldTypes.Length; i++)
				{
					if (obj.ObjectType == HeldTypes[i])
					{
						onoff = true;
						break;
					}
				}
			}
			Change(onoff);
		}
	}
}
