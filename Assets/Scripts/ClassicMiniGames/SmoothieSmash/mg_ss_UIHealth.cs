using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothieSmash
{
	public class mg_ss_UIHealth : MonoBehaviour
	{
		private mg_ss_GameLogic_Survival m_logic;

		private int m_health;

		private Image m_healthBar;

		private mg_ss_EHealthState m_state;

		private Animator m_animator;

		private float m_delayTotal;

		private float m_delayTimer;

		protected void Awake()
		{
			m_state = mg_ss_EHealthState.DELAY;
			m_healthBar = base.transform.Find("bar").GetComponent<Image>();
			m_animator = GetComponentInChildren<Animator>();
		}

		protected void Start()
		{
			m_logic = (MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic as mg_ss_GameLogic_Survival);
			m_delayTotal = 1f;
		}

		protected void Update()
		{
			if (m_state == mg_ss_EHealthState.DELAY)
			{
				m_delayTimer += Time.deltaTime;
				if (m_delayTimer >= m_delayTotal)
				{
					m_state = mg_ss_EHealthState.ACTIVE;
					m_animator.enabled = true;
					m_animator.Play("mg_ss_HealthBar", 0, (m_delayTimer - m_delayTotal) / 1f);
				}
			}
			if (m_health != m_logic.Health)
			{
				m_health = m_logic.Health;
				float percentage = (float)m_health / 100f;
				SetPercentage(percentage);
			}
		}

		private void SetPercentage(float p_percentageFill)
		{
			m_healthBar.fillAmount = p_percentageFill;
		}
	}
}
