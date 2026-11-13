using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ConveyorLogic
	{
		private mg_ss_ConveyorSpeedData m_speedData;

		private mg_ss_ConveyorTimeData m_timeData;

		private float m_speedMultiplier;

		private mg_ss_ConveyorObject m_conveyorObject;

		public float ItemSpacingTime
		{
			get
			{
				return m_speedData.ItemSpacing;
			}
		}

		public float BaseSpeed
		{
			get
			{
				return m_speedData.BaseSpeed;
			}
		}

		public Transform Conveyor
		{
			get
			{
				return m_conveyorObject.transform;
			}
		}

		public Transform ItemSpawnPoint_Top
		{
			get
			{
				return m_conveyorObject.ItemSpawnPoint_Top;
			}
		}

		public Transform ItemSpawnPoint_Bottom
		{
			get
			{
				return m_conveyorObject.ItemSpawnPoint_Bottom;
			}
		}

		public Vector2 ConveyorWorldPosition
		{
			get
			{
				return m_conveyorObject.ConveyorPosition.position;
			}
		}

		public float CurrentSpeed
		{
			get
			{
				float a = m_timeData.Speed * m_speedMultiplier;
				return Mathf.Max(a, 0.1f);
			}
		}

		public void Initialize(mg_ss_GameScreen p_screen, mg_ss_ConveyorSpeedData p_speedData)
		{
			m_speedData = p_speedData;
			m_timeData = m_speedData.TimeDataHead;
			m_conveyorObject = p_screen.ConveyorObject;
			m_speedMultiplier = 1f;
		}

		public void OnConveyorCollision()
		{
			m_conveyorObject.OnConveyorCollision();
			MinigameManager.GetActive().PlaySFX("mg_ss_sfx_player_conveyor_impact");
		}

		public void CheckSpeedIncrease(float p_gameTime)
		{
			if (m_timeData.NextData != null && m_timeData.NextData.MinTime <= p_gameTime)
			{
				m_timeData = m_timeData.NextData;
				DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Conveyor Speed Tier Increase " + m_timeData.Speed, DisneyMobile.CoreUnitySystems.Logger.TagFlags.GAME);
			}
		}

		public void IncreaseSpeedMultiplier()
		{
			m_speedMultiplier += m_speedData.SpeedRegainPercent / 100f;
			m_speedMultiplier = Mathf.Min(m_speedMultiplier, 1f);
		}

		public void DropSpeed()
		{
			m_speedMultiplier = m_speedData.MinSpeedPercent / 100f;
		}
	}
}
