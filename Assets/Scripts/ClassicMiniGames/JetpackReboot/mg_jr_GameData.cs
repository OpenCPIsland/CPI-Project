using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_GameData : MonoBehaviour
	{
		private float m_unitsPerMeter = 1.28f;

		private float m_startingSpeed = 2.5f;

		private float m_maxSpeed = 5.08f;

		private float m_turboSpeed = 7.42f;

		private float m_accellerationBetweenNormalAndTurbo = 3.5f;

		private float m_distancePenaltyAsPercentageOfDistance = 0.5f;

		private float m_distanceReachMaxSpeed = 665f;

		private float m_distanceDifficulty = 100f;

		private float m_distanceTransition = 130f;

		private float m_noTurboBeforeTransitionDistance = 15f;

		private float m_emptySpaceAfterTurbo = 10f;

		private float m_emptySpaceAtStartOfLevel = 16f;

		private float m_coinsForBonusRobot = 350f;

		private int m_turboLevelsPerTurboRun = 4;

		private float m_turboCameraZoomRate = 0.75f;

		private bool m_enforceMaxSpeed = false;

		public float StartingSpeed
		{
			get
			{
				return MetersToUnits(m_startingSpeed);
			}
		}

		public float MaxSpeed
		{
			get
			{
				return MetersToUnits(m_maxSpeed);
			}
		}

		public float TurboSpeed
		{
			get
			{
				return MetersToUnits(m_turboSpeed);
			}
		}

		public float AccellerationBetweenNormalAndTurbo
		{
			get
			{
				return m_accellerationBetweenNormalAndTurbo;
			}
		}

		public float DistancePenaltyPercentage
		{
			get
			{
				return m_distancePenaltyAsPercentageOfDistance;
			}
		}

		public float DistanceToReachMaxSpeed
		{
			get
			{
				return m_distanceReachMaxSpeed;
			}
		}

		public float DistanceDifficulty
		{
			get
			{
				return m_distanceDifficulty;
			}
		}

		public float EnvironmentDistanceBeforeTransition
		{
			get
			{
				return m_distanceTransition;
			}
		}

		public float NoTurboBeforeTransitionDistance
		{
			get
			{
				return m_noTurboBeforeTransitionDistance;
			}
		}

		public float EmptySpaceAfterTurbo
		{
			get
			{
				return m_emptySpaceAfterTurbo * m_unitsPerMeter;
			}
		}

		public float EmptySpaceAtStartOfLevel
		{
			get
			{
				return MetersToUnits(m_emptySpaceAtStartOfLevel);
			}
		}

		public float CoinsForBonusRobot
		{
			get
			{
				return m_coinsForBonusRobot;
			}
		}

		public int TurboLevelsPerTurboRun
		{
			get
			{
				return m_turboLevelsPerTurboRun;
			}
		}

		public float TurboCameraZoomRate
		{
			get
			{
				return m_turboCameraZoomRate;
			}
		}

		public bool EnforceMaxSpeed
		{
			get
			{
				return m_enforceMaxSpeed;
			}
		}

		public float UnitsToMeters(float _units)
		{
			return _units / m_unitsPerMeter;
		}

		public float MetersToUnits(float _meters)
		{
			return _meters * m_unitsPerMeter;
		}
	}
}
