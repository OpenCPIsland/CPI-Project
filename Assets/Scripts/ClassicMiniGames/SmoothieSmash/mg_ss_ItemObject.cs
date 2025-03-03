using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemObject : MonoBehaviour
	{
		protected mg_ss_GameLogic m_logic;

		private float m_screenHalfWidth;

		private Animator m_animator;

		private SpriteRenderer m_renderer;

		protected mg_ss_IItemMovement m_movement;

		private mg_ss_EItemAnimation m_animation;

		public mg_ss_EItemTypes ItemType
		{
			get;
			protected set;
		}

		public mg_ss_ECollideState CollideState
		{
			get;
			protected set;
		}

		public bool Collidable
		{
			get
			{
				return CollideState == mg_ss_ECollideState.NONE;
			}
		}

		public bool ItemFinished
		{
			get;
			private set;
		}

		public virtual bool ChaosItem
		{
			get
			{
				return false;
			}
		}

		public virtual bool PowerUp
		{
			get
			{
				return false;
			}
		}

		public virtual bool ToeStub
		{
			get
			{
				return false;
			}
		}

		protected mg_ss_EItemAnimation Animation
		{
			get
			{
				return m_animation;
			}
			set
			{
				m_animator.SetInteger("animation", (int)value);
				m_animation = value;
			}
		}

		protected virtual void Awake()
		{
			m_logic = MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic;
			m_animator = GetComponentInChildren<Animator>();
			m_renderer = GetComponentInChildren<SpriteRenderer>();
		}

		public virtual void Initialize(mg_ss_EItemTypes p_itemType, mg_ss_IItemMovement p_movement, Vector2 p_spawnPointBottom, Vector2 p_spawnPointTop, float p_screenHalfWidth, bool p_chaosItem)
		{
			m_movement = p_movement;
			m_screenHalfWidth = p_screenHalfWidth;
			ItemFinished = false;
			ItemType = p_itemType;
			CollideState = mg_ss_ECollideState.NONE;
			Animation = mg_ss_EItemAnimation.IDLE;
			SetInitialPosition(p_spawnPointBottom);
		}

		public virtual void OnDestroy()
		{
		}

		protected void SetInitialPosition(Vector2 p_position)
		{
			base.transform.position = p_position;
			m_movement.Initialize(this);
		}

		public virtual void UpdatePosition(float p_deltaTime, float p_conveyorSpeed)
		{
			m_movement.UpdatePosition(base.transform, p_deltaTime, p_conveyorSpeed);
		}

		public virtual void MinigameUpdate(float p_deltaTime, float p_conveyorSpeed)
		{
			UpdatePosition(p_deltaTime, p_conveyorSpeed);
			CheckOffScreen();
		}

		protected virtual void CheckOffScreen()
		{
			ItemFinished = (base.transform.position.x + m_renderer.bounds.size.x < 0f - m_screenHalfWidth);
		}

		public void RemoveItem()
		{
			ItemFinished = true;
		}

		public virtual void OnCollided(mg_ss_EPlayerAction p_playerAction)
		{
			UpdateCollideState(p_playerAction);
			m_movement.OnCollided();
		}

		private void UpdateCollideState(mg_ss_EPlayerAction p_playerAction)
		{
			if (p_playerAction == mg_ss_EPlayerAction.SMASHING)
			{
				CollideState = mg_ss_ECollideState.SMASH;
			}
			else
			{
				CollideState = mg_ss_ECollideState.NORMAL;
			}
		}

		public bool IsItemOnConveyor()
		{
			return m_logic.ItemManager.IsItemOnConveyor(this);
		}

		public void PlaceOnConveyor()
		{
			Vector2 v = base.transform.position;
			v.y = m_logic.ItemManager.ConveyorPosY;
			base.transform.position = v;
		}

		public void Bounce()
		{
			m_movement = new mg_ss_ItemMovement_Bounce(CalculateRandomBounceVelocity());
			m_movement.Initialize(this);
		}

		public virtual void PlayBounceSFX()
		{
		}

		public static float CalculateRandomBounceVelocity()
		{
			return Random.Range(8f, 10f);
		}

		public virtual void ShowHighlight(bool p_show)
		{
		}

		public override string ToString()
		{
			return ItemType.ToString();
		}
	}
}
