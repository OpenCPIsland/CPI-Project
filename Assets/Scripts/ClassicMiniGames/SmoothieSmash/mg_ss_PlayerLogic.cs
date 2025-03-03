using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_PlayerLogic
	{
		private Camera m_mainCamera;

		private mg_ss_GameLogic m_logic;

		private mg_ss_PlayerObject m_playerObject;

		private mg_ss_InputManager m_inputManager;

		private SmoothieSmashInputObserver InputObserver;

		private bool m_conveyorCollidedWith;

		private mg_ss_EPlayerState playerState;

		private float m_swipeTimer;

		private float m_velocity;

		private float m_velocityAdj;

		public bool ConveyorCollidedWith
		{
			get
			{
				return m_conveyorCollidedWith;
			}
			set
			{
				m_conveyorCollidedWith = value;
				if (m_conveyorCollidedWith && m_logic.ChaosModeActivated)
				{
					SetCollision(mg_ss_EPlayerState.COLLISION);
					m_conveyorCollidedWith = false;
				}
			}
		}

		public mg_ss_EPlayerState State
		{
			get
			{
				return playerState;
			}
			private set
			{
				Debug.Log(string.Concat("State changing from ", playerState, " to ", value));
				playerState = value;
			}
		}

		public mg_ss_EPlayerAction Action
		{
			get;
			private set;
		}

		public Vector2 WorldTouch
		{
			get;
			private set;
		}

		public float Velocity
		{
			get
			{
				return m_velocity;
			}
		}

		private bool AbleToSmash
		{
			get
			{
				return Action != mg_ss_EPlayerAction.SMASHING && m_swipeTimer >= 0.3f && m_logic.GameState == mg_ss_EGameState.ACTIVE && (State == mg_ss_EPlayerState.JUMPING || State == mg_ss_EPlayerState.FALLING);
			}
		}

		public Vector2 ConveyorWorldPosition
		{
			get
			{
				return m_logic.ConveyorWorldPosition;
			}
		}

		public mg_ss_PlayerLogic()
		{
			m_mainCamera = MinigameManager.GetActive().MainCamera;
			m_inputManager = new mg_ss_InputManager(MinigameManager.GetActive().MainCamera, this);
			InputObserver = MinigameManager.GetActive<mg_SmoothieSmash>().InputObserver;
		}

		public void Initialize(mg_ss_GameScreen p_screen, mg_ss_GameLogic p_logic)
		{
			m_logic = p_logic;
			m_velocity = 3.2f;
			State = mg_ss_EPlayerState.NONE;
			Action = mg_ss_EPlayerAction.NONE;
			m_playerObject = p_screen.PlayerObject;
			m_playerObject.Initialize(this, p_screen.GameZoneLeft, p_screen.GameZoneRight);
		}

		private void Reset()
		{
			m_velocity = 3.2f;
			State = mg_ss_EPlayerState.NONE;
			Action = mg_ss_EPlayerAction.NONE;
			ConveyorCollidedWith = false;
			m_playerObject.Reset();
		}

		public void Destroy()
		{
			m_inputManager.TidyUp();
			m_inputManager = null;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			switch (State)
			{
			case mg_ss_EPlayerState.DYING_COLLISION:
				break;
			case mg_ss_EPlayerState.NONE:
				Update_None();
				break;
			case mg_ss_EPlayerState.DYING:
				Update_Dying(p_deltaTime);
				break;
			default:
				Update_Default(p_deltaTime);
				break;
			}
		}

		private void Update_Default(float p_deltaTime)
		{
			if (ConveyorCollidedWith)
			{
				OnConveyorCollision();
				return;
			}
			m_swipeTimer += p_deltaTime;
			UpdateXDestination(p_deltaTime);
			AdjustPosY(p_deltaTime);
			CheckJumpExtreme();
		}

		private void Update_None()
		{
			if (m_velocity >= 0f)
			{
				State = mg_ss_EPlayerState.JUMPING;
				return;
			}
			State = mg_ss_EPlayerState.FALLING;
			m_playerObject.SetFalling();
		}

		private void Update_Dying(float p_deltaTime)
		{
			m_playerObject.UpdateDyingScale(p_deltaTime);
		}

		private void AdjustPosY(float p_deltaTime)
		{
			AdjustVelocity(p_deltaTime);
			m_playerObject.UpdatePosY(p_deltaTime);
		}

		private void UpdateXDestination(float p_deltaTime)
		{
			mg_ss_InputTouch touch = m_inputManager.GetTouch();
			if (touch != null)
			{
				WorldTouch = m_mainCamera.ScreenToWorldPoint(touch.Position);
				m_playerObject.UpdatePosX(p_deltaTime);
			}
			if (InputObserver.CurrentSteering != Vector2.zero)
			{
				m_playerObject.UpdatePosX(p_deltaTime, InputObserver.CurrentSteering);
			}
		}

		private void AdjustVelocity(float p_deltaTime)
		{
			if (State != mg_ss_EPlayerState.COLLISION)
			{
				float num = 0f;
				if (Action == mg_ss_EPlayerAction.SMASHING)
				{
					num = 13.5f;
				}
				m_velocity -= m_velocityAdj;
				m_velocity -= (5f + num) * p_deltaTime;
				m_velocityAdj = 0f;
			}
			else
			{
				m_velocity = 0f;
			}
		}

		private void CheckJumpExtreme()
		{
			if (m_velocity <= 0f && State == mg_ss_EPlayerState.JUMPING)
			{
				State = mg_ss_EPlayerState.FALLING;
				m_playerObject.SetFalling();
			}
		}

		public void OnTriggerEnter2D(Collider2D p_other)
		{
			if (State == mg_ss_EPlayerState.JUMPING || State == mg_ss_EPlayerState.FALLING)
			{
				CheckItemCollision(p_other);
			}
		}

		public void OnTriggerStay2D(Collider2D p_other)
		{
			if (State == mg_ss_EPlayerState.JUMPING || State == mg_ss_EPlayerState.FALLING)
			{
				CheckItemCollision(p_other);
			}
		}

		private void CheckItemCollision(Collider2D p_other)
		{
			mg_ss_ItemObject component = p_other.GetComponent<mg_ss_ItemObject>();
			if (component != null && component.Collidable)
			{
				if (component.ToeStub)
				{
					Jump();
					m_playerObject.PlayToeStub();
				}
				else if (State == mg_ss_EPlayerState.FALLING && (component.IsItemOnConveyor() || (!component.ChaosItem && !component.PowerUp)))
				{
					SetCollision(mg_ss_EPlayerState.COLLISION);
				}
				if (component.IsItemOnConveyor())
				{
					m_swipeTimer = 0f;
				}
				component.OnCollided(Action);
				switch (component.ItemType)
				{
				case mg_ss_EItemTypes.PINEAPPLE:
					component.transform.position -= new Vector3(0f, 0.5f, 0f);
					break;
				case mg_ss_EItemTypes.BLUEBERRY:
					component.transform.position -= new Vector3(0f, 1f, 0f);
					break;
				default:
					component.transform.position -= new Vector3(0f, 0.25f, 0f);
					break;
				}
				ConveyorCollidedWith = false;
			}
		}

		private void SetCollision(mg_ss_EPlayerState p_collisionState)
		{
			State = p_collisionState;
			m_playerObject.SetCollision();
		}

		public void OnConveyorCollision()
		{
			SetCollision(mg_ss_EPlayerState.DYING_COLLISION);
			m_logic.OnConveyorCollision();
		}

		public void CollisionAnimFinished()
		{
			if (State == mg_ss_EPlayerState.DYING_COLLISION)
			{
				Kill();
			}
			else
			{
				Jump();
			}
		}

		private void Jump()
		{
			State = mg_ss_EPlayerState.JUMPING;
			m_velocity = 5.5f;
			Action = mg_ss_EPlayerAction.NONE;
			m_swipeTimer = 0f;
			m_velocityAdj = 0f;
		}

		public void StartSmashing()
		{
			if (AbleToSmash)
			{
				m_velocityAdj = 4.8f;
				Action = mg_ss_EPlayerAction.SMASHING;
				State = mg_ss_EPlayerState.FALLING;
				m_playerObject.SetFalling();
			}
		}

		public void Kill()
		{
			State = mg_ss_EPlayerState.DYING;
			m_playerObject.SetDying();
		}

		public void FinishedDying()
		{
			Reset();
			m_logic.PlayerDied();
		}
	}
}
