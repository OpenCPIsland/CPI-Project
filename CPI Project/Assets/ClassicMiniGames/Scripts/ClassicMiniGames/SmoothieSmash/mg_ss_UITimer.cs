using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothieSmash
{
	public class mg_ss_UITimer : MonoBehaviour
	{
		private mg_ss_GameLogic_Normal m_logic;

		private int m_displaySeconds;

		private Text m_text;

		private mg_ss_ETimerState m_state;

		private Animator m_animator;

		private float m_delayTotal;

		private float m_delayTimer;

		protected void Awake()
		{
			m_state = mg_ss_ETimerState.DELAY;
			m_text = GetComponentInChildren<Text>();
			m_animator = GetComponentInChildren<Animator>();
		}

		protected void Start()
		{
			m_logic = (MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic as mg_ss_GameLogic_Normal);
			m_delayTotal = 1f;
		}

		protected void Update()
		{
			if (m_state == mg_ss_ETimerState.DELAY)
			{
				m_delayTimer += Time.deltaTime;
				if (m_delayTimer >= m_delayTotal)
				{
					m_state = mg_ss_ETimerState.ACTIVE;
					m_animator.enabled = true;
					m_animator.Play("mg_ss_Timer", 0, (m_delayTimer - m_delayTotal) / 1f);
				}
			}
			if ((int)m_logic.GameTimer != m_displaySeconds)
			{
				int num = (int)(m_logic.GameTimer / 60f);
				string text = string.Format(arg1: Mathf.Max(0, (int)(m_logic.GameTimer % 60f)).ToString("00"), format: "{0}:{1}", arg0: num);
				m_text.text = text;
				m_displaySeconds = (int)m_logic.GameTimer;
				UpdateColor();
				if (m_logic.GameTimer <= 15f)
				{
					MinigameManager.GetActive().PlaySFX("mg_ss_sfx_countdown_timer");
				}
			}
		}

		private void UpdateColor()
		{
			Color color = m_text.color;
			if (m_logic.GameTimer <= 15f)
			{
				color.b = 0f;
				color.g = 0f;
			}
			else
			{
				color.b = 255f;
				color.g = 255f;
			}
			m_text.color = color;
		}
	}
}
