using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using UnityEngine;

namespace ClubPenguin.Audio
{
	internal class SceneMusic : MonoBehaviour
	{
		[Serializable]
		public class Snapshot
		{
			public string Name;

			public float TimeToReach;
		}

		public string EventOnEnter;

		public string EventName = "AudioMixer";

		public Snapshot SnapshotOnEnter = new Snapshot();

		public Snapshot SnapshotOnExit = new Snapshot();

		public string PersistToScene;

		private EventDispatcher dispatcher;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			startMusic();
		}

		private void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			}
		}

		private bool onSceneTransitionStart(ClubPenguin.Core.SceneTransitionEvents.TransitionStart evt) 
		{
			if (evt.SceneName != PersistToScene && !string.IsNullOrEmpty(SnapshotOnExit.Name))
			{
				TransitionToSnapshotData transitionToSnapshotData = new TransitionToSnapshotData();
				transitionToSnapshotData._snapshot = SnapshotOnExit.Name;
				transitionToSnapshotData._timeToReach = SnapshotOnExit.TimeToReach;
				EventManager.Instance.PostEvent(EventName, EventAction.TransitionToSnapshot, transitionToSnapshotData);
			}
			return false;
		}

		private void startMusic()
		{
			if (!string.IsNullOrEmpty(EventOnEnter) && !EventManager.Instance.IsEventActive(EventOnEnter, null))
			{
				EventManager.Instance.PostEvent(EventOnEnter, EventAction.PlaySound, null);
				if (!string.IsNullOrEmpty(SnapshotOnEnter.Name))
				{
					TransitionToSnapshotData transitionToSnapshotData = new TransitionToSnapshotData();
					transitionToSnapshotData._snapshot = SnapshotOnEnter.Name;
					transitionToSnapshotData._timeToReach = SnapshotOnEnter.TimeToReach;
					EventManager.Instance.PostEvent(EventName, EventAction.TransitionToSnapshot, transitionToSnapshotData);
				}
			}
		}
	}
}
