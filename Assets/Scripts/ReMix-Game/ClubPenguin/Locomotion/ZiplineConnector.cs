using ClubPenguin.Actions;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class ZiplineConnector : MonoBehaviour
	{
		private Dictionary<GameObject, Transform> ziplineEndpoints;

		private CPDataEntityCollection dataEntityCollection;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			SceneRefs.SetZiplineConnector(this);
			ziplineEndpoints = null;
		}

		public void ConnectIfNeeded(DataEntityHandle handle, LocomotionState state)
		{
			GameObjectReferenceData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component) && isInteractingWithZiplineActionGraph(component.GameObject))
			{
				return;
			}
			LocomotionData component2 = dataEntityCollection.GetComponent<LocomotionData>(handle);
			if (!component2.LocomotionStateIsInitialized && state == LocomotionState.Zipline && dataEntityCollection.TryGetComponent(handle, out component))
			{
				GameObject gameObject = component.GameObject;
				if (gameObject != null && !gameObject.IsDestroyed())
				{
					connectPlayerObject(gameObject);
				}
			}
		}

		private void connectPlayerObject(GameObject player)
		{
			if (!isInteractingWithZiplineActionGraph(player))
			{
				Transform component = player.GetComponent<Transform>();
				Dictionary<GameObject, Transform> dictionary = getZiplineEndpoints();
				float num = 10f;
				GameObject gameObject = null;
				foreach (KeyValuePair<GameObject, Transform> item in dictionary)
				{
					float num2 = Mathf.Abs(Vector3.Distance(component.position, item.Value.position));
					if (num2 < num)
					{
						num = num2;
						gameObject = item.Key;
					}
				}
				if (gameObject != null)
				{
					component.position = gameObject.GetComponent<LocomoteToAction>().Waypoints[0].position;
					CoroutineRunner.Start(startSequence(player.gameObject, gameObject), typeof(ZiplineController), "Remote player zipline loading delay");
				}
			}
		}

		private bool isInteractingWithZiplineActionGraph(GameObject player)
		{
			if (player.IsDestroyed())
			{
				return false;
			}
			LocomotionTracker component = player.GetComponent<LocomotionTracker>();
			if (component == null)
			{
				return false;
			}
			if (component.IsCurrentControllerOfType<ZiplineController>())
			{
				return true;
			}
			GameObject trigger = SceneRefs.ActionSequencer.GetTrigger(player);
			if (trigger == null)
			{
				return false;
			}
			return trigger.GetComponent<SetZiplineLocomotionAction>() != null;
		}

		private IEnumerator startSequence(GameObject player, GameObject zipline)
		{
			yield return null;
			SceneRefs.ActionSequencer.StartSequence(player.gameObject, zipline);
		}

		private Dictionary<GameObject, Transform> getZiplineEndpoints()
		{
			if (ziplineEndpoints == null)
			{
				SetZiplineLocomotionAction[] array = Object.FindObjectsOfType<SetZiplineLocomotionAction>();
				ziplineEndpoints = new Dictionary<GameObject, Transform>(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					WarpTunnelAction component = array[i].gameObject.GetComponent<WarpTunnelAction>();
					Transform[] componentsInChildren = component.Waypoints.GetComponentsInChildren<Transform>(true);
					ziplineEndpoints.Add(component.gameObject, componentsInChildren[componentsInChildren.Length - 1]);
				}
			}
			return ziplineEndpoints;
		}
	}
}
