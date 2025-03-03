using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinigameFramework
{
	public class MinigameManager : MonoBehaviour
	{
		private static MinigameManager m_instance = null;

		private Minigame m_activeMinigame;

		public static bool IsPaused
		{
			get
			{
				return !(Instance.m_activeMinigame != null) || Instance.m_activeMinigame.IsPaused;
			}
		}

		public int PlayerCoins
		{
			get;
			private set;
		}

		public static MinigameManager Instance
		{
			get
			{
				return m_instance;
			}
		}

		public MinigameManager()
		{
			PlayerCoins = 0;
			m_activeMinigame = null;
		}

		public void Awake()
		{
			if (m_instance == null)
			{
				Object.DontDestroyOnLoad(this);
				m_instance = this;
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempted to create multiple MinigameManagers!");
			}
		}

		public void Shutdown()
		{
			m_instance = null;
			Object.Destroy(base.gameObject);
		}

		public void ShowMinigame(EMinigameTypes _type)
		{
			string initialScene = MinigameFactory.GetInitialScene(_type);
			Debug.Log("Loading scene: " + initialScene);
			SceneManager.LoadScene(initialScene);
		}

		public void OnMiniGameLoaded(Minigame _minigame)
		{
			m_activeMinigame = _minigame;
			m_activeMinigame.MusicVolume = ClassicMiniGames.MainGameMusicVolume;
			m_activeMinigame.SFxVolume = ClassicMiniGames.MainGameSFXVolume;
			m_activeMinigame.ResumeGame();
		}

		public static Minigame GetActive()
		{
			Minigame minigame = null;
			if (m_instance != null)
			{
				minigame = m_instance.m_activeMinigame;
				if (minigame != null)
				{
					minigame.MusicVolume = ClassicMiniGames.MainGameMusicVolume;
					minigame.SFxVolume = ClassicMiniGames.MainGameSFXVolume;
				}
			}
			return minigame;
		}

		public static T GetActive<T>() where T : Minigame
		{
			T result = null;
			if (m_instance != null)
			{
				return m_instance.m_activeMinigame as T;
			}
			return result;
		}

		public void ExitMinigame()
		{
			BaseGameController.DestroyInstance();
			ClassicMiniGames.AddCoinsToAccount(Instance.PlayerCoins);
			Instance.PlayerCoins = 0;
			m_activeMinigame = null;
			SceneManager.LoadScene("ClassicMiniGames");
			Resources.UnloadUnusedAssets();
		}

		public Color GetPenguinColor()
		{
			return new Color(0f, 1f, 1f);
		}

		public string GetPenguinName()
		{
			return "Penguin";
		}

		public void OnMinigameQuit()
		{
			m_activeMinigame.OnQuit();
		}

		public void OnMinigameEnded()
		{
			if (m_activeMinigame.CoinsEarned > 0)
			{
				PlayerCoins += m_activeMinigame.CoinsEarned;
				UIManager.Instance.OpenScreen("mg_ResultScreen", false, OnResultsClosed, null);
				m_activeMinigame.PauseGame();
			}
			else
			{
				ExitMinigame();
			}
		}

		private void OnResultsClosed(UIControlBase _screen)
		{
			ExitMinigame();
		}
	}
}
