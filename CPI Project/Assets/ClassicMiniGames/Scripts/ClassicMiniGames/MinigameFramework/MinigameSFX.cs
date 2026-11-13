using UnityEngine;

namespace MinigameFramework
{
	public class MinigameSFX : MonoBehaviour
	{
		public AudioSource AudioTrack;

		private bool m_registered = false;

		public string Name
		{
			get
			{
				return AudioTrack.clip.name;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return AudioTrack.isPlaying;
			}
		}

		public bool IsLooping
		{
			get
			{
				return AudioTrack.loop;
			}
		}

		public void Awake()
		{
			RegisterSFXWithManager();
		}

		public void Start()
		{
			if (!m_registered)
			{
				RegisterSFXWithManager();
			}
		}

		private void RegisterSFXWithManager()
		{
			Minigame active = MinigameManager.GetActive();
			if (AudioTrack != null && active != null)
			{
				active.RegisterSFX(this);
				m_registered = true;
				Stop();
			}
		}

		public void OnDestroy()
		{
			Minigame active = MinigameManager.GetActive();
			if (active != null && m_registered)
			{
				active.UnregisterSFX(this);
			}
		}

		public void Pause()
		{
			AudioTrack.Pause();
		}

		public void Play()
		{
			AudioTrack.Play();
		}

		public void Stop()
		{
			AudioTrack.Stop();
		}

		internal void SetVolume(float sfxVolume)
		{
			if (AudioTrack != null)
			{
				AudioTrack.volume = sfxVolume;
			}
		}
	}
}
