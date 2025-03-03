using Fabric;
using UnityEngine;

namespace ClubPenguin
{
	public static class SoundUtils
	{
		public static void PlayAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
				}
			}
		}

		public static void StopAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound);
				}
			}
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
	}
}
