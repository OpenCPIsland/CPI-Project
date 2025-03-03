using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_GoldenAppleRotationObject : MonoBehaviour
	{
		private static string ANIM_TRIGGER_SPARKLE = "Sparkle";

		private mg_ss_ERotatingAppleState m_state;

		private mg_ss_ERotatingAppleState m_nextState;

		private Animator m_animator;

		private mg_ss_GoldenApple_FlyInfo m_flyData;

		private mg_ss_GoldenApple_RotateInfo m_rotateData;

		private float m_delayTimer;

		protected void Awake()
		{
			m_animator = GetComponent<Animator>();
			m_state = mg_ss_ERotatingAppleState.FLYING;
		}

		public void Delay(float p_delay)
		{
			m_delayTimer = p_delay;
			if (m_delayTimer >= 0f)
			{
				m_nextState = m_state;
				m_state = mg_ss_ERotatingAppleState.DELAYED;
			}
		}

		public void Fly(mg_ss_GoldenApple_FlyInfo p_flyInfo)
		{
			base.transform.position = p_flyInfo.Start;
			m_flyData = p_flyInfo;
			m_state = mg_ss_ERotatingAppleState.FLYING;
		}

		public void RotateAround(mg_ss_GoldenApple_RotateInfo p_rotateInfo)
		{
			m_rotateData = p_rotateInfo;
		}

		public void MinigameUpdate(float p_deltaTime, Vector2 p_target)
		{
			m_rotateData.RotateAround = p_target;
			switch (m_state)
			{
			case mg_ss_ERotatingAppleState.NONE:
				break;
			case mg_ss_ERotatingAppleState.INVISIBLE:
				base.gameObject.SetActive(false);
				break;
			case mg_ss_ERotatingAppleState.DELAYED:
				Update_Delayed(p_deltaTime);
				break;
			case mg_ss_ERotatingAppleState.FLYING:
				Update_Flying(p_deltaTime);
				break;
			case mg_ss_ERotatingAppleState.ROTATING:
				Update_Rotation(p_deltaTime);
				break;
			case mg_ss_ERotatingAppleState.PARTICLES:
				base.transform.position = m_rotateData.RotateAround;
				break;
			}
		}

		private void Update_Delayed(float p_deltaTime)
		{
			m_delayTimer -= p_deltaTime;
			if (m_delayTimer <= 0f)
			{
				m_state = m_nextState;
				m_nextState = mg_ss_ERotatingAppleState.NONE;
			}
		}

		private void Update_Flying(float p_deltaTime)
		{
			float num = p_deltaTime - m_flyData.RemainingFlyTime;
			m_flyData.RemainingFlyTime -= p_deltaTime;
			if (num >= 0f)
			{
				m_state = mg_ss_ERotatingAppleState.ROTATING;
				Update_Rotation(num);
				return;
			}
			float num2 = 1f - m_flyData.RemainingFlyTime / m_flyData.TotalFlyTime;
			Vector2 v = base.transform.position;
			v.x = m_flyData.Start.x + (m_flyData.Target.x - m_flyData.Start.x) * num2;
			v.y = m_flyData.Start.y + (m_flyData.Target.y - m_flyData.Start.y) * num2;
			v.y -= Mathf.Sin(num2 * (float)Math.PI) * 1.2f;
			base.transform.position = v;
		}

		private void Update_Rotation(float p_delta)
		{
			if (!(p_delta <= 0f))
			{
				float num = (float)Math.PI * 2f;
				float num2 = p_delta / m_rotateData.RotateTime;
				num2 *= num;
				m_rotateData.CurrentAngle += num2;
				if (m_rotateData.MaxTurns > 0f && (m_rotateData.CurrentAngle - m_rotateData.StartingAngle) / num >= m_rotateData.MaxTurns)
				{
					m_state = mg_ss_ERotatingAppleState.PARTICLES;
					m_animator.SetTrigger(ANIM_TRIGGER_SPARKLE);
				}
				Vector2 rotateAround = m_rotateData.RotateAround;
				rotateAround.y -= m_rotateData.Offset;
				rotateAround.x += m_rotateData.Radius * Mathf.Cos(m_rotateData.CurrentAngle);
				rotateAround.y += m_rotateData.Radius * Mathf.Sin(m_rotateData.CurrentAngle);
				base.transform.position = rotateAround;
			}
		}
	}
}
