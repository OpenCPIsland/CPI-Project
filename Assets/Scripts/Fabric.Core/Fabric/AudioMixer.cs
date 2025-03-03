using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/AudioMixer")]
	public class AudioMixer : MonoBehaviour, IEventListener
	{
		[SerializeField]
		public List<UnityEngine.Audio.AudioMixer> _audioMixers = new List<UnityEngine.Audio.AudioMixer>();

		[NonSerialized]
		public bool _destroy;

		bool IEventListener.IsDestroyed
		{
			get
			{
				return this == null;
			}
		}

		private void Awake()
		{
			EventManager.Instance.RegisterListener(this, "AudioMixer");
		}

		EventStatus IEventListener.Process(Event zEvent)
		{
			if (_audioMixers.Count == 0)
			{
				return EventStatus.Not_Handled;
			}
			switch (zEvent.EventAction)
			{
			case EventAction.LoadAudioMixer:
			{
				UnityEngine.Audio.AudioMixer audioMixer3 = Resources.Load((string)zEvent._parameter) as UnityEngine.Audio.AudioMixer;
				if (audioMixer3 != null)
				{
					_audioMixers.Add(audioMixer3);
				}
				break;
			}
			case EventAction.UnloadAudioMixer:
			{
				UnityEngine.Audio.AudioMixer audioMixer2 = _audioMixers.Find((UnityEngine.Audio.AudioMixer x) => x.name.Contains((string)zEvent._parameter));
				if (audioMixer2 != null)
				{
					_audioMixers.Remove(audioMixer2);
					Resources.UnloadAsset(audioMixer2);
				}
				break;
			}
			case EventAction.TransitionToSnapshot:
			{
				TransitionToSnapshotData transitionToSnapshotData = (TransitionToSnapshotData)zEvent._parameter;
				if (transitionToSnapshotData == null)
				{
					break;
				}
				for (int i = 0; i < _audioMixers.Count; i++)
				{
					UnityEngine.Audio.AudioMixer audioMixer = _audioMixers[i];
					if (audioMixer != null)
					{
						AudioMixerSnapshot audioMixerSnapshot = audioMixer.FindSnapshot(transitionToSnapshotData._snapshot);
						if (audioMixerSnapshot != null)
						{
							audioMixerSnapshot.TransitionTo(transitionToSnapshotData._timeToReach);
						}
					}
				}
				break;
			}
			}
			return EventStatus.Handled;
		}

		bool IEventListener.IsActive(GameObject parentGameObject)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(string eventName, List<EventListener> listeners)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(int eventID, List<EventListener> listeners)
		{
			return false;
		}

		bool IEventListener.GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo)
		{
			return false;
		}
	}
}
