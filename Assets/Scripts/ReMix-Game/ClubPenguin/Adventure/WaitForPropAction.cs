using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class WaitForPropAction : FsmStateAction
	{
		[RequiredField]
		public PropDefinition propDefinition;

		public FsmEvent StartRetrieveEvent;

		public FsmEvent RetrievedEvent;

		public FsmEvent StoredEvent;

		public FsmEvent UsedEvent;

		public FsmBool WaitForAction = false;

		private GameObject player;

		public override void OnEnter()
		{
			player = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			PropUser component = player.GetComponent<PropUser>();
			if (component != null)
			{
				addEventListeners();
			}
			else
			{
				Service.Get<EventDispatcher>().AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onPlayerSpawned);
			}
			if (!WaitForAction.Value)
			{
				Finish();
			}
		}

		private bool onPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			addEventListeners();
			return false;
		}

		private void addEventListeners()
		{
			PropUser component = player.GetComponent<PropUser>();
			component.EPropSpawned += onPropStartRetrieve;
			component.EPropRetrieved += onPropRetrieved;
			component.EPropUseStarted += onUsePropStarted;
			component.EPropStored += onPropStored;
		}

		public override void OnExit()
		{
			if (player != null)
			{
				PropUser component = player.GetComponent<PropUser>();
				if (component != null)
				{
					component.EPropSpawned -= onPropStartRetrieve;
					component.EPropRetrieved -= onPropRetrieved;
					component.EPropUseStarted -= onUsePropStarted;
					component.EPropStored -= onPropStored;
				}
			}
			Service.Get<EventDispatcher>().RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onPlayerSpawned);
		}

		private void onPropStartRetrieve(Prop prop)
		{
			if (IsProp(prop))
			{
				base.Fsm.Event(StartRetrieveEvent);
			}
		}

		private void onPropRetrieved(Prop prop)
		{
			if (IsProp(prop))
			{
				base.Fsm.Event(RetrievedEvent);
			}
		}

		private void onUsePropStarted(Prop prop)
		{
			if (IsProp(prop))
			{
				base.Fsm.Event(UsedEvent);
			}
		}

		private void onPropStored(Prop prop)
		{
			if (IsProp(prop))
			{
				base.Fsm.Event(StoredEvent);
			}
		}

		private bool IsProp(Prop prop)
		{
			if (propDefinition == null || prop == null || prop.gameObject.IsDestroyed())
			{
				Finish();
				return false;
			}
			return prop.PropId == propDefinition.GetNameOnServer();
		}
	}
}
