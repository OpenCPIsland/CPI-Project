using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace SmoothieSmash
{
	public class mg_ss_UICountdown : MonoBehaviour
	{
		private static string ANIM_PARAM_COUNTDOWN = "countdown";

		private mg_ss_GameLogic m_logic;

		private int m_numberDisplayed;

		private Animator m_animator;

		private Image m_image;

		protected void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_image = GetComponent<Image>();
			m_image.enabled = false;
		}

		protected void Start()
		{
			m_logic = MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic;
		}

		protected void Update()
		{
			bool flag = m_logic.GameState == mg_ss_EGameState.COUNTDOWN;
			m_image.enabled = flag;
			if (flag)
			{
				int num = 0;
				float num2 = m_logic.CountdownTotalTime / 3f;
				if (m_logic.CountdownTimer < num2)
				{
					num = 3;
				}
				else if (m_logic.CountdownTimer < num2 * 2f)
				{
					num = 2;
				}
				else if (m_logic.CountdownTimer < num2 * 3f)
				{
					num = 1;
				}
				m_animator.SetInteger(ANIM_PARAM_COUNTDOWN, num);
				UpdateAlpha(m_logic.CountdownTimer % num2, num2);
				if (num != m_numberDisplayed)
				{
					m_numberDisplayed = num;
					MinigameManager.GetActive().PlaySFX("mg_ss_sfx_countdown_timer");
				}
			}
		}

		private void UpdateAlpha(float p_intervalTime, float p_intervalLength)
		{
			float num = p_intervalTime / p_intervalLength;
			Color color = m_image.color;
			color.a = 1f - 1f * num;
			m_image.color = color;
		}
	}
}
