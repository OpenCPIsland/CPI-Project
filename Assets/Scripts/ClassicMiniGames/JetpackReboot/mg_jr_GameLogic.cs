using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_GameLogic : MonoBehaviour
	{
		public abstract class GameLogicState
		{
			private mg_jr_GameLogic m_Logic;

			protected mg_jr_GameLogic Logic
			{
				get
				{
					return m_Logic;
				}
			}

			public bool IsTurboAllowed
			{
				get;
				protected set;
			}

			public bool IsGameInProgress
			{
				get;
				protected set;
			}

			public GameLogicState(mg_jr_GameLogic _logic)
			{
				m_Logic = _logic;
				IsTurboAllowed = false;
				IsGameInProgress = true;
			}

			public virtual void EnterState()
			{
			}

			public virtual void MinigameUpdate(float _deltaTime)
			{
				Logic.ScrollingSpeed.MinigameUpdate(_deltaTime);
				Logic.Odometer.MinigameUpdate(_deltaTime);
				Logic.EnvironmentManager.CurrentEnvironment.MinigameUpdate(_deltaTime);
				Logic.m_levelManager.MinigameUpdate(_deltaTime);
				Logic.Player.MinigameUpdate(_deltaTime);
			}

			public virtual void OnTouchDrag(int _finger, Vector2 _position)
			{
				if (_finger == 0)
				{
					Logic.Player.OnTouchDrag(_position);
				}
			}

			public virtual void OnTouchPress(bool _isDown, int _finger, Vector2 _position)
			{
				if (_finger == 0)
				{
					Logic.Player.OnTouchPress(_isDown, _position);
				}
			}
		}

		public class GameLogicStateLaunchPad : GameLogicState
		{
			public GameLogicStateLaunchPad(mg_jr_GameLogic _logic)
				: base(_logic)
			{
				base.IsTurboAllowed = true;
				base.IsGameInProgress = false;
			}

			public override void EnterState()
			{
				UIManager.Instance.ClearScreenStack();
				base.Logic.m_minigame.MainCamera.orthographicSize = 3.84f;
				if (base.Logic.m_activeBoss != null)
				{
					base.Logic.m_minigame.Resources.ReturnPooledResource(base.Logic.m_activeBoss.gameObject);
				}
				if (base.Logic.m_turboSpeedLines.gameObject.activeSelf)
				{
					base.Logic.m_turboSpeedLines.StopLinesImmediately();
					base.Logic.m_turboSpeedLines.gameObject.SetActive(false);
				}
				if (base.Logic.m_transition != null)
				{
					Object.Destroy(base.Logic.m_transition.gameObject);
				}
				base.Logic.SpawnStartPlatform();
				Vector3 position = base.Logic.m_startPlatform.transform.Find("mg_jr_PenguinStartPoint").position;
				GameObject pooledResource = base.Logic.m_minigame.Resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_TRANSITION);
				base.Logic.m_transition = pooledResource.GetComponent<mg_jr_Transition>();
				base.Logic.m_transition.transform.parent = base.Logic.transform;
				base.Logic.m_levelManager.RemoveAllLevels();
				base.Logic.m_levelManager.ResetLevelRarityAllEnvironments();
				base.Logic.EnvironmentManager.ChangeEnvironment(new mg_jr_EnvironmentID(EnvironmentType.FOREST, EnvironmentVariant.DEFAULT));
				base.Logic.m_environmentsSeenThisRun = 1;
				base.Logic.m_lastBossWasOnEnvironmentNumber = 0;
				base.Logic.ScrollingSpeed.Reset();
				base.Logic.ScrollingSpeed.ScrollingEnabled = true;
				base.Logic.Odometer.Reset();
				base.Logic.m_minigame.GoalManager.ResetRunGoalProgress();
				base.Logic.Player.LoadInitialState(position);
				if (base.Logic.IsFirstGameAfterLoading)
				{
					base.Logic.IntroGary = base.Logic.m_minigame.Resources.GetPooledResourceByComponent<mg_jr_IntroGary>(mg_jr_ResourceList.GARY_INTRO);
					base.Logic.IntroGary.transform.SetParent(base.Logic.transform, false);
					base.Logic.IntroGary.transform.position = new Vector2(-0.5f, 1f);
					base.Logic.IntroGary.StartIntro(IntroComplete);
					base.Logic.IsFirstGameAfterLoading = false;
				}
				else if (base.Logic.IntroGary != null)
				{
					base.Logic.m_minigame.Resources.ReturnPooledResource(base.Logic.IntroGary.gameObject);
					base.Logic.IntroGary = null;
				}
				base.Logic.m_minigame.MusicManager.SelectTrack(mg_jr_Sound.MENU_MUSIC_AMBIENT.ClipName());
				UIManager.Instance.OpenScreen("mg_jr_StartScreen", false, null, null);
			}

			private void IntroComplete()
			{
				base.Logic.m_minigame.Resources.ReturnPooledResource(base.Logic.IntroGary.gameObject);
				base.Logic.IntroGary = null;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
			}

			public override void OnTouchDrag(int _finger, Vector2 _position)
			{
			}

			public override void OnTouchPress(bool _isDown, int _finger, Vector2 _position)
			{
			}
		}

		protected class GameLogicStateNormalMode : GameLogicState
		{
			private bool m_isTransitionPending = false;

			public GameLogicStateNormalMode(mg_jr_GameLogic _logic)
				: base(_logic)
			{
				base.IsTurboAllowed = true;
			}

			public override void EnterState()
			{
				base.Logic.ScrollingSpeed.ScrollingEnabled = true;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.MinigameUpdate(_deltaTime);
				if (base.Logic.Player.IsDead)
				{
					base.Logic.ChangeState(new GameLogicStateGameOver(base.Logic));
				}
				else if (!m_isTransitionPending)
				{
					float distanceToTransition = base.Logic.DistanceToTransition;
					if (distanceToTransition < base.Logic.m_gameBalance.NoTurboBeforeTransitionDistance)
					{
						base.IsTurboAllowed = false;
					}
					if (distanceToTransition <= 0f)
					{
						base.Logic.m_levelManager.RemoveNonVisibleLevels();
						base.Logic.m_levelManager.ContinuousLevels = false;
						m_isTransitionPending = true;
					}
				}
				else if (base.Logic.m_levelManager.NumberOfLevelsActive == 0)
				{
					bool flag = base.Logic.m_environmentsSeenThisRun % 2 == 0;
					bool flag2 = base.Logic.m_environmentsSeenThisRun != base.Logic.m_lastBossWasOnEnvironmentNumber;
					if (flag && flag2)
					{
						base.Logic.ChangeState(new GameLogicStateBossFight(base.Logic));
						return;
					}
					UIManager.Instance.PopScreen();
					base.Logic.m_levelManager.ResetLevelRarity();
					base.Logic.ChangeState(new GameLogicStateTransition(base.Logic));
				}
			}
		}

		public class GameLogicStateStartingTurbo : GameLogicState
		{
			private bool m_isTurboAnnounceDone = false;

			private bool m_areLinesStarted = false;

			private mg_jr_Level m_lastLevelInCurrentTurboSequence = null;

			public GameLogicStateStartingTurbo(mg_jr_GameLogic _logic)
				: base(_logic)
			{
			}

			public override void EnterState()
			{
				mg_jr_TurboAnnounce pooledResourceByComponent = base.Logic.m_minigame.Resources.GetPooledResourceByComponent<mg_jr_TurboAnnounce>(mg_jr_ResourceList.PREFAB_TURBO_ANNOUNCE);
				pooledResourceByComponent.Announce(TurboAnnounceDone);
				base.Logic.ScrollingSpeed.EnableTurboSpeed(true);
				base.Logic.m_levelManager.RemoveNonVisibleLevels();
				base.Logic.m_levelManager.AddXtoQueue(EnvironmentType.TURBO, base.Logic.m_gameBalance.TurboLevelsPerTurboRun);
				float num = base.Logic.m_levelManager.EndPositionXofQueue - base.Logic.Player.transform.position.x;
				float timeToSpendInTurbo = num / base.Logic.m_gameBalance.MaxSpeed;
				base.Logic.Player.TurboDevice.UpdateTurboBurnTime(timeToSpendInTurbo);
				m_lastLevelInCurrentTurboSequence = base.Logic.m_levelManager.LastLevelInQueue;
				Assert.NotNull(m_lastLevelInCurrentTurboSequence, "No levels in queue");
				Assert.AreEqual(EnvironmentType.TURBO, m_lastLevelInCurrentTurboSequence.LevelDefinition.EnvironmentType, "Level isn't turbo");
				base.Logic.m_levelManager.AddEmptyLevelToQueue(base.Logic.m_gameBalance.EmptySpaceAfterTurbo);
				base.Logic.m_minigame.MusicManager.SelectTrack(mg_jr_Sound.TURBO_MODE.ClipName());
				base.Logic.m_minigame.PlaySFX(mg_jr_Sound.PLAYER_JETPACK_TURBO_START.ClipName());
				base.Logic.m_minigame.PlaySFX(mg_jr_Sound.PLAYER_TURBO_MODE_LOOP.ClipName());
				base.Logic.m_minigame.GoalManager.AddToProgress(mg_jr_Goal.GoalType.USE_TURBO, 1f);
			}

			private void TurboAnnounceDone()
			{
				m_isTurboAnnounceDone = true;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.MinigameUpdate(_deltaTime);
				if (m_isTurboAnnounceDone)
				{
					if (!m_areLinesStarted)
					{
						base.Logic.m_turboSpeedLines.gameObject.SetActive(true);
						base.Logic.m_turboSpeedLines.StartLines(mg_jr_SpeedLineScreenFx.LineStartMode.STAGGERED_START);
						m_areLinesStarted = true;
					}
					base.Logic.m_minigame.MainCamera.orthographicSize += _deltaTime * base.Logic.m_gameBalance.TurboCameraZoomRate;
					if (base.Logic.m_minigame.MainCamera.orthographicSize >= 5f)
					{
						base.Logic.m_minigame.MainCamera.orthographicSize = 5f;
						base.Logic.ChangeState(new GameLogicStateTurboMode(base.Logic, m_lastLevelInCurrentTurboSequence));
					}
				}
			}
		}

		public class GameLogicStateTurboMode : GameLogicState
		{
			private mg_jr_Level m_lastLevelInCurrentTurboSequence = null;

			public GameLogicStateTurboMode(mg_jr_GameLogic _logic, mg_jr_Level _lastLevelInCurrentTurboSequence)
				: base(_logic)
			{
				Assert.NotNull(_lastLevelInCurrentTurboSequence, "Turbomode running without a valid end level");
				m_lastLevelInCurrentTurboSequence = _lastLevelInCurrentTurboSequence;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.MinigameUpdate(_deltaTime);
				if (base.Logic.DistanceToTransition <= 0f)
				{
					base.Logic.m_levelManager.ContinuousLevels = false;
				}
				if (m_lastLevelInCurrentTurboSequence == null && base.Logic.Player.TurboDevice.TimeRemainingInTurbo <= base.Logic.m_timeToZoomCameraForTurbo)
				{
					base.Logic.ChangeState(new GameLogicStateEndingTurbo(base.Logic));
				}
			}
		}

		public class GameLogicStateEndingTurbo : GameLogicState
		{
			public GameLogicStateEndingTurbo(mg_jr_GameLogic _logic)
				: base(_logic)
			{
			}

			public override void EnterState()
			{
				base.Logic.m_levelManager.ResetLevelRarity(EnvironmentType.TURBO);
				base.Logic.ScrollingSpeed.EnableTurboSpeed(false);
				base.Logic.Player.EndTurbo();
				base.Logic.m_turboSpeedLines.StopLine(OnTurboLinesStopped);
				base.Logic.m_minigame.StopSFX(mg_jr_Sound.PLAYER_TURBO_MODE_LOOP.ClipName());
				base.Logic.m_minigame.PlaySFX(mg_jr_Sound.UI_TURBO_MODE_END.ClipName());
				base.Logic.m_minigame.MusicManager.SelectTrack(mg_jr_Sound.THEME_SONG_AMBIENT.ClipName());
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.MinigameUpdate(_deltaTime);
				base.Logic.m_minigame.MainCamera.orthographicSize -= _deltaTime * base.Logic.m_gameBalance.TurboCameraZoomRate;
				if (base.Logic.m_minigame.MainCamera.orthographicSize <= 3.84f)
				{
					base.Logic.m_minigame.MainCamera.orthographicSize = 3.84f;
					base.Logic.ChangeState(new GameLogicStateNormalMode(base.Logic));
				}
			}

			private void OnTurboLinesStopped()
			{
				base.Logic.m_turboSpeedLines.gameObject.SetActive(false);
			}
		}

		public class GameLogicStateTransition : GameLogicState
		{
			public GameLogicStateTransition(mg_jr_GameLogic _logic)
				: base(_logic)
			{
			}

			public override void EnterState()
			{
				base.Logic.Player.OnTouchPress(false, Vector3.zero);
				base.Logic.m_transition.Transition(base.Logic.Player, OnEnvironmentTransitionComplete);
				base.Logic.ScrollingSpeed.EnableTurboSpeed(true);
				base.Logic.EnvironmentManager.ChangeEnvironment();
				base.Logic.m_environmentsSeenThisRun++;
			}

			private void OnEnvironmentTransitionComplete()
			{
				if (base.Logic.m_currentGameState == this)
				{
					UIManager.Instance.OpenScreen("mg_jr_GameScreen", false, null, null);
					base.Logic.m_levelManager.AddEmptyLevelToQueue(base.Logic.m_gameBalance.EmptySpaceAtStartOfLevel * 0.5f);
					base.Logic.m_levelManager.ContinuousLevels = true;
					base.Logic.Odometer.EnvironmentChanged();
					base.Logic.ScrollingSpeed.EnableTurboSpeed(false);
					base.Logic.ChangeState(new GameLogicStateNormalMode(base.Logic));
				}
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.Logic.ScrollingSpeed.MinigameUpdate(_deltaTime);
				base.Logic.EnvironmentManager.CurrentEnvironment.MinigameUpdate(_deltaTime);
				base.Logic.m_levelManager.MinigameUpdate(_deltaTime);
				base.Logic.Player.MinigameUpdate(_deltaTime);
			}
		}

		public class GameLogicStateBossFight : GameLogicState
		{
			private const float AFTER_BATTLE_REST_TIME = 2f;

			private bool m_isBattleComplete = false;

			private float m_timeSpentWaitingAfterBattle = 0f;

			public GameLogicStateBossFight(mg_jr_GameLogic _logic)
				: base(_logic)
			{
			}

			public override void EnterState()
			{
				if (base.Logic.m_activeBoss != null)
				{
					base.Logic.m_minigame.Resources.ReturnPooledResource(base.Logic.m_activeBoss.gameObject);
					base.Logic.m_activeBoss = null;
				}
				List<mg_jr_ResourceList> list = new List<mg_jr_ResourceList>();
				switch (base.Logic.EnvironmentManager.CurrentEnvironment.Type)
				{
				case EnvironmentType.FOREST:
					list.Add(mg_jr_ResourceList.BOSS_HERBERT);
					list.Add(mg_jr_ResourceList.BOSS_PROTOBOT);
					list.Add(mg_jr_ResourceList.BOSS_KLUTZY);
					break;
				case EnvironmentType.TOWN:
					list.Add(mg_jr_ResourceList.BOSS_HERBERT);
					list.Add(mg_jr_ResourceList.BOSS_PROTOBOT);
					break;
				case EnvironmentType.WATER:
					list.Add(mg_jr_ResourceList.BOSS_HERBERT);
					list.Add(mg_jr_ResourceList.BOSS_KLUTZY);
					break;
				case EnvironmentType.CAVE:
					list.Add(mg_jr_ResourceList.BOSS_PROTOBOT);
					list.Add(mg_jr_ResourceList.BOSS_KLUTZY);
					break;
				default:
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Invalid environment type to select bosses for, using default boss.");
					break;
				}
				mg_jr_ResourceList assetTag = (list.Count <= 0) ? mg_jr_ResourceList.BOSS_KLUTZY : list[Random.Range(0, list.Count)];
				base.Logic.m_lastBossWasOnEnvironmentNumber = base.Logic.m_environmentsSeenThisRun;
				base.Logic.m_activeBoss = base.Logic.m_minigame.Resources.GetPooledResourceByComponent<mg_jr_Boss>(assetTag);
				base.Logic.m_activeBoss.transform.parent = base.Logic.transform;
				base.Logic.m_activeBoss.StartBossBattle(OnBossBattleComplete);
			}

			private void OnBossBattleComplete()
			{
				m_isBattleComplete = true;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
				base.MinigameUpdate(_deltaTime);
				if (base.Logic.Player.IsDead)
				{
					base.Logic.ChangeState(new GameLogicStateGameOver(base.Logic));
				}
				if (m_isBattleComplete)
				{
					m_timeSpentWaitingAfterBattle += Time.deltaTime;
					if (m_timeSpentWaitingAfterBattle > 2f)
					{
						UIManager.Instance.PopScreen();
						base.Logic.ChangeState(new GameLogicStateTransition(base.Logic));
					}
				}
			}
		}

		public class GameLogicStateGameOver : GameLogicState
		{
			public GameLogicStateGameOver(mg_jr_GameLogic _logic)
				: base(_logic)
			{
				base.IsGameInProgress = false;
			}

			public override void EnterState()
			{
				base.Logic.ScrollingSpeed.ScrollingEnabled = false;
				base.Logic.m_minigame.StopAllSFX();
				base.Logic.m_minigame.PlayerStats.SubmitNewDistance((int)base.Logic.Odometer.DistanceTravelledThisRun);
				base.Logic.m_minigame.PlayerStats.SubmitNewCoinAmount(base.Logic.Player.CoinsCollected);
				base.Logic.m_minigame.PlayerStats.SaveStats();
				base.Logic.m_minigame.GoalManager.AddToProgress(mg_jr_Goal.GoalType.PLAY_GAMES, 1f);
				if (base.Logic.m_minigame.GoalManager.AreAnyActiveGoalsComplete)
				{
					UIManager.Instance.OpenScreen("mg_jr_GoalScreen", false, null, null);
				}
				else
				{
					UIManager.Instance.OpenScreen("mg_jr_ResultScreen", false, null, null);
				}
			}

			public override void MinigameUpdate(float _deltaTime)
			{
			}
		}

		public class GameLogicStateIdle : GameLogicState
		{
			public GameLogicStateIdle(mg_jr_GameLogic _logic)
				: base(_logic)
			{
				base.IsGameInProgress = false;
			}

			public override void MinigameUpdate(float _deltaTime)
			{
			}

			public override void OnTouchDrag(int _finger, Vector2 _position)
			{
			}

			public override void OnTouchPress(bool _isDown, int _finger, Vector2 _position)
			{
			}
		}

		private const float TURBO_ORTHO_SIZE = 5f;

		private mg_JetpackReboot m_minigame;

		private GameLogicState m_currentGameState;

		private mg_jr_GameData m_gameBalance;

		private GameObject m_startPlatform = null;

		private mg_jr_LevelManager m_levelManager;

		private int m_environmentsSeenThisRun = 1;

		private mg_jr_Boss m_activeBoss = null;

		private int m_lastBossWasOnEnvironmentNumber = 0;

		private mg_jr_Transition m_transition;

		private mg_jr_SpeedLineScreenFx m_turboSpeedLines;

		private float m_timeToZoomCameraForTurbo;

		private Bounds m_turboPlayArea;

		public mg_JetpackReboot MiniGame
		{
			get
			{
				return m_minigame;
			}
		}

		private bool IsFirstGameAfterLoading
		{
			get;
			set;
		}

		public bool IsGameInProgress
		{
			get
			{
				bool result = false;
				if (m_currentGameState != null)
				{
					result = m_currentGameState.IsGameInProgress;
				}
				return result;
			}
		}

		public bool IsTurboAllowed
		{
			get
			{
				bool result = false;
				if (m_currentGameState != null)
				{
					result = m_currentGameState.IsTurboAllowed;
				}
				return result;
			}
		}

		public mg_jr_GameData GameBalance
		{
			get
			{
				return m_gameBalance;
			}
		}

		public mg_jr_Penguin Player
		{
			get;
			private set;
		}

		public mg_jr_EnvironmentManager EnvironmentManager
		{
			get;
			private set;
		}

		public mg_jr_ScrollingSpeed ScrollingSpeed
		{
			get;
			private set;
		}

		public mg_jr_Odometer Odometer
		{
			get;
			private set;
		}

		public mg_jr_IntroGary IntroGary
		{
			get;
			private set;
		}

		public Bounds TurboPlayArea
		{
			get
			{
				return m_turboPlayArea;
			}
		}

		private float DistanceToTransition
		{
			get
			{
				return m_gameBalance.EnvironmentDistanceBeforeTransition - Odometer.DistanceTravelledInEnvironment;
			}
		}

		private void ChangeState(GameLogicState _newState)
		{
			Assert.NotNull(_newState, "Can't set a null state");
			m_currentGameState = _newState;
			m_currentGameState.EnterState();
		}

		public virtual void Awake()
		{
			IsFirstGameAfterLoading = true;
			IntroGary = null;
			m_minigame = MinigameManager.GetActive<mg_JetpackReboot>();
			m_gameBalance = base.gameObject.AddComponent<mg_jr_GameData>();
			ScrollingSpeed = base.gameObject.AddComponent<mg_jr_ScrollingSpeed>();
			Odometer = base.gameObject.AddComponent<mg_jr_Odometer>();
			Odometer.Init(m_gameBalance, m_minigame.GoalManager);
			m_timeToZoomCameraForTurbo = m_gameBalance.TurboCameraZoomRate * Mathf.Abs(1.16000009f);
			ScrollingSpeed.Init(m_gameBalance, Odometer);
			ScrollingSpeed.ScrollingEnabled = false;
			Player = m_minigame.Resources.GetPooledResourceByComponent<mg_jr_Penguin>(mg_jr_ResourceList.GAME_PREFAB_PENGUIN);
			Player.transform.parent = base.transform;
			GameObject pooledResource = m_minigame.Resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_TRANSITION);
			m_transition = pooledResource.GetComponent<mg_jr_Transition>();
			m_transition.transform.parent = base.transform;
			GameObject gameObject = new GameObject("mg_jr_EnvironmentManager");
			gameObject.transform.parent = base.transform;
			EnvironmentManager = gameObject.AddComponent<mg_jr_EnvironmentManager>();
			GameObject gameObject2 = new GameObject("mg_jr_LevelContainer");
			gameObject2.transform.parent = base.transform;
			m_levelManager = gameObject2.AddComponent<mg_jr_LevelManager>();
			m_levelManager.Init(ScrollingSpeed, this);
			m_turboSpeedLines = m_minigame.Resources.GetPooledResourceByComponent<mg_jr_SpeedLineScreenFx>(mg_jr_ResourceList.PREFAB_TURBO_SPEEDLINES);
			m_turboSpeedLines.transform.parent = base.transform;
			m_turboSpeedLines.gameObject.SetActive(false);
			m_turboPlayArea = new Bounds(Vector3.zero, new Vector3(10f * Camera.main.aspect, 10f, 40f));
		}

		public void LoadInitialState()
		{
			ChangeState(new GameLogicStateLaunchPad(this));
		}

		private void SpawnStartPlatform()
		{
			if (m_startPlatform != null)
			{
				m_minigame.Resources.ReturnPooledResource(m_startPlatform);
			}
			m_startPlatform = m_minigame.Resources.GetInstancedResource(mg_jr_ResourceList.PREFAB_START_PLATFORM);
			Vector3 position = m_startPlatform.transform.position;
			position.x = m_minigame.VisibleWorldBounds.min.x;
			m_startPlatform.transform.position = position;
			m_startPlatform.transform.parent = base.transform;
			mg_jr_Scroller component = m_startPlatform.GetComponent<mg_jr_Scroller>();
			component.enabled = false;
			SpriteRenderer component2 = m_startPlatform.GetComponent<SpriteRenderer>();
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.START_PLATFORM);
		}

		public void StartGame(bool _startWithBonusRobot = false)
		{
			Assert.IsTrue(m_currentGameState.GetType() == typeof(GameLogicStateLaunchPad), "Not in valid state to start game");
			UIManager.Instance.ClearScreenStack();
			UIManager.Instance.OpenScreen("mg_jr_GameScreen", false, null, null);
			m_minigame.MusicManager.SelectTrack(mg_jr_Sound.THEME_SONG_AMBIENT.ClipName(), true);
			if (_startWithBonusRobot)
			{
				Player.AddFreeRobotPenguin();
			}
			Player.Launch();
			ChangeState(new GameLogicStateNormalMode(this));
			m_levelManager.AddEmptyLevelToQueue(m_gameBalance.EmptySpaceAtStartOfLevel);
			m_levelManager.ContinuousLevels = true;
			mg_jr_Scroller component = m_startPlatform.GetComponent<mg_jr_Scroller>();
			component.enabled = true;
		}

		public void ActivateTurboMode()
		{
			if (m_currentGameState.IsTurboAllowed && Player.StartTurbo())
			{
				ChangeState(new GameLogicStateStartingTurbo(this));
			}
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (m_currentGameState != null)
			{
				m_currentGameState.MinigameUpdate(_deltaTime);
			}
		}

		public int GetCurrentDifficulty()
		{
			int a = 1 + (Mathf.FloorToInt(Odometer.DistanceTravelledThisRun / m_gameBalance.DistanceDifficulty) + Player.RobotPenguinCount);
			return Mathf.Min(a, m_levelManager.MaximumDifficulty);
		}

		public int RandomisedCurrentDifficulty()
		{
			int num = GetCurrentDifficulty();
			int num2 = Random.Range(0, 101);
			switch (num)
			{
			case 2:
				if (num2 >= 40)
				{
					num--;
				}
				break;
			case 3:
				if (num2 < 40)
				{
					num -= 2;
				}
				else if (num2 < 70)
				{
					num--;
				}
				break;
			default:
				if (num2 < 40)
				{
					num = 1;
				}
				else if (num2 < 50)
				{
					num -= 2;
				}
				else if (num2 < 70)
				{
					num--;
				}
				break;
			case 1:
				break;
			}
			return num;
		}

		public void OnTouchDrag(int _finger, Vector2 _position)
		{
			if (_finger == 0 && m_currentGameState != null)
			{
				m_currentGameState.OnTouchDrag(_finger, _position);
			}
		}

		public void OnTouchPress(bool _isDown, int _finger, Vector2 _position)
		{
			if (_finger == 0 && m_currentGameState != null)
			{
				m_currentGameState.OnTouchPress(_isDown, _finger, _position);
			}
		}
	}
}
