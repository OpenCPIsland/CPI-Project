using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Penguin : MonoBehaviour
	{
		public delegate void OnCountChanged(int _newCount);

		private enum PenguinState
		{
			ON_PLATFORM,
			TAKING_OFF,
			FLYING,
			FLYING_TURBO,
			DYING,
			DEAD,
			TRANSITIONING,
			MAX
		}

		private const float JR_FPS = 33.3f;

		private const int MAX_ROBO_PENGUINS = 3;

		private PenguinState m_currentState = PenguinState.ON_PLATFORM;

		private mg_jr_PenguinData m_penguinData = new mg_jr_PenguinData();

		private mg_JetpackReboot m_miniGame;

		private Camera m_mainCamera = null;

		private Vector3 m_positionAfterTakeOff = Vector3.zero;

		private float m_velocity = 0f;

		private float m_acceleration = 0f;

		private float m_limitUp = 1f;

		private float m_limitDown = -1f;

		private Transform[] m_robotTurboPositions = new Transform[3];

		private Animator m_animator = null;

		private mg_jr_Collector m_collecter = null;

		private mg_jr_Blinker m_blinker = null;

		private bool m_isBlinking = false;

		private float m_blinkTimeRemaining = 0f;

		private bool m_isInvincible = false;

		private bool m_isInvinciblityTimeLimited = false;

		private float m_invinciblityTimeRemaining = 0f;

		private mg_jr_ObstacleDestroyer m_obstacleDestroyer = null;

		private GameObject m_penguinSpritesContainer = null;

		private SpriteRenderer m_penguinRenderer = null;

		private SpriteRenderer m_skinRenderer = null;

		private SpriteRenderer m_thrustRenderer = null;

		private SpriteRenderer m_domeRenderer = null;

		private bool m_isLeavingTurbo = false;

		private float m_fingerToPenguinDeltaToEndTurbo = 0.2f;

		private Vector3 m_lastTouchPositionInWorld = Vector3.zero;

		private int m_coinsCollected = 0;

		private List<mg_jr_RobotPenguin> m_guardianRoboPenguins = new List<mg_jr_RobotPenguin>();

		private bool m_isThrusting = false;

		public Vector3 VisualCenterOfPenguin
		{
			get
			{
				return m_penguinRenderer.bounds.center;
			}
		}

		public int CoinsCollected
		{
			get
			{
				return m_coinsCollected;
			}
			private set
			{
				if (m_coinsCollected != value)
				{
					m_coinsCollected = value;
					if (this.CoinCountChanged != null)
					{
						this.CoinCountChanged(m_coinsCollected);
					}
				}
			}
		}

		public int CoinsCollectedSession
		{
			get;
			private set;
		}

		public mg_jr_TurboDevice TurboDevice
		{
			get;
			private set;
		}

		public bool IsDead
		{
			get
			{
				return m_currentState == PenguinState.DEAD;
			}
		}

		public int RobotPenguinCount
		{
			get
			{
				return m_guardianRoboPenguins.Count;
			}
		}

		public event OnCountChanged CoinCountChanged;

		public void AwardBonusCoins(int _numberOfCoins)
		{
			CoinsCollected += _numberOfCoins;
			CoinsCollectedSession += _numberOfCoins;
		}

		private void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			TurboDevice = GetComponent<mg_jr_TurboDevice>();
			if (TurboDevice == null)
			{
				TurboDevice = base.gameObject.AddComponent<mg_jr_TurboDevice>();
			}
			m_collecter = GetComponent<mg_jr_Collector>();
			if (m_collecter == null)
			{
				m_collecter = base.gameObject.AddComponent<mg_jr_Collector>();
			}
			m_obstacleDestroyer = GetComponent<mg_jr_ObstacleDestroyer>();
			m_obstacleDestroyer.enabled = false;
			m_obstacleDestroyer.ObstacleDestroyed += OnCoinsCollected;
			Assert.NotNull(m_obstacleDestroyer, "ObstacleDestroyer  required");
			m_collecter.CoinCollected += OnCoinsCollected;
			m_collecter.RobotPenguinCollected += OnRobotPenguinCollected;
			m_collecter.TurboCollected += OnTuboCollected;
			CoinsCollectedSession = 0;
			m_blinker = GetComponent<mg_jr_Blinker>();
			if (m_blinker == null)
			{
				m_blinker = base.gameObject.AddComponent<mg_jr_Blinker>();
			}
			m_robotTurboPositions[0] = base.transform.Find("mg_jr_TopRobotSocket");
			m_robotTurboPositions[1] = base.transform.Find("mg_jr_MiddleRobotSocket");
			m_robotTurboPositions[2] = base.transform.Find("mg_jr_BottomRobotSocket");
			for (int i = 0; i < m_robotTurboPositions.Length; i++)
			{
				Assert.NotNull(m_robotTurboPositions[i], "Robot turbo position not found");
			}
			m_penguinSpritesContainer = base.transform.Find("mg_jr_penguinSprites").gameObject;
			m_animator = m_penguinSpritesContainer.GetComponent<Animator>();
			Assert.NotNull(m_animator);
			GameObject gameObject = base.transform.Find("mg_jr_penguinSprites/mg_jr_PenguinSprite").gameObject;
			m_penguinRenderer = gameObject.GetComponent<SpriteRenderer>();
			m_penguinRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN);
			GameObject gameObject2 = base.transform.Find("mg_jr_penguinSprites/mg_jr_Penguin_Thrust").gameObject;
			Assert.NotNull(gameObject2, "Thruster gameobject not found");
			m_thrustRenderer = gameObject2.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_thrustRenderer, "Thruster has no spriterenderer");
			m_thrustRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN_THRUST);
			GameObject gameObject3 = base.transform.Find("mg_jr_penguinSprites/mg_jr_Penguin_Tint").gameObject;
			Assert.NotNull(gameObject3, "Skin gameobject not found");
			m_skinRenderer = gameObject3.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_skinRenderer, "Skin has no spriterenderer");
			m_skinRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.TINT);
			GameObject gameObject4 = base.transform.Find("mg_jr_penguinSprites/mg_jr_PenguinTurboDome").gameObject;
			Assert.NotNull(gameObject4, "Skin gameobject not found");
			m_domeRenderer = gameObject4.GetComponent<SpriteRenderer>();
			Assert.NotNull(m_domeRenderer, "Skin has no spriterenderer");
			m_domeRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.DOME);
			m_mainCamera = Camera.main;
			m_limitUp = m_mainCamera.orthographicSize - m_penguinData.DistanceToTopOfScreenLimit;
			m_limitDown = 0f - m_mainCamera.orthographicSize + m_penguinData.DistanceToBottomOfScreenLimit;
		}

		public void LoadInitialState(Vector3 _penguinStartPoint)
		{
			base.gameObject.SetActive(true);
			base.transform.position = _penguinStartPoint;
			m_currentState = PenguinState.ON_PLATFORM;
			SetThrusting(false);
			float x = MinigameManager.GetActive<mg_JetpackReboot>().VisibleWorldBounds.min.x + m_penguinData.NormalDistanceFromLeft;
			float y = base.transform.position.y + m_penguinData.OnTakeOffClimb;
			m_positionAfterTakeOff = new Vector3(x, y, 0f);
			CoinsCollected = 0;
			StopBlinking();
			BecomeVulnerable();
			while (RemoveRobotPenguin(false))
			{
			}
			m_animator.SetBool("TurboMode", false);
			DisableObstacleDestroyer();
			TurboDevice.LoadInitialState();
			m_isLeavingTurbo = false;
			m_lastTouchPositionInWorld = Vector3.zero;
			m_animator.SetTrigger("StandOnPlatform");
		}

		public void Launch()
		{
			m_animator.SetTrigger("TakeOff");
			SetThrusting(true);
			m_currentState = PenguinState.TAKING_OFF;
			TurboDevice.BeginTracking();
		}

		private void OnDestroy()
		{
			m_collecter.CoinCollected -= OnCoinsCollected;
			m_collecter.RobotPenguinCollected -= OnRobotPenguinCollected;
			m_collecter.TurboCollected -= OnTuboCollected;
		}

		public void StartTransition()
		{
			m_currentState = PenguinState.TRANSITIONING;
			m_animator.SetTrigger("StartTransition");
			Vector3 position = base.transform.position;
			position.y = m_penguinData.YPosDuringTransition;
			base.transform.position = position;
		}

		public void EndTransition()
		{
			m_currentState = PenguinState.FLYING;
			m_animator.SetTrigger("Idle");
		}

		public bool StartTurbo()
		{
			if (m_currentState != PenguinState.FLYING)
			{
				return false;
			}
			bool flag = TurboDevice.ActivateTurbo();
			if (flag)
			{
				if (m_isBlinking)
				{
					StopBlinking();
				}
				BecomeInvincible();
				EnableObstacleDestroyer();
				RobotPenguinsToTurbo();
				m_currentState = PenguinState.FLYING_TURBO;
				m_animator.SetBool("TurboMode", true);
				m_lastTouchPositionInWorld = base.transform.position;
			}
			return flag;
		}

		public void EndTurbo()
		{
			m_isLeavingTurbo = true;
			m_animator.SetBool("TurboMode", false);
			RobotPenguinsToNormal();
			DisableObstacleDestroyer();
			BecomeVulnerable();
		}

		private void KillPenguin()
		{
			SetThrusting(false);
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.PLAYER_EXPLODE.ClipName());
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.PLAYER_DEATH.ClipName());
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			GameObject pooledResource = active.Resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_PENGUIN_EXPLOSION);
			SpriteRenderer component = pooledResource.GetComponent<SpriteRenderer>();
			Assert.NotNull(component, "Effect must have a spriterenderer");
			Vector3 center = component.bounds.center;
			Vector3 b = pooledResource.transform.position - center;
			pooledResource.transform.position = m_penguinRenderer.bounds.center + b;
			m_animator.SetTrigger("Kill");
			m_currentState = PenguinState.DYING;
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			float num = 33.3f * _deltaTime;
			if (m_isBlinking && m_blinkTimeRemaining > 0f)
			{
				m_blinkTimeRemaining -= _deltaTime;
				if (m_blinkTimeRemaining <= 0f)
				{
					StopBlinking();
				}
			}
			if (m_isInvinciblityTimeLimited && m_isInvincible && m_invinciblityTimeRemaining > 0f)
			{
				m_invinciblityTimeRemaining -= _deltaTime;
				if (m_invinciblityTimeRemaining <= 0f)
				{
					BecomeVulnerable();
				}
			}
			switch (m_currentState)
			{
			case PenguinState.ON_PLATFORM:
				break;
			case PenguinState.DEAD:
				break;
			case PenguinState.TRANSITIONING:
				break;
			case PenguinState.TAKING_OFF:
			{
				Vector3 vector = m_positionAfterTakeOff - base.transform.position;
				Vector3 normalized = vector.normalized;
				float num2 = m_penguinData.TakeOffSpeed * num;
				if (num2 * num2 > vector.sqrMagnitude)
				{
					base.transform.position = m_positionAfterTakeOff;
					m_currentState = PenguinState.FLYING;
					SetThrusting(false);
				}
				else
				{
					base.transform.position += normalized * num2;
				}
				break;
			}
			case PenguinState.FLYING:
				NormalMovementUpdate(num);
				break;
			case PenguinState.FLYING_TURBO:
				TurboMovementUpdate(num);
				break;
			case PenguinState.DYING:
				NormalMovementUpdate(num);
				if (!MinigameManager.GetActive<mg_JetpackReboot>().VisibleWorldBounds.Intersects(m_penguinRenderer.bounds))
				{
					m_currentState = PenguinState.DEAD;
					m_animator.SetTrigger("Idle");
					base.gameObject.SetActive(false);
					m_penguinSpritesContainer.transform.localPosition = Vector3.zero;
				}
				break;
			default:
				Assert.IsTrue(false, "Unhandled penguin state in update");
				break;
			}
		}

		private void NormalMovementUpdate(float _dtFPS)
		{
			m_velocity += m_acceleration * _dtFPS;
			m_acceleration += m_acceleration * (m_penguinData.AccelerationFactor * _dtFPS * 0.5f);
			m_velocity = Mathf.Clamp(m_velocity, m_penguinData.MinVerticalVelocity, m_penguinData.MaxVerticalVelocity);
			m_acceleration = Mathf.Clamp(m_acceleration, m_penguinData.AccelerationMaxDown, m_penguinData.AccelerationMaxUp);
			Vector3 position = base.transform.position;
			position.y += m_velocity * _dtFPS;
			if (position.y > m_limitUp)
			{
				position.y = m_limitUp;
				m_velocity = 0f;
			}
			else if (position.y < m_limitDown)
			{
				position.y = m_limitDown;
				m_velocity = 0f;
			}
			base.transform.position = position;
		}

		private void TurboMovementUpdate(float _dtFPS)
		{
			float num = 0f;
			if (!m_isLeavingTurbo)
			{
				num = m_lastTouchPositionInWorld.y;
			}
			float num2 = num + m_penguinRenderer.bounds.size.y * 0.5f - base.transform.position.y;
			float value = base.transform.position.y + num2 * m_penguinData.DistanceReductionFactor;
			float max = m_mainCamera.orthographicSize - m_penguinData.DistanceToTopOfScreenLimit;
			float min = 0f - m_mainCamera.orthographicSize + m_penguinData.DistanceToBottomOfScreenLimit;
			value = Mathf.Clamp(value, min, max);
			Vector3 position = base.transform.position;
			position.y = value;
			base.transform.position = position;
			if (m_isLeavingTurbo && num2 < m_fingerToPenguinDeltaToEndTurbo)
			{
				m_isLeavingTurbo = false;
				SetThrusting(false);
				m_currentState = PenguinState.FLYING;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			mg_jr_Collectable component = other.GetComponent<mg_jr_Collectable>();
			if (component == null)
			{
				ObstacleCollision();
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			mg_jr_Collectable component = other.GetComponent<mg_jr_Collectable>();
			if (component == null)
			{
				ObstacleCollision();
			}
		}

		private void ObstacleCollision()
		{
			if (!m_isInvincible && m_currentState == PenguinState.FLYING)
			{
				if (HasRobotPenguins())
				{
					RemoveRobotPenguin();
					BecomeInvincible(m_penguinData.InvincibilityDuration);
					StartBlinking(m_penguinData.InvincibilityDuration);
					mg_jr_GameLogic gameLogic = MinigameManager.GetActive<mg_JetpackReboot>().GameLogic;
					gameLogic.Odometer.ApplyDistancePenalty();
				}
				else
				{
					KillPenguin();
				}
			}
		}

		private void SetThrusting(bool _thrusting)
		{
			if (m_isThrusting != _thrusting)
			{
				m_isThrusting = _thrusting;
				if (m_isThrusting)
				{
					m_isThrusting = true;
					m_acceleration = m_penguinData.AccelerationStartUp;
					m_animator.SetBool("Thrusting", m_isThrusting);
					MinigameManager.GetActive().PlaySFX(mg_jr_Sound.JETPACK_LOOP.ClipName());
				}
				else
				{
					m_isThrusting = false;
					m_acceleration = m_penguinData.AaccelerationStartDown;
					m_animator.SetBool("Thrusting", m_isThrusting);
					MinigameManager.GetActive().StopSFX(mg_jr_Sound.JETPACK_LOOP.ClipName());
					MinigameManager.GetActive().PlaySFX(mg_jr_Sound.JETPACK_LOOP_END.ClipName());
				}
			}
		}

		public void AddFreeRobotPenguin()
		{
			mg_jr_RobotPenguin pooledResourceByComponent = m_miniGame.Resources.GetPooledResourceByComponent<mg_jr_RobotPenguin>(mg_jr_ResourceList.GAME_PREFAB_ROBOT_PENGUIN);
			pooledResourceByComponent.Start();
			if (!AddRobotPenguin(pooledResourceByComponent))
			{
				m_miniGame.Resources.ReturnPooledResource(pooledResourceByComponent.gameObject);
			}
		}

		private bool RemoveRobotPenguin(bool _explode = true)
		{
			bool result = false;
			if (m_guardianRoboPenguins.Count != 0)
			{
				mg_jr_RobotPenguin mg_jr_RobotPenguin = m_guardianRoboPenguins[m_guardianRoboPenguins.Count - 1];
				m_guardianRoboPenguins.Remove(mg_jr_RobotPenguin);
				mg_jr_RobotPenguin.Collector.CoinCollected -= OnCoinsCollected;
				mg_jr_RobotPenguin.Collector.RobotPenguinCollected -= OnRobotPenguinCollected;
				mg_jr_RobotPenguin.Collector.TurboCollected -= OnTuboCollected;
				mg_jr_RobotPenguin.ObstacleDestroyer.ObstacleDestroyed -= OnCoinsCollected;
				mg_jr_RobotPenguin.ToggleTurboAnimation(false);
				if (_explode)
				{
					mg_jr_RobotPenguin.Explode();
				}
				else
				{
					mg_jr_RobotPenguin.Recycle();
				}
				result = true;
				m_miniGame.GoalManager.AddToProgress(mg_jr_Goal.GoalType.LOSE_ROBOTS, 1f);
			}
			return result;
		}

		private bool AddRobotPenguin(mg_jr_RobotPenguin _theRoboPenguin)
		{
			bool result = false;
			if (m_guardianRoboPenguins.Count < 3)
			{
				_theRoboPenguin.transform.parent = base.transform.parent;
				GameObject gameObject = base.gameObject;
				if (m_currentState == PenguinState.FLYING_TURBO)
				{
					Vector3 localPosition = m_robotTurboPositions[m_guardianRoboPenguins.Count].localPosition;
					_theRoboPenguin.MakeFollower(gameObject, localPosition, true);
					_theRoboPenguin.ObstacleDestroyer.enabled = true;
				}
				else
				{
					Vector3 localPosition = new Vector3(0f - m_penguinData.RobotFollowDistance, 0f, 0f);
					if (m_guardianRoboPenguins.Count > 0)
					{
						gameObject = m_guardianRoboPenguins[m_guardianRoboPenguins.Count - 1].gameObject;
					}
					_theRoboPenguin.MakeFollower(gameObject, localPosition);
				}
				m_guardianRoboPenguins.Add(_theRoboPenguin);
				result = true;
				m_miniGame.GoalManager.AddToProgress(mg_jr_Goal.GoalType.COLLECT_ROBOTS, 1f);
				_theRoboPenguin.Collector.CoinCollected += OnCoinsCollected;
				_theRoboPenguin.Collector.RobotPenguinCollected += OnRobotPenguinCollected;
				_theRoboPenguin.Collector.TurboCollected += OnTuboCollected;
				_theRoboPenguin.ObstacleDestroyer.ObstacleDestroyed += OnCoinsCollected;
				_theRoboPenguin.ToggleTurboAnimation(m_currentState == PenguinState.FLYING_TURBO);
				if (m_isBlinking)
				{
					float phase = m_blinker.CurrentPhase();
					_theRoboPenguin.Blinker.StartBlinkingFromPhase(phase);
				}
			}
			return result;
		}

		private void RobotPenguinsToNormal()
		{
			Vector3 offset = new Vector3(0f - m_penguinData.RobotFollowDistance, 0f, 0f);
			for (int i = 0; i < m_guardianRoboPenguins.Count; i++)
			{
				mg_jr_RobotPenguin mg_jr_RobotPenguin = m_guardianRoboPenguins[i];
				if (i == 0)
				{
					mg_jr_RobotPenguin.MakeFollower(base.gameObject, offset);
				}
				else
				{
					mg_jr_RobotPenguin.MakeFollower(m_guardianRoboPenguins[i - 1].gameObject, offset);
				}
				mg_jr_RobotPenguin.ToggleTurboAnimation(false);
			}
		}

		private void RobotPenguinsToTurbo()
		{
			int num = 0;
			foreach (mg_jr_RobotPenguin guardianRoboPenguin in m_guardianRoboPenguins)
			{
				guardianRoboPenguin.MakeFollower(base.gameObject, m_robotTurboPositions[num++].localPosition, true);
				guardianRoboPenguin.ToggleTurboAnimation(true);
			}
		}

		public bool HasRobotPenguins()
		{
			return m_guardianRoboPenguins.Count != 0;
		}

		private void StartBlinking(float _forTime)
		{
			Assert.IsTrue(_forTime > 0f, "Blinking duration must be greater than 0");
			m_blinker.StartBlinking();
			foreach (mg_jr_RobotPenguin guardianRoboPenguin in m_guardianRoboPenguins)
			{
				guardianRoboPenguin.Blinker.StartBlinking();
			}
			m_isBlinking = true;
			m_blinkTimeRemaining = _forTime;
		}

		private void StopBlinking()
		{
			m_blinker.StopBlinking();
			foreach (mg_jr_RobotPenguin guardianRoboPenguin in m_guardianRoboPenguins)
			{
				guardianRoboPenguin.Blinker.StopBlinking();
			}
			m_isBlinking = false;
			m_blinkTimeRemaining = 0f;
		}

		private void BecomeInvincible()
		{
			m_isInvincible = true;
			m_isInvinciblityTimeLimited = false;
			m_invinciblityTimeRemaining = 0f;
		}

		private void BecomeInvincible(float _forTime)
		{
			m_isInvincible = true;
			m_isInvinciblityTimeLimited = true;
			m_invinciblityTimeRemaining = _forTime;
		}

		private void BecomeVulnerable()
		{
			m_isInvincible = false;
			m_isInvinciblityTimeLimited = false;
			m_invinciblityTimeRemaining = 0f;
		}

		private void EnableObstacleDestroyer()
		{
			m_obstacleDestroyer.enabled = true;
			foreach (mg_jr_RobotPenguin guardianRoboPenguin in m_guardianRoboPenguins)
			{
				guardianRoboPenguin.ObstacleDestroyer.enabled = true;
			}
		}

		private void DisableObstacleDestroyer()
		{
			m_obstacleDestroyer.enabled = false;
			foreach (mg_jr_RobotPenguin guardianRoboPenguin in m_guardianRoboPenguins)
			{
				guardianRoboPenguin.ObstacleDestroyer.enabled = false;
			}
		}

		public void OnTouchDrag(Vector2 _position)
		{
			m_lastTouchPositionInWorld = m_mainCamera.ScreenToWorldPoint(new Vector3(_position.x, _position.y, 0f));
		}

		public void OnTouchPress(bool _isDown, Vector2 _position)
		{
			m_lastTouchPositionInWorld = m_mainCamera.ScreenToWorldPoint(new Vector3(_position.x, _position.y, 0f));
			switch (m_currentState)
			{
			case PenguinState.ON_PLATFORM:
			case PenguinState.TAKING_OFF:
			case PenguinState.DYING:
			case PenguinState.DEAD:
			case PenguinState.TRANSITIONING:
				break;
			case PenguinState.FLYING_TURBO:
				break;
			case PenguinState.FLYING:
				SetThrusting(_isDown);
				break;
			default:
				Assert.IsTrue(false, "Unhandled penguin state in on touch");
				break;
			}
		}

		private void OnCoinsCollected(int _numberCollected)
		{
			CoinsCollected += _numberCollected;
			CoinsCollectedSession += _numberCollected;
			m_miniGame.GoalManager.AddToProgress(mg_jr_Goal.GoalType.COLLECT_COINS, _numberCollected);
			m_miniGame.CoinsEarned += _numberCollected;
		}

		private void OnRobotPenguinCollected(mg_jr_RobotPenguin _theRoboPenguin)
		{
			if (!AddRobotPenguin(_theRoboPenguin))
			{
				_theRoboPenguin.Recycle();
			}
		}

		private void OnTuboCollected(int _numberCollected)
		{
			TurboDevice.AdjustTurboPoints(_numberCollected);
		}
	}
}
