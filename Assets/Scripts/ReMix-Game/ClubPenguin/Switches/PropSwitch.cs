using ClubPenguin.Core;
using ClubPenguin.Props;
using ClubPenguin.Tags;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class PropSwitch : Switch
	{
		[Tooltip("The collection of props that enable this switch")]
		public PropDefinition[] Props = new PropDefinition[0];

		public TagMatcher Tags;

		private HashSet<string> propsIdSet;

		private DataEntityHandle localPlayerHandle;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		public override object GetSwitchParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("equippedIds", new List<string>(getPropIds()));
			dictionary.Add("tags", Tags.GetExportParameters());
			return dictionary;
		}

		public override string GetSwitchType()
		{
			return "equipped";
		}

		private HashSet<string> getPropIds()
		{
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < Props.Length; i++)
			{
				if (!(Props[i] == null))
				{
					hashSet.Add(Props[i].GetNameOnServer());
				}
			}
			return hashSet;
		}

		public void Start()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			propsIdSet = getPropIds();
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
			Change(false);
		}

		public void OnDestroy()
		{
			HeldObjectsData component;
			if (DataEntityHandle.IsNullValue(localPlayerHandle))
			{
				if (eventDispatcher != null)
				{
					eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
				}
			}
			else if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component) && component != null)
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
			if (obj != null)
			{
				for (int i = 0; i < Props.Length; i++)
				{
					if (!(Props[i] == null) && Props[i].isDefinition(obj.ObjectId))
					{
						Change(propsIdSet.Contains(obj.ObjectId));
						return;
					}
				}
			}
			Change(false);
		}
	}
}
