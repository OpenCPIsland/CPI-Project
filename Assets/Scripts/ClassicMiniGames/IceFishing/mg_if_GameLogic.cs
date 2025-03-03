using ClubPenguin.Classic.MiniGames;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_GameLogic : MonoBehaviour
	{
		private mg_if_Variables m_variables;

		private mg_if_InputManager m_inputManager;

		private bool m_initialized;

		private mg_if_SpawnManager m_spawnManager;

		private mg_if_IceBox m_iceBox;

		private mg_if_FishingRod m_fishingRod;

		private float m_gameOverTimer;

		private bool m_resultShown;

		private MouseInputObserver mouseInputObserver;

		public Transform TopZone
		{
			get;
			private set;
		}

		public Transform BottomZone
		{
			get;
			private set;
		}

		public Transform AlphaLeft
		{
			get;
			private set;
		}

		public Transform AlphaRight
		{
			get;
			private set;
		}

		public GameObject CrabStopLeft
		{
			get;
			private set;
		}

		public GameObject CrabStopRight
		{
			get;
			private set;
		}

		public mg_if_FishingRod FishingRod
		{
			get
			{
				return m_fishingRod;
			}
		}

		public int Lives
		{
			get;
			private set;
		}

		public int FishCaught
		{
			get;
			private set;
		}

		public bool GameOver
		{
			get;
			private set;
		}

		public void Awake()
		{
			m_variables = MinigameManager.GetActive<mg_IceFishing>().Resources.Variables;
			TopZone = base.transform.Find("mg_if_ZoneTop");
			BottomZone = base.transform.Find("mg_if_ZoneBottom");
			AlphaLeft = base.transform.Find("mg_if_AlphaLeft");
			AlphaRight = base.transform.Find("mg_if_AlphaRight");
			CrabStopLeft = base.transform.Find("mg_if_CrabStop_Left").gameObject;
			CrabStopRight = base.transform.Find("mg_if_CrabStop_Right").gameObject;
			GameObject gameObject = base.transform.Find("mg_if_GameBG").gameObject;
			Vector3 localScale = gameObject.transform.localScale;
			MinigameSpriteHelper.FitSpriteToScreen(MinigameManager.GetActive<mg_IceFishing>().MainCamera, gameObject, false);
			base.transform.localScale = gameObject.transform.localScale;
			gameObject.transform.localScale = localScale;
			mouseInputObserver = base.gameObject.AddComponent<MouseInputObserver>();
		}

		internal void OnMouseMoved(Vector3 mousePosition)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
			m_fishingRod.MoveLineTo(mousePosition);
		}

		public void Start()
		{
			m_spawnManager = GetComponent<mg_if_SpawnManager>();
			m_fishingRod = GetComponentInChildren<mg_if_FishingRod>();
			m_iceBox = GetComponentInChildren<mg_if_IceBox>();
			Lives = m_variables.StartingLives;
			GameOver = false;
			m_resultShown = false;
			FishCaught = 0;
			m_inputManager = new mg_if_InputManager(MinigameManager.GetActive().MainCamera, this, mouseInputObserver);
			m_initialized = true;
		}

		public void OnDestroy()
		{
			m_inputManager.TidyUp();
			m_inputManager = null;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (!m_initialized)
			{
				return;
			}
			m_fishingRod.MinigameUpdate(p_deltaTime);
			m_spawnManager.MinigameUpdate(p_deltaTime);
			if (GameOver && !m_resultShown)
			{
				m_gameOverTimer -= p_deltaTime;
				if (m_gameOverTimer <= 0f)
				{
					UIManager.Instance.OpenScreen("mg_if_ResultScreen", false, OnResultPopped, null);
					m_resultShown = true;
				}
			}
		}

		public void LoseLife()
		{
			if (!GameOver)
			{
				Lives--;
				if (Lives <= 0)
				{
					EndGame();
				}
			}
		}

		public void GainLife()
		{
			if (!GameOver)
			{
				Lives++;
				MinigameManager.GetActive().PlaySFX("mg_if_sfx_WormCan1Up");
			}
		}

		public void OnFishCaught()
		{
			if (!GameOver)
			{
				FishCaught++;
				m_spawnManager.CheckGameRound(FishCaught);
				m_iceBox.OnFishCaught(FishCaught);
				MinigameManager.GetActive<mg_IceFishing>().CoinsEarned += m_variables.CoinsPerFish;
			}
		}

		private void OnResultPopped(UIControlBase p_screen)
		{
			if ((p_screen as mg_if_ResultScreen).Restart)
			{
				MinigameManager.GetActive<mg_IceFishing>().ShowTitle();
			}
			else
			{
				MinigameManager.Instance.OnMinigameEnded();
			}
		}

		public void EndGame()
		{
			GameOver = true;
			m_gameOverTimer = m_variables.GameOverTimer;
		}

		public void OnTouchDown(Vector2 p_screenPos)
		{
			if (!GameOver && !MinigameManager.IsPaused)
			{
				m_fishingRod.OnTouchDown(p_screenPos);
			}
		}

		public void OnSimpleTap(Vector2 p_screenPos)
		{
			if (!GameOver && !MinigameManager.IsPaused)
			{
				m_fishingRod.OnSimpleTap(p_screenPos, TopZone);
			}
		}

		public void DebugSpawn()
		{
			m_spawnManager.DebugSpawn();
		}
	}
}
