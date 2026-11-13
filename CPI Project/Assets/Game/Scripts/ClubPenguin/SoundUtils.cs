using Fabric;
using UnityEngine;

namespace ClubPenguin
{
	public static class SoundUtils
	{
		private const float DefaultProximityDistance = 20f;

		public static void PlayAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			PostAudioEvent(audioEventName, EventAction.PlaySound, null, anchorObj);
		}

		public static void StopAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			PostAudioEvent(audioEventName, EventAction.StopSound, null, anchorObj);
		}

		public static void AudioSetSwitchEvent(string eventName, string childComponentName, GameObject go = null)
		{
			AudioEvent(eventName, EventAction.SetSwitch, childComponentName, go);
		}

		public static void AudioEvent(string eventName, EventAction fabricEvent, string childComponentName, GameObject go = null)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				if (go == null)
				{
					EventManager.Instance.PostEvent(eventName, fabricEvent, childComponentName);
				}
				else
				{
					EventManager.Instance.PostEvent(eventName, fabricEvent, childComponentName, go);
				}
			}
		}

		public static bool PostAudioEvent(string audioEventName, EventAction fabricEvent, object parameter = null, GameObject anchorObj = null, float maxDistance = DefaultProximityDistance)
		{
			if (string.IsNullOrEmpty(audioEventName) || !ShouldPlayAtListenerDistance(anchorObj, maxDistance))
			{
				return false;
			}

			if (parameter == null)
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, fabricEvent, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, fabricEvent);
				}
			}
			else if (anchorObj != null)
			{
				EventManager.Instance.PostEvent(audioEventName, fabricEvent, parameter, anchorObj);
			}
			else
			{
				EventManager.Instance.PostEvent(audioEventName, fabricEvent, parameter);
			}

			return true;
		}

		private static bool ShouldPlayAtListenerDistance(GameObject anchorObj, float maxDistance)
		{
			if (anchorObj == null || maxDistance < 0f)
			{
				return true;
			}

			var listener = UnityEngine.Object.FindAnyObjectByType<AudioListener>();
			var listenerTransform = (listener != null) ? listener.transform : null;
			if (listenerTransform == null)
			{
				return true;
			}

			if (anchorObj == listenerTransform.gameObject)
			{
				return true;
			}

			return Vector3.Distance(anchorObj.transform.position, listenerTransform.position) <= maxDistance;
		}
	}
}
