using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace PuffleRoundup
{
	public class mg_pr_RoundController : MonoBehaviour
	{
		private const int m_MAX_TIME = 120;

		private const int m_MIN_TIME = 20;

		private mg_pr_UIRoundCount m_roundCount;

		private mg_pr_UITimerCount m_timerCount;

		private mg_pr_UIPieTimer m_pieTimer;

		private GameObject m_pufflePen;

		private Animator m_pufflePenAnim;

		private float m_roundTime;

		public float m_gameTime;

		public float m_bonusTime;

		public int m_round;

		public int m_pufflesCaught;

		public float m_coinsRound;

		public float m_coinsTotal;

		public bool m_complete;

		public bool m_failed;

		private float m_coinMultiplier;

		private int m_pufflesEscaped;

		private int m_numToSpawn;

		private bool m_isPlaying;

		public mg_PuffleRoundup Minigame;

		private void Awake()
		{
			Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
			m_roundCount = GetComponentInChildren<mg_pr_UIRoundCount>();
			m_timerCount = GetComponentInChildren<mg_pr_UITimerCount>();
			m_pieTimer = GetComponentInChildren<mg_pr_UIPieTimer>();
			m_pufflePen = Minigame.transform.Find("mg_pr_GameContainer/mg_pr_pf_GameBG(Clone)/mg_pr_pf_Pen").gameObject;
			m_pufflePenAnim = m_pufflePen.GetComponent<Animator>();
			m_pufflePenAnim.SetInteger("dir", 1);
		}

		private void Start()
		{
			m_isPlaying = false;
			StartRound();
		}

		private void Update()
		{
			if (m_isPlaying)
			{
				CheckGameTime();
			}
			UpdateMultiplier();
		}

		private void CheckGameTime()
		{
			if (m_gameTime > 0f && m_gameTime <= 10f)
			{
				MinigameManager.GetActive().PlaySFX("mg_pr_sfx_timer");
			}
			if (m_gameTime <= 0f)
			{
				MinigameManager.GetActive().PlaySFX("mg_pr_sfx_gameOver");
			}
			if (m_gameTime <= 0f)
			{
				m_gameTime = 0f;
				CompleteRound();
				m_isPlaying = false;
			}
		}

		private void UpdateMultiplier()
		{
			if ((float)m_round / 3f <= 1f)
			{
				m_coinMultiplier = 5f;
			}
			if ((float)m_round / 3f > 1f && (float)(m_round / 3) <= 2f)
			{
				m_coinMultiplier = 5.5f;
			}
			if ((float)m_round / 3f > 2f && (float)(m_round / 3) <= 3f)
			{
				m_coinMultiplier = 6f;
			}
			if ((float)m_round / 3f > 3f && (float)(m_round / 3) <= 4f)
			{
				m_coinMultiplier = 6.5f;
			}
			if ((float)m_round / 3f > 4f)
			{
				m_coinMultiplier = 7f;
			}
			m_roundCount.m_round = m_round;
			m_timerCount.m_timer = m_gameTime;
		}

		public void IncrementTime()
		{
			m_gameTime -= 1f;
		}

		public void UpdateTime()
		{
			m_roundTime = 120 - (m_round - 1) * 15;
			if (m_roundTime < 20f)
			{
				m_roundTime = 20f;
			}
			m_gameTime = m_roundTime + m_bonusTime;
			if (m_gameTime > 120f)
			{
				m_gameTime = 120f;
			}
		}

		public void UpdateScore()
		{
			m_coinsRound = (float)m_pufflesCaught * m_coinMultiplier;
			m_coinsTotal += m_coinsRound;
		}

		private void Init()
		{
			UpdateTime();
			m_pieTimer.m_gameTime = m_gameTime;
			m_pieTimer.StartTimer();
			m_complete = false;
			m_failed = false;
			m_isPlaying = true;
		}

		public void StartRound()
		{
			m_round = 1;
			m_bonusTime = 0f;
			m_numToSpawn = 0;
			m_coinsRound = 0f;
			m_coinsTotal = 0f;
			Init();
			float x = 0f;
			float y = -1f;
			m_pufflePen.transform.position = new Vector3(x, y, 0f);
			int value = 1;
			m_pufflePenAnim.SetInteger("dir", value);
			SendMessage("ClearObsticles");
			SendMessage("ClearPuffles");
			SendMessage("SpawnPuffles");
			GetComponent<mg_pr_GameLogic>().caught = 0;
			GetComponent<mg_pr_GameLogic>().escaped = 0;
			InvokeRepeating("IncrementTime", 1f, 1f);
		}

		public void RestartRound()
		{
			Init();
			SendMessage("ClearPuffles");
			SendMessage("SpawnPuffles");
			GetComponent<mg_pr_GameLogic>().caught = 0;
			GetComponent<mg_pr_GameLogic>().escaped = 0;
			InvokeRepeating("IncrementTime", 1f, 1f);
		}

		public void NextRound()
		{
			m_round++;
			Init();
			float x = Random.Range(-3f, 3f);
			float y = Random.Range(-2f, 1.5f);
			m_pufflePen.transform.position = new Vector3(x, y, 0f);
			int value = Random.Range(1, 5);
			m_pufflePenAnim.SetInteger("dir", value);
			if (m_round > 2)
			{
				int num = Random.Range(2, 5);
				m_numToSpawn = m_round / num;
				if (m_numToSpawn < 1)
				{
					m_numToSpawn = 1;
				}
			}
			SendMessage("ClearObsticles");
			SendMessage("ClearPuffles");
			SendMessage("SpawnObsticles", m_numToSpawn);
			SendMessage("SpawnPuffles");
			GetComponent<mg_pr_GameLogic>().caught = 0;
			GetComponent<mg_pr_GameLogic>().escaped = 0;
			InvokeRepeating("IncrementTime", 1f, 1f);
		}

		public void CompleteRound()
		{
			if (m_isPlaying)
			{
				StopTimer();
				m_pufflesCaught = GetComponent<mg_pr_GameLogic>().caught;
				m_pufflesEscaped = GetComponent<mg_pr_GameLogic>().escaped;
				if (m_pufflesCaught > 0)
				{
					m_bonusTime = m_gameTime / 2f;
					UpdateScore();
					m_complete = true;
					m_isPlaying = false;
					MinigameManager.GetActive().CoinsEarned = (int)m_coinsTotal;
					UIManager.Instance.OpenScreen("mg_pr_ResultScreen", false, null, null);
				}
				else
				{
					m_bonusTime = 0f;
					UpdateScore();
					m_failed = true;
					m_isPlaying = false;
					UIManager.Instance.OpenScreen("mg_pr_ResultScreen", false, null, null);
				}
			}
		}

		public void StopTimer()
		{
			CancelInvoke();
			m_pieTimer.StopTimer();
		}
	}
}
