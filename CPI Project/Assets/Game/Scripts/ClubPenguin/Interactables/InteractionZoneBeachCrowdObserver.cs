using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InteractionZoneBeachCrowdObserver : InteractionZoneObserver
	{
		[SerializeField]
		private GameObject BeachCrowdZonePickupPrefab;

		private Dictionary<GameObject, GameObject> PenguinToPlayMakerObjects = new Dictionary<GameObject, GameObject>();

		public void OnDisable()
		{
			RestoreDefaultPlayerUIControls();
		}

		protected override bool OnPlayerTriggerInteractionZone(InteractionZoneEvents.InteractionZoneEvent evt)
		{
			bool isLocalPlayer = evt.Collider.CompareTag("Player");
			switch (evt.StateChange)
			{
			case ZoneInteractionStateChange.EnterZone:
				StartDancingMode(isLocalPlayer, evt);
				break;
			case ZoneInteractionStateChange.ExitZone:
				StopDancingMode(isLocalPlayer, evt);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return false;
		}

		private void StartDancingMode(bool isLocalPlayer, InteractionZoneEvents.InteractionZoneEvent evt)
		{
			AttachPlaymakerFSM(evt.Collider.gameObject);
		}

		private void StopDancingMode(bool isLocalPlayer, InteractionZoneEvents.InteractionZoneEvent evt)
		{
			if (isLocalPlayer)
			{
				RestoreDefaultPlayerUIControls();
			}
			RemovePlaymakerFSM(evt.Collider.gameObject);
			StopPlayerDancing(evt.Collider.gameObject);
		}

		private void StopPlayerDancing(GameObject go)
		{
			if (go != null)
			{
				Animator component = go.GetComponent<Animator>();
				component.SetBool("Dancing", false);
			}
		}

		private void RestoreDefaultPlayerUIControls()
		{
		}

		private GameObject AttachPlaymakerFSM(GameObject go)
		{
			GameObject gameObject = null;
			if (BeachCrowdZonePickupPrefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(BeachCrowdZonePickupPrefab);
				if (gameObject != null)
				{
					PenguinToPlayMakerObjects[go] = gameObject;
					gameObject.transform.SetParent(go.transform);
					gameObject.SetActive(true);
				}
			}
			return gameObject;
		}

		private void RemovePlaymakerFSM(GameObject go)
		{
			if (PenguinToPlayMakerObjects.ContainsKey(go))
			{
				GameObject obj = PenguinToPlayMakerObjects[go];
				UnityEngine.Object.Destroy(obj);
			}
		}
	}
}
