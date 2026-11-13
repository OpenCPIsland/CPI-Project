using Fabric;
using UnityEngine;

namespace ClubPenguin.Audio
{
	internal class AnimToFabricEvents : MonoBehaviour
	{
		public bool Mute;

		public GameObject OverrideSoundSource;

		private GameObject getSoundSource()
		{
			return (OverrideSoundSource != null) ? OverrideSoundSource : base.gameObject;
		}

		public void FabricPlaySound(string name)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.PlaySound, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot PlaySound: {name}", this);
				}
			}
		}

		public void FabricPauseSound(string name)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.PauseSound, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot PauseSound: {name}", this);
				}
			}
		}

		public void FabricUnpauseSound(string name)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.UnpauseSound, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot UnpauseSound: {name}", this);
				}
			}
		}

		public void FabricStopSound(string name)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.StopSound, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot StopSound: {name}", this);
				}
			}
		}

		public void FabricStopAllSound(string name)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.StopAll, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot StopAll: {name}", this);
				}
			}
		}

		public void FabricSetVolume(string name, float volume)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.SetVolume, volume, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot SetVolume: {name}", this);
				}
			}
		}

		public void FabricSetPitch(string name, float pitch)
		{
			if (!Mute)
			{
				if (EventManager.Instance != null)
				{
					EventManager.Instance.PostEvent(name, EventAction.SetPitch, pitch, getSoundSource());
				}
				else
				{
					Debug.LogWarning($"EventManager.Instance is null, cannot SetPitch: {name}", this);
				}
			}
		}
	}
}
