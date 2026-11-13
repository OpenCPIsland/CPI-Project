using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_FishingHook : MonoBehaviour
	{
		private static string ANIM_STATE = "state";

		private static string ANIM_IN_WATER = "in_water";

		private static string ANIM_MOVEMENT = "movement";

		private float m_hookSpeed;

		private mg_if_FishingRod m_rod;

		private mg_if_WormDrop m_wormDrop;

		private mg_if_GameLogic m_logic;

		private Vector2 m_top;

		private Vector2 m_bottom;

		private Animator m_animator;

		private mg_if_Fish m_caughtFish;

		private bool m_hookJustDropped;

		private mg_if_EHookMovement m_movement;

		private mg_if_EHookState m_state;

		public mg_if_EHookState State
		{
			get
			{
				return m_state;
			}
			private set
			{
				m_state = value;
				m_animator.SetInteger(ANIM_STATE, (int)value);
			}
		}

		public bool IsAboveWater
		{
			get
			{
				return !m_animator.GetBool(ANIM_IN_WATER);
			}
		}

		public bool IsMoving
		{
			get
			{
				return m_movement != mg_if_EHookMovement.NOT_MOVING;
			}
		}

		public bool IsBroken
		{
			get
			{
				return State == mg_if_EHookState.NO_HOOK || State == mg_if_EHookState.NO_WORM || State == mg_if_EHookState.SHOCKED;
			}
		}

		public void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
			SetMovement(mg_if_EHookMovement.NOT_MOVING);
			State = mg_if_EHookState.BAITED;
			m_top = base.transform.localPosition;
			m_hookSpeed = MinigameManager.GetActive<mg_IceFishing>().Resources.Variables.HookSpeed;
		}

		public void Start()
		{
			m_logic = MinigameManager.GetActive<mg_IceFishing>().Logic;
			m_bottom = base.transform.parent.Find("mg_if_BottomCheck").localPosition;
		}

		public void Initialize(mg_if_FishingRod p_rod, mg_if_WormDrop p_wormDrop)
		{
			m_rod = p_rod;
			m_wormDrop = p_wormDrop;
			m_wormDrop.gameObject.SetActive(false);
		}

		public void OnTriggerEnter2D(Collider2D p_other)
		{
			if (p_other.gameObject.name == "mg_if_OutOfWaterCheck")
			{
				m_animator.SetBool(ANIM_IN_WATER, false);
			}
			else if (State == mg_if_EHookState.BAITED && !m_logic.GameOver)
			{
				mg_if_Fish componentInParent = p_other.GetComponentInParent<mg_if_Fish>();
				if (componentInParent != null && componentInParent.FishState == mg_if_EFishState.STATE_SWIMMING)
				{
					State = mg_if_EHookState.CAUGHT_YELLOW;
					m_caughtFish = componentInParent;
					m_caughtFish.OnHooked(this);
				}
			}
			else
			{
				CheckObstacleCollision(p_other);
			}
		}

		public void OnTriggerStay2D(Collider2D p_other)
		{
			CheckObstacleCollision(p_other);
		}

		public void OnTriggerExit2D(Collider2D p_other)
		{
			if (p_other.gameObject.name == "mg_if_OutOfWaterCheck")
			{
				m_animator.SetBool(ANIM_IN_WATER, true);
			}
		}

		public void MinigameUpdate(float p_deltaTime, Vector2 p_destination)
		{
			m_animator.SetInteger(ANIM_MOVEMENT, (int)m_movement);
			UpdatePosition(p_deltaTime, p_destination);
		}

		private void CheckObstacleCollision(Collider2D p_other)
		{
			if (!IsBroken && p_other.name != "object_size")
			{
				mg_if_ObstacleHook componentInParent = p_other.GetComponentInParent<mg_if_ObstacleHook>();
				if (componentInParent != null)
				{
					componentInParent.OnObstacleHitHook(m_rod);
				}
			}
		}

		private void UpdatePosition(float p_deltaTime, Vector2 p_destination)
		{
			mg_if_EHookMovement mg_if_EHookMovement = mg_if_EHookMovement.NOT_MOVING;
			if (!Mathf.Approximately(p_destination.y, base.transform.localPosition.y))
			{
				Vector2 v = base.transform.localPosition;
				if (p_destination.y > base.transform.localPosition.y)
				{
					v.y = Mathf.Min(m_top.y, p_destination.y, base.transform.localPosition.y + m_hookSpeed * p_deltaTime);
					mg_if_EHookMovement = mg_if_EHookMovement.MOVING_UP;
				}
				else
				{
					v.y = Mathf.Max(m_bottom.y, p_destination.y, base.transform.localPosition.y - m_hookSpeed * p_deltaTime);
					mg_if_EHookMovement = mg_if_EHookMovement.MOVING_DOWN;
				}
				if (m_hookJustDropped)
				{
					v.y = p_destination.y;
					m_hookJustDropped = false;
				}
				if ((mg_if_EHookMovement == mg_if_EHookMovement.MOVING_DOWN && v.y < p_destination.y) || (mg_if_EHookMovement == mg_if_EHookMovement.MOVING_UP && v.y > p_destination.y))
				{
					v.y = p_destination.y;
					mg_if_EHookMovement = mg_if_EHookMovement.NOT_MOVING;
				}
				else if (Mathf.Approximately(v.y, m_bottom.y) || Mathf.Approximately(v.y, m_top.y))
				{
					mg_if_EHookMovement = mg_if_EHookMovement.NOT_MOVING;
				}
				base.transform.localPosition = v;
			}
			SetMovement(mg_if_EHookMovement);
		}

		public void CatchFish()
		{
			if (m_caughtFish != null)
			{
				m_caughtFish.OnCaught();
				m_rod.OnFishCaught();
				m_caughtFish = null;
				State = mg_if_EHookState.BAITED;
			}
		}

		public void ReleaseFish()
		{
			if (m_caughtFish != null)
			{
				m_caughtFish.OnReleased();
				m_caughtFish = null;
				State = mg_if_EHookState.BAITED;
			}
		}

		public void Shock()
		{
			ReleaseFish();
			State = mg_if_EHookState.SHOCKED;
		}

		public void StopShock()
		{
			State = mg_if_EHookState.NO_WORM;
		}

		public void BaitHook()
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_WormAdd");
			State = mg_if_EHookState.BAITED;
		}

		public void DropHook()
		{
			ReleaseFish();
			State = mg_if_EHookState.NO_HOOK;
			m_animator.SetBool(ANIM_IN_WATER, false);
			m_wormDrop.DropWorm(base.transform.position);
			m_hookJustDropped = true;
		}

		private void SetMovement(mg_if_EHookMovement p_newMovement)
		{
			if (p_newMovement != m_movement)
			{
				m_movement = p_newMovement;
				StopSFX();
				if (m_movement == mg_if_EHookMovement.MOVING_DOWN)
				{
					MinigameManager.GetActive().PlaySFX("mg_if_sfx_LineLowerLoop");
				}
				else if (m_movement == mg_if_EHookMovement.MOVING_UP)
				{
					MinigameManager.GetActive().PlaySFX("mg_if_sfx_LineRaiseLoop");
				}
			}
		}

		private void StopSFX()
		{
			MinigameManager.GetActive().StopSFX("mg_if_sfx_LineLowerLoop");
			MinigameManager.GetActive().StopSFX("mg_if_sfx_LineRaiseLoop");
		}
	}
}
