using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_GameLogic
	{
		private mg_ss_EGameState m_gameState;

		protected bool m_gameStarted;

		private int m_combo;

		private float m_introTimer;

		private float m_suspendTimer;

		private mg_ss_PlayerLogic m_player;

		private mg_ss_ConveyorLogic m_conveyor;

		private mg_ss_ItemGenerator m_itemGenerator;

		public mg_SmoothieSmash Minigame
		{
			get;
			private set;
		}

		public mg_ss_EGameState GameState
		{
			get
			{
				return m_gameState;
			}
			private set
			{
				if (m_gameState != mg_ss_EGameState.GAME_OVER)
				{
					m_gameState = value;
				}
			}
		}

		public float GameTime
		{
			get;
			protected set;
		}

		public int Combo
		{
			get
			{
				return m_combo;
			}
			private set
			{
				m_combo = Mathf.Min(value, 10);
			}
		}

		public float CountdownTimer
		{
			get;
			private set;
		}

		public float CountdownTotalTime
		{
			get
			{
				return 2.1f;
			}
		}

		public mg_ss_ItemManager ItemManager
		{
			get;
			private set;
		}

		public mg_ss_ChaosManager ChaosManager
		{
			get;
			private set;
		}

		public mg_ss_Score Scoring
		{
			get;
			protected set;
		}

		public Vector2 ConveyorWorldPosition
		{
			get
			{
				return m_conveyor.ConveyorWorldPosition;
			}
		}

		public float ConveyorSpeed
		{
			get
			{
				return m_conveyor.CurrentSpeed;
			}
		}

		public float ConveyorItemSpacing
		{
			get
			{
				return m_conveyor.ItemSpacingTime;
			}
		}

		public bool ChaosModeActivated
		{
			get
			{
				return ChaosManager.Activated;
			}
		}

		public mg_ss_GameLogic()
		{
			Minigame = MinigameManager.GetActive<mg_SmoothieSmash>();
			ItemManager = new mg_ss_ItemManager();
			m_player = new mg_ss_PlayerLogic();
			m_conveyor = new mg_ss_ConveyorLogic();
			ChaosManager = new mg_ss_ChaosManager();
		}

		public virtual void Initialize(mg_ss_GameScreen p_screen)
		{
			m_conveyor.Initialize(p_screen, Minigame.Resources.ConveyorSpeedData);
			m_player.Initialize(p_screen, this);
			ItemManager.Initialize(m_conveyor, p_screen, Minigame);
			m_itemGenerator = p_screen.ItemGenerator;
			m_itemGenerator.Initialize(this, p_screen);
			ChaosManager.Initialize(this, p_screen.PlayerObject);
			m_introTimer = 2f;
			GameState = mg_ss_EGameState.INTRO;
			GameTime = 0f;
			m_gameStarted = false;
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_conveyor_loop");
		}

		public void Destroy()
		{
			Minigame active = MinigameManager.GetActive();
			if (active != null)
			{
				MinigameManager.GetActive().StopSFX("mg_ss_sfx_conveyor_loop");
			}
			m_player.Destroy();
			ChaosManager.Destroy();
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			switch (GameState)
			{
			case mg_ss_EGameState.INTRO:
				Update_Intro(p_deltaTime);
				break;
			case mg_ss_EGameState.COUNTDOWN:
				Update_Countdown(p_deltaTime);
				break;
			case mg_ss_EGameState.ACTIVE:
				Update_Active(p_deltaTime);
				break;
			case mg_ss_EGameState.SUSPEND:
				Update_Suspend(p_deltaTime);
				break;
			}
		}

		private void Update_Intro(float p_deltaTime)
		{
			m_introTimer -= p_deltaTime;
			if (m_introTimer <= 0f)
			{
				SetStateToCountdown();
			}
			UpdateFruit(p_deltaTime);
		}

		private void Update_Countdown(float p_deltaTime)
		{
			UpdateGeneric(p_deltaTime);
			CountdownTimer += p_deltaTime;
			if (CountdownTimer >= CountdownTotalTime)
			{
				m_gameStarted = true;
				GameState = mg_ss_EGameState.ACTIVE;
				Minigame.PlaySFX("mg_ss_sfx_game_start");
			}
		}

		protected void Update_Active(float p_deltaTime)
		{
			UpdateGeneric(p_deltaTime);
			m_player.MinigameUpdate(p_deltaTime);
			if (CheckForGameOver())
			{
				OnGameOver();
			}
		}

		protected virtual void OnGameOver()
		{
			GameState = mg_ss_EGameState.GAME_OVER;
			Minigame.CoinsEarned += Scoring.Coins;
			UIManager.Instance.OpenScreen("mg_ss_ResultScreen", false, OnResultPopped, null);
		}

		private void Update_Suspend(float p_deltaTime)
		{
			m_suspendTimer += p_deltaTime;
			if (m_suspendTimer >= 1.5f)
			{
				GameState = mg_ss_EGameState.ACTIVE;
			}
			ChaosManager.MinigameUpdate_Suspend(p_deltaTime);
			ItemManager.MinigameUpdate_Suspend(p_deltaTime);
		}

		private void UpdateGeneric(float p_deltaTime)
		{
			UpdateGameTime(p_deltaTime);
			m_conveyor.CheckSpeedIncrease(GameTime);
			m_itemGenerator.CheckTierIncrease(GameTime);
			ChaosManager.MinigameUpdate(p_deltaTime);
			UpdateFruit(p_deltaTime);
		}

		protected virtual void UpdateGameTime(float p_deltaTime)
		{
			if (m_gameStarted)
			{
				GameTime += p_deltaTime;
			}
		}

		private void UpdateFruit(float p_deltaTime)
		{
			m_itemGenerator.MinigameUpdate(p_deltaTime);
			ItemManager.MinigameUpdate(p_deltaTime);
		}

		protected virtual bool CheckForGameOver()
		{
			return false;
		}

		public void SetStateToCountdown()
		{
			GameState = mg_ss_EGameState.COUNTDOWN;
			CountdownTimer = 0f;
		}

		public virtual void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			Scoring.Score += 10;
			Scoring.Score += Combo * 2;
			m_conveyor.IncreaseSpeedMultiplier();
			m_itemGenerator.OnFruitCollision(p_fruit);
			if (p_fruit.ChaosItem)
			{
				OnChaosItemCollision();
			}
		}

		public virtual void OnBombCollision(mg_ss_Item_BombObject p_bomb)
		{
			if (!ChaosModeActivated)
			{
				KillPlayer();
			}
		}

		public virtual void OnAnvilCollision(mg_ss_Item_AnvilObject p_anvil)
		{
			m_conveyor.DropSpeed();
		}

		public void OnFruitSquashed(mg_ss_Item_FruitObject p_fruit, bool p_correctFruit)
		{
			ItemManager.OnFruitSquashed(p_fruit, p_correctFruit);
		}

		public void ComboIncrease(mg_ss_Item_FruitObject p_fruit)
		{
			Combo++;
			p_fruit.ShowCombo(Combo);
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_fruit_point_" + Mathf.Min(Combo, 5).ToString("00"));
		}

		public void ComboReset()
		{
			Combo = 0;
		}

		public void OnGoldenAppleCollision(mg_ss_Item_GoldenAppleObject p_goldenApple)
		{
			ChaosManager.OnGoldenAppleCollected();
		}

		public virtual void OnClockCollision(mg_ss_Item_ClockObject p_clock)
		{
		}

		public void KillPlayer()
		{
			m_player.Kill();
		}

		public void PlayerDied()
		{
			m_conveyor.DropSpeed();
			SetStateToCountdown();
			ComboReset();
		}

		public bool ItemPartOfOrder(mg_ss_EItemTypes p_itemType)
		{
			return m_itemGenerator.ItemPartOfOrder(p_itemType);
		}

		public virtual void OnConveyorCollision()
		{
			ItemManager.BounceAllItems();
			m_conveyor.OnConveyorCollision();
		}

		public void SuspendGame()
		{
			GameState = mg_ss_EGameState.SUSPEND;
			m_suspendTimer = 0f;
		}

		public void OnChaosModeEnded()
		{
			m_itemGenerator.OnChaosModeEnded();
		}

		public void SpawnChaosFruit(float p_delay)
		{
			m_itemGenerator.SpawnChoasFruit(p_delay);
		}

		protected virtual void OnChaosItemCollision()
		{
		}

		private void OnResultPopped(UIControlBase p_screen)
		{
			if ((p_screen as mg_ss_ResultScreen).Restart)
			{
				Minigame.ShowTitle();
			}
			else
			{
				MinigameManager.Instance.OnMinigameEnded();
			}
		}
	}
}
