using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_ScrollingSpeed : MonoBehaviour
	{
		private mg_jr_GameData m_gameBalance = null;

		private mg_jr_Odometer m_distanceData = null;

		private float m_speedModifier = 0f;

		private bool m_isTurboTransitioning = false;

		private float m_differenceCurrentNormalSpeedAndTurboSpeed = 0f;

		private float m_turboSpeedModifier = 0f;

		private float m_turboAccelleration = 0.5f;

		public float SpeedModifier
		{
			get
			{
				return m_speedModifier;
			}
		}

		public bool ScrollingEnabled
		{
			get;
			set;
		}

		public void Init(mg_jr_GameData _balance, mg_jr_Odometer _odo)
		{
			m_gameBalance = _balance;
			m_distanceData = _odo;
		}

		public void Start()
		{
			Assert.NotNull(m_gameBalance, "Scrolling requires an mg_jr_GameBalance component on the same object");
			Assert.NotNull(m_distanceData, "Scrolling requires an mg_jr_Odometer component on the same object");
			Reset();
		}

		public void Reset()
		{
			m_speedModifier = 0f;
			m_turboSpeedModifier = 0f;
			ScrollingEnabled = false;
		}

		public float CurrentSpeedForObstacles()
		{
			return CurrentSpeedFor(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0);
		}

		public float CurrentSpeedFor(mg_jr_SpriteDrawingLayers.DrawingLayers _layer)
		{
			float num = 0f;
			switch (_layer)
			{
			case mg_jr_SpriteDrawingLayers.DrawingLayers.BACKGROUND:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_B:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_A:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.TOP_BORDER:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.BOTTOM_BORDER:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_2:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_3:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_4:
				num = m_speedModifier + m_turboSpeedModifier;
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN_THRUST:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.TINT:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN:
				num = 0f;
				break;
			default:
				Assert.IsTrue(false, "No speed available for layer: " + _layer);
				break;
			}
			return BaseSpeedFor(_layer) + num;
		}

		private float BaseSpeedFor(mg_jr_SpriteDrawingLayers.DrawingLayers _layer)
		{
			float result = 0f;
			switch (_layer)
			{
			case mg_jr_SpriteDrawingLayers.DrawingLayers.BACKGROUND:
				result = m_gameBalance.StartingSpeed * (0.5f / m_gameBalance.StartingSpeed);
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_B:
				result = m_gameBalance.StartingSpeed * (1f / m_gameBalance.StartingSpeed);
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_A:
				result = m_gameBalance.StartingSpeed * (2.5f / m_gameBalance.StartingSpeed);
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.TOP_BORDER:
				result = m_gameBalance.StartingSpeed * (2f / m_gameBalance.StartingSpeed);
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.BOTTOM_BORDER:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_2:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_3:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_4:
				result = m_gameBalance.StartingSpeed;
				break;
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN_THRUST:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.TINT:
			case mg_jr_SpriteDrawingLayers.DrawingLayers.PLAYER_PENGUIN:
				result = 0f;
				break;
			default:
				Assert.IsTrue(false, "No speed available for layer: " + _layer);
				break;
			}
			return result;
		}

		public void EnableTurboSpeed(bool _enable)
		{
			m_isTurboTransitioning = true;
			m_turboAccelleration = m_gameBalance.AccellerationBetweenNormalAndTurbo;
			if (!_enable)
			{
				m_turboAccelleration = 0f - m_turboAccelleration;
			}
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (!ScrollingEnabled)
			{
				return;
			}
			if (m_isTurboTransitioning)
			{
				m_differenceCurrentNormalSpeedAndTurboSpeed = m_gameBalance.TurboSpeed - CurrentSpeedForObstacles();
				m_turboSpeedModifier += m_turboAccelleration * _deltaTime;
				if (m_turboAccelleration > 0f)
				{
					if (m_turboSpeedModifier >= m_differenceCurrentNormalSpeedAndTurboSpeed)
					{
						m_turboSpeedModifier = m_differenceCurrentNormalSpeedAndTurboSpeed;
						m_isTurboTransitioning = false;
					}
				}
				else if (m_turboSpeedModifier <= 0f)
				{
					m_differenceCurrentNormalSpeedAndTurboSpeed = 0f;
					m_isTurboTransitioning = false;
				}
			}
			float num = m_distanceData.DistanceTravelledThisRunAfterPenalties / m_gameBalance.DistanceToReachMaxSpeed;
			if (m_gameBalance.EnforceMaxSpeed)
			{
				num = Mathf.Clamp01(num);
			}
			m_speedModifier = (m_gameBalance.MaxSpeed - m_gameBalance.StartingSpeed) * num;
		}
	}
}
