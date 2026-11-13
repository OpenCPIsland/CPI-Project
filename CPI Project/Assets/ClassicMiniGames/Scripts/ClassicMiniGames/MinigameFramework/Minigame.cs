using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameFramework
{
	public abstract class Minigame : MonoBehaviour
	{
		[HideInInspector]
		public float MusicVolume;

		[HideInInspector]
		public float SFxVolume;

		protected Camera m_mainCamera = null;

		private bool m_isPauseScreenOpen = false;

		private bool m_isQuiting = false;

		private float m_oldTimeScale;

		private Dictionary<string, List<MinigameSFX>> m_soundEffects;

		private List<MinigameSFX> m_pausedSoundEffects;

		private List<AudioSource> m_registeredMusic;

		private List<AudioSource> m_pausedMusic;

		private bool m_areSFXEnabled = true;

		private bool firstRun = true;

		public Camera MainCamera
		{
			get
			{
				return m_mainCamera;
			}
		}

		public int CoinsEarned
		{
			get;
			set;
		}

		public bool IsPaused
		{
			get;
			private set;
		}

		protected string PasuseScreenName
		{
			get;
			set;
		}

		public bool AreSFXEnabled
		{
			get
			{
				return m_areSFXEnabled;
			}
			set
			{
				if (m_areSFXEnabled != value)
				{
					m_areSFXEnabled = value;
					if (!m_areSFXEnabled)
					{
						StopAllSFX();
						m_pausedSoundEffects.Clear();
					}
				}
			}
		}

		public virtual void Awake()
		{
			PasuseScreenName = "mg_PauseScreen";
			Time.timeScale = 1f;
			if (MinigameManager.Instance == null)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogFatal(this, "You can't run a minigame without a minigame manager! Don't run Minigame scenes directly!");
			}
			IsPaused = false;
			CoinsEarned = 0;
			AreSFXEnabled = true;
			m_registeredMusic = new List<AudioSource>();
			m_pausedMusic = new List<AudioSource>();
			m_soundEffects = new Dictionary<string, List<MinigameSFX>>();
			m_pausedSoundEffects = new List<MinigameSFX>();
			MinigameManager.Instance.OnMiniGameLoaded(this);
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
			ClassicMiniGames.PushBackButtonHandler(PauseGame);
		}

		protected void SetMainCamera(string _cameraName)
		{
			Camera[] allCameras = Camera.allCameras;
			int num = 0;
			Camera camera;
			while (true)
			{
				if (num < allCameras.Length)
				{
					camera = allCameras[num];
					if (camera.gameObject.name == _cameraName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			m_mainCamera = camera;
		}

		public abstract void MinigameUpdate(float _deltaTime);

		public void Update()
		{
			if (firstRun)
			{
				firstRun = false;
				SetVolume(MusicVolume, SFxVolume);
			}
			if (!IsPaused)
			{
				float deltaTime = Time.deltaTime;
				MinigameUpdate(deltaTime);
			}
		}

		public void PauseGame()
		{
			if (!IsPaused)
			{
				m_oldTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				IsPaused = true;
				OnPause();
			}
		}

		public void ResumeGame()
		{
			if (IsPaused)
			{
				IsPaused = false;
				Time.timeScale = m_oldTimeScale;
				OnResume();
			}
		}

		protected virtual void OnPause()
		{
			MinigameScreen minigameScreen = UIManager.Instance.GetTopScreen() as MinigameScreen;
			if (minigameScreen == null || minigameScreen.ShouldShowPauseOver)
			{
				UIManager.Instance.OpenScreen(PasuseScreenName, false, OnPauseScreenPopped, null);
				m_isPauseScreenOpen = true;
			}
			PauseMusic();
			PauseLoopingSFX();
		}

		protected void OnPauseScreenPopped(UIControlBase p_screen)
		{
			m_isPauseScreenOpen = false;
			ResumeGame();
		}

		public virtual void OnApplicationPause(bool p_paused)
		{
			if (p_paused)
			{
				PauseGame();
			}
			else if (!m_isPauseScreenOpen && !m_isQuiting)
			{
				ResumeGame();
			}
		}

		public void PauseMusic()
		{
			foreach (AudioSource item in m_registeredMusic)
			{
				if (item != null && item.isPlaying)
				{
					m_pausedMusic.Add(item);
					item.Pause();
				}
			}
		}

		private void PauseLoopingSFX()
		{
			foreach (KeyValuePair<string, List<MinigameSFX>> soundEffect in m_soundEffects)
			{
				foreach (MinigameSFX item in soundEffect.Value)
				{
					if (item.IsPlaying && item.IsLooping)
					{
						m_pausedSoundEffects.Add(item);
						item.Pause();
					}
				}
			}
		}

		protected virtual void OnResume()
		{
			ResumeMusic();
			ResumeLoopingSFX();
			ClassicMiniGames.RemoveBackButtonHandler(PauseGame);
			ClassicMiniGames.PushBackButtonHandler(PauseGame);
		}

		public void ResumeMusic()
		{
			foreach (AudioSource item in m_pausedMusic)
			{
				item.volume = MusicVolume;
				item.Play();
			}
			m_pausedMusic.Clear();
		}

		private void ResumeLoopingSFX()
		{
			foreach (MinigameSFX pausedSoundEffect in m_pausedSoundEffects)
			{
				pausedSoundEffect.SetVolume(SFxVolume);
				pausedSoundEffect.Play();
			}
			m_pausedSoundEffects.Clear();
		}

		internal void RegisterMusic(AudioSource _track)
		{
			_track.volume = MusicVolume;
			m_registeredMusic.Add(_track);
		}

		public void RestartMusic(string _name)
		{
			foreach (AudioSource item in m_registeredMusic)
			{
				if (item.clip.name == _name)
				{
					item.volume = MusicVolume;
					item.Stop();
					item.Play();
				}
			}
		}

		public abstract string GetMinigameName();

		internal void RegisterSFX(MinigameSFX _minigameSFX)
		{
			if (!m_soundEffects.ContainsKey(_minigameSFX.Name))
			{
				_minigameSFX.SetVolume(SFxVolume);
				m_soundEffects.Add(_minigameSFX.Name, new List<MinigameSFX>());
			}
			m_soundEffects[_minigameSFX.Name].Add(_minigameSFX);
		}

		internal void UnregisterSFX(MinigameSFX _minigameSFX)
		{
			if (m_soundEffects.ContainsKey(_minigameSFX.Name))
			{
				List<MinigameSFX> list = m_soundEffects[_minigameSFX.Name];
				list.Remove(_minigameSFX);
			}
		}

		public MinigameSFX PlaySFX(string _name)
		{
			if (!AreSFXEnabled)
			{
				return null;
			}
			MinigameSFX result = null;
			if (m_soundEffects.ContainsKey(_name))
			{
				List<MinigameSFX> list = m_soundEffects[_name];
				foreach (MinigameSFX item in list)
				{
					item.SetVolume(SFxVolume);
					if (!item.IsPlaying)
					{
						item.Play();
						result = item;
						break;
					}
				}
			}
			return result;
		}

		public void StopSFX(string _name)
		{
			if (m_soundEffects.ContainsKey(_name))
			{
				List<MinigameSFX> list = m_soundEffects[_name];
				foreach (MinigameSFX item in list)
				{
					if (item.IsPlaying)
					{
						item.Stop();
					}
				}
			}
		}

		public void StopAllSFX()
		{
			foreach (List<MinigameSFX> value in m_soundEffects.Values)
			{
				foreach (MinigameSFX item in value)
				{
					if (item.IsPlaying)
					{
						item.Stop();
					}
				}
			}
		}

		public void SetVolume(float musicVolume, float sfxVolume)
		{
			foreach (List<MinigameSFX> value in m_soundEffects.Values)
			{
				foreach (MinigameSFX item in value)
				{
					item.SetVolume(sfxVolume);
				}
			}
			foreach (AudioSource item2 in m_registeredMusic)
			{
				item2.volume = musicVolume;
			}
		}

		public virtual void OnQuit()
		{
			m_soundEffects.Clear();
			m_registeredMusic.Clear();
			MinigameManager.Instance.OnMinigameEnded();
			m_isQuiting = true;
			ClassicMiniGames.RemoveBackButtonHandler(PauseGame);
		}
	}
}
