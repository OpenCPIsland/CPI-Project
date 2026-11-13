using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_MusicManager : MonoBehaviour
	{
		private Dictionary<string, AudioSource> m_availableTracks = new Dictionary<string, AudioSource>();

		private AudioSource m_currentlySelectedTrack = null;

		private bool m_isMusicEnabled = true;

		private bool m_isPaused = false;

		public bool IsMusicEnabled
		{
			get
			{
				return m_isMusicEnabled;
			}
			set
			{
				if (m_isMusicEnabled != value)
				{
					m_isMusicEnabled = value;
					UpdatePlayingState();
				}
			}
		}

		public bool IsPaused
		{
			get
			{
				return m_isPaused;
			}
			set
			{
				if (m_isPaused != value)
				{
					m_isPaused = value;
					UpdatePlayingState();
				}
			}
		}

		private void UpdatePlayingState()
		{
			if (!(m_currentlySelectedTrack != null))
			{
				return;
			}
			if (IsMusicEnabled)
			{
				if (IsPaused)
				{
					m_currentlySelectedTrack.Pause();
				}
				else
				{
					m_currentlySelectedTrack.Play();
				}
			}
			else
			{
				m_currentlySelectedTrack.Stop();
			}
		}

		public void CreateTracks(ICollection<AudioSource> _source)
		{
			foreach (AudioSource item in _source)
			{
				if (m_availableTracks.ContainsKey(item.clip.name))
				{
					DisneyMobile.CoreUnitySystems.Logger.LogDebug(this, "Duplicate track, skipping");
				}
				else
				{
					m_availableTracks.Add(item.clip.name, item);
				}
			}
		}

		public void SelectTrack(string _clipName, bool _restart = false)
		{
			if (!_restart && m_currentlySelectedTrack != null && m_currentlySelectedTrack.clip.name.Equals(_clipName))
			{
				return;
			}
			if (m_availableTracks.ContainsKey(_clipName))
			{
				if (m_currentlySelectedTrack != null && m_currentlySelectedTrack.isPlaying)
				{
					m_currentlySelectedTrack.Stop();
				}
				m_currentlySelectedTrack = m_availableTracks[_clipName];
				UpdatePlayingState();
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempted to change to a track which isn't registered with the music manager");
			}
		}

		public void SetVolume(float volume)
		{
			foreach (AudioSource value in m_availableTracks.Values)
			{
				value.volume = volume;
			}
		}
	}
}
