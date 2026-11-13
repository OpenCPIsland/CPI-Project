using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Interaction")]
	public class InteractionWatcher : TaskWatcher
	{
		public string Path;

		private LocomotionEventBroadcaster locoEventBroadcaster;

		public override object GetExportParameters()
		{
			return Path;
		}

		public override string GetWatcherType()
		{
			return "interaction";
		}

		public override void OnActivate()
		{
			base.OnActivate();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				locoEventBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				locoEventBroadcaster.OnInteractionStartedEvent += onInteractionStarted;
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			if (locoEventBroadcaster != null)
			{
				locoEventBroadcaster.OnInteractionStartedEvent -= onInteractionStarted;
			}
		}

		private void onInteractionStarted(GameObject trigger)
		{
			if (trigger.GetPath() == Path)
			{
				taskIncrement();
			}
		}
	}
}
