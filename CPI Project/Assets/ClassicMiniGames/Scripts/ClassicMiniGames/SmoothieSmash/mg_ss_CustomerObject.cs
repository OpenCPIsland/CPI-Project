using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_CustomerObject : MonoBehaviour
	{
		private static string ANIM_PARAM_STATE = "state";

		public bool IsSpecial;

		private mg_ss_CustomerManager m_manager;

		private SpriteRenderer m_tint;

		private Animator m_animator;

		private float m_drinkDelayTimer;

		private mg_ss_ECustomerObjectState m_state;

		private mg_ss_ECustomerObjectState State
		{
			get
			{
				return m_state;
			}
			set
			{
				m_state = value;
				m_animator.SetInteger(ANIM_PARAM_STATE, (int)m_state);
			}
		}

		protected void Awake()
		{
			Transform transform = base.transform.Find("tint");
			if (transform != null)
			{
				m_tint = transform.GetComponent<SpriteRenderer>();
			}
			m_animator = GetComponentInChildren<Animator>();
		}

		public void Initialize(mg_ss_CustomerManager p_manager, bool p_isSpecial)
		{
			m_manager = p_manager;
			IsSpecial = p_isSpecial;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (State == mg_ss_ECustomerObjectState.DRINK_DELAY)
			{
				m_drinkDelayTimer += p_deltaTime;
				if (m_drinkDelayTimer >= 1.05f)
				{
					State = mg_ss_ECustomerObjectState.DRINK;
				}
			}
		}

		public void ZipIn()
		{
			if (m_tint != null)
			{
				m_tint.color = MinigameSpriteHelper.RandomPenguinColor();
			}
			m_animator.Play("mg_ss_customer_zipin");
		}

		private void OnZipInCompleted()
		{
			State = mg_ss_ECustomerObjectState.IDLE;
		}

		private void OnZipOutCompleted()
		{
			m_manager.OnCustomerLeft();
		}

		public void OrderCompleted()
		{
			m_drinkDelayTimer = 0f;
			State = mg_ss_ECustomerObjectState.DRINK_DELAY;
		}

		public void StepCompleted()
		{
			State = mg_ss_ECustomerObjectState.CHEER;
		}

		private void OnCheerCompleted()
		{
			State = mg_ss_ECustomerObjectState.IDLE;
		}
	}
}
