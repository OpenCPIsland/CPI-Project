using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneDanceFloorObserver : InteractionZoneObserver
	{
		[SerializeField]
		private GameObject DanceZonePickupPrefab;

		[Tooltip("The props that this observer cares about and will remove when the zone is disabled.")]
		public PropDefinition[] PropTypesObserved;

		private HashSet<GameObject> PenguinsDancing = new HashSet<GameObject>();

		public void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onZoneTransitionEvent);
		}

		public void OnDisable()
		{
			StopPenguinsDancing();
			Service.Get<EventDispatcher>().RemoveListener<SceneTransitionEvents.TransitionStart>(onZoneTransitionEvent);
		}

		private bool onZoneTransitionEvent(SceneTransitionEvents.TransitionStart evt)
		{
			StopPenguinsDancing();
			return false;
		}

		private void StopPenguinsDancing()
		{
			foreach (GameObject item in PenguinsDancing)
			{
				StopDancingMode(item);
			}
			PenguinsDancing.Clear();
		}

		protected override bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			switch (evt.StateChange)
			{
			case ZoneInteractionStateChange.EnterZone:
				StartDancingMode(evt);
				break;
			case ZoneInteractionStateChange.ExitZone:
				StopDancingMode(evt.Collider.gameObject);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return false;
		}

		private void StartDancingMode(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			PenguinsDancing.Add(evt.Collider.gameObject);
		}

		private void StopDancingMode(GameObject go)
		{
			if (!(go != null))
			{
				return;
			}
			PropUser component = go.GetComponent<PropUser>();
			if (component != null && IsPropObserved(component.Prop))
			{
				PropCancel component2 = component.Prop.gameObject.GetComponent<PropCancel>();
				if (component2 != null)
				{
					component2.UnequipProp(true);
				}
				if (go.CompareTag("Player"))
				{
					Service.Get<PropService>().LocalPlayerStoreProp();
				}
			}
		}

		private bool IsPropObserved(Prop prop)
		{
			if (PropTypesObserved != null && prop != null)
			{
				for (int i = 0; i < PropTypesObserved.Length; i++)
				{
					if (prop.PropId == PropTypesObserved[i].GetNameOnServer() || prop.PropId == PropTypesObserved[i].Id.ToString())
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Update()
		{
		}
	}
}
