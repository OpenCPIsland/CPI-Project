using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ChaosManager
	{
		private mg_ss_GameLogic m_logic;

		private mg_ss_ChaosAnimation m_chaosAnimation;

		private mg_ss_ChaosModeData m_dataHead;

		private mg_ss_ChaosModeData m_currentData;

		private float m_lastFruitSpawnDelay;

		private float m_currentTierTotalTime;

		private float m_lastSpawnTime;

		public float ChaosTotalTime
		{
			get;
			private set;
		}

		public bool Activated
		{
			get;
			private set;
		}

		public int ApplesCollected
		{
			get;
			private set;
		}

		public float ActivationTimer
		{
			get;
			private set;
		}

		public void Initialize(mg_ss_GameLogic p_logic, mg_ss_PlayerObject p_playerObject)
		{
			m_logic = p_logic;
			m_dataHead = m_logic.Minigame.Resources.ChaosModeData;
			m_chaosAnimation = new mg_ss_ChaosAnimation();
			m_chaosAnimation.Initialize(p_playerObject);
			CalculateTotalTime();
			Reset();
		}

		public void Destroy()
		{
			m_chaosAnimation.Reset();
		}

		private void CalculateTotalTime()
		{
			ChaosTotalTime = 0f;
			for (mg_ss_ChaosModeData mg_ss_ChaosModeData = m_dataHead; mg_ss_ChaosModeData != null; mg_ss_ChaosModeData = mg_ss_ChaosModeData.NextData)
			{
				ChaosTotalTime += mg_ss_ChaosModeData.TimeActive;
			}
		}

		private void Reset()
		{
			Activated = false;
			ApplesCollected = Mathf.Max(0, ApplesCollected - 5);
			ActivationTimer = 0f;
			m_lastFruitSpawnDelay = 0.2f;
			m_lastSpawnTime = 0f;
			m_currentData = m_dataHead;
			m_currentTierTotalTime = m_currentData.TimeActive;
			m_chaosAnimation.Reset();
			m_logic.OnChaosModeEnded();
		}

		public void OnGoldenAppleCollected()
		{
			ApplesCollected++;
			if (!Activated && ApplesCollected >= 5)
			{
				Activated = true;
				MinigameManager.GetActive().PlaySFX("mg_ss_sfx_chaos_mode");
				ActivationTimer = 0f;
				m_chaosAnimation.FlyApplesFly(m_logic.Minigame);
				m_logic.SuspendGame();
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (!Activated)
			{
				return;
			}
			m_chaosAnimation.MinigameUpdate(p_deltaTime);
			m_lastFruitSpawnDelay -= p_deltaTime;
			ActivationTimer += p_deltaTime;
			CheckTierIncrease();
			if (!Activated)
			{
				return;
			}
			if (m_currentData.FruitPerSecond == 0)
			{
				m_lastFruitSpawnDelay = m_currentTierTotalTime;
				return;
			}
			float num = 1f / (float)m_currentData.FruitPerSecond;
			while (m_lastSpawnTime + num <= ActivationTimer)
			{
				m_lastSpawnTime += num;
				SpawnFruit(num);
			}
		}

		public void MinigameUpdate_Suspend(float p_deltaTime)
		{
			m_chaosAnimation.MinigameUpdate(p_deltaTime);
		}

		private void CheckTierIncrease()
		{
			if (ActivationTimer >= m_currentTierTotalTime)
			{
				if (m_currentData.NextData == null)
				{
					Reset();
					return;
				}
				m_currentData = m_currentData.NextData;
				m_currentTierTotalTime += m_currentData.TimeActive;
			}
		}

		private void SpawnFruit(float p_interval)
		{
			m_lastFruitSpawnDelay += p_interval;
			m_logic.SpawnChaosFruit(m_lastFruitSpawnDelay);
		}
	}
}
