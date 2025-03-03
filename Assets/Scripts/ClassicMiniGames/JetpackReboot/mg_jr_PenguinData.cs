using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_PenguinData
	{
		private float m_normalDistanceFromLeft = 2.06f;

		private float m_onTakeOffClimb = 1f;

		private float m_yPosDuringTransition = 0f;

		private float m_distanceReductionFactor = 0.05f;

		private float m_takeOffSpeed = 0.05f;

		private float m_maxVerticalVelocity = 0.17f;

		private float m_minVerticalVelocity = -0.14f;

		private float m_accelerationFactor = 0.0088f;

		private float m_accelerationStartUp = 0.016f;

		private float m_accelerationStartDown = -0.012f;

		private float m_accelerationMaxUp = 0.078f;

		private float m_accelerationMaxDown = -0.04f;

		private float m_distanceToTopOfScreenLimit = 0.43f;

		private float m_distanceToBottomOfScreenLimit = 1.95f;

		private float m_robotFollowDistance = 0.5f;

		private float m_invincibilityDuration = 3f;

		public float NormalDistanceFromLeft
		{
			get
			{
				return m_normalDistanceFromLeft;
			}
		}

		public float OnTakeOffClimb
		{
			get
			{
				return m_onTakeOffClimb;
			}
		}

		public float YPosDuringTransition
		{
			get
			{
				return m_yPosDuringTransition;
			}
		}

		public float DistanceReductionFactor
		{
			get
			{
				return Mathf.Clamp(m_distanceReductionFactor, 0.05f, 1f);
			}
		}

		public float TakeOffSpeed
		{
			get
			{
				return m_takeOffSpeed;
			}
		}

		public float MaxVerticalVelocity
		{
			get
			{
				return m_maxVerticalVelocity;
			}
		}

		public float MinVerticalVelocity
		{
			get
			{
				return m_minVerticalVelocity;
			}
		}

		public float AccelerationFactor
		{
			get
			{
				return m_accelerationFactor;
			}
		}

		public float AccelerationStartUp
		{
			get
			{
				return m_accelerationStartUp;
			}
		}

		public float AaccelerationStartDown
		{
			get
			{
				return m_accelerationStartDown;
			}
		}

		public float AccelerationMaxUp
		{
			get
			{
				return m_accelerationMaxUp;
			}
		}

		public float AccelerationMaxDown
		{
			get
			{
				return m_accelerationMaxDown;
			}
		}

		public float DistanceToTopOfScreenLimit
		{
			get
			{
				return m_distanceToTopOfScreenLimit;
			}
		}

		public float DistanceToBottomOfScreenLimit
		{
			get
			{
				return m_distanceToBottomOfScreenLimit;
			}
		}

		public float RobotFollowDistance
		{
			get
			{
				return m_robotFollowDistance;
			}
		}

		public float InvincibilityDuration
		{
			get
			{
				return m_invincibilityDuration;
			}
		}
	}
}
