using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaypointAction : FsmStateAction
	{
		[RequiredField]
		public string WaypointName;

		public string WaypointZone;

		public WaypointType WaypointType;

		public bool ShowOnscreenIndicator = true;

		public float ActivationDistance = 1f;

		private GameObject waypointTarget;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			waypointTarget = GameObject.Find(WaypointName);
			if (waypointTarget != null)
			{
				dispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(waypointTarget.transform, ShowOnscreenIndicator));
				if (WaypointType == WaypointType.COLLIDER)
				{
					dispatcher.AddListener<QuestEvents.QuestWaypointTriggerEntered>(onQuestWaypointTriggerEntered);
				}
				else if (WaypointType == WaypointType.INTERACT)
				{
					GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
					if (localPlayerGameObject != null)
					{
						localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>().OnInteractionStartedEvent += onInteractionStarted;
					}
				}
			}
			else
			{
				ZoneDefinition currentZone = Service.Get<ZoneTransitionService>().CurrentZone;
				string nextZoneInPath = Service.Get<ZonePathing>().GetNextZoneInPath(currentZone.ZoneName, WaypointZone);
				GameObject gameObject = GameObject.Find(nextZoneInPath + "Transition");
				if (gameObject != null)
				{
					dispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(gameObject.transform, ShowOnscreenIndicator));
				}
			}
		}

		public override string ErrorCheck()
		{
			string text = "";
			if (WaypointName.TrimEnd() != WaypointName)
			{
				text += "The waypoint name has trailing whitespace. Please fix! ";
			}
			if (WaypointName.TrimStart() != WaypointName)
			{
				text += "The waypoint name has leading whitespace. Please fix! ";
			}
			return text;
		}

		public override void OnExit()
		{
			if (waypointTarget != null)
			{
				dispatcher.DispatchEvent(default(HudEvents.SetNavigationTarget));
			}
			if (WaypointType == WaypointType.COLLIDER)
			{
				dispatcher.RemoveListener<QuestEvents.QuestWaypointTriggerEntered>(onQuestWaypointTriggerEntered);
			}
			else if (WaypointType == WaypointType.INTERACT)
			{
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				if (localPlayerGameObject != null)
				{
					localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>().OnInteractionStartedEvent -= onInteractionStarted;
				}
			}
		}

		public override void OnUpdate()
		{
			if (waypointTarget != null)
			{
				float magnitude = (base.Owner.transform.position - waypointTarget.transform.position).magnitude;
				if (WaypointType == WaypointType.DISTANCE && magnitude < ActivationDistance)
				{
					Finish();
				}
			}
		}

		private bool onQuestWaypointTriggerEntered(QuestEvents.QuestWaypointTriggerEntered evt)
		{
			if (evt.WaypointName == WaypointName)
			{
				Finish();
			}
			return false;
		}

		private void onInteractionStarted(GameObject trigger)
		{
			if (trigger.name == WaypointName || trigger.transform.parent.name == WaypointName)
			{
				Finish();
			}
		}
	}
}
