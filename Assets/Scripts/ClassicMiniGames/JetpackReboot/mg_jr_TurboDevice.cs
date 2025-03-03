using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_TurboDevice : MonoBehaviour
	{
		public delegate void OnTurboPointsChanged(float _fuelPercentage);

		public delegate void OnTurboStateChanged(TurboState _newState);

		public delegate void OnTurboBurnCompleted();

		public enum TurboState
		{
			INACTIVE,
			FILLING,
			FULL,
			ENGAGED
		}

		private TurboState m_tuboCollectionState;

		private float m_turboDuration = 10f;

		private float m_currentTurboPoints = 0f;

		private mg_jr_TurboData m_turboData;

		private float m_turboDrainRate;

		public TurboState DeviceState
		{
			get
			{
				return m_tuboCollectionState;
			}
			private set
			{
				if (m_tuboCollectionState != value)
				{
					m_tuboCollectionState = value;
					if (this.StateChanged != null)
					{
						this.StateChanged(m_tuboCollectionState);
					}
				}
			}
		}

		public float TurboDuration
		{
			get
			{
				return m_turboDuration;
			}
		}

		public float TimeRemainingInTurbo
		{
			get
			{
				float result = 0f;
				if (DeviceState == TurboState.ENGAGED)
				{
					result = FuelPercentage * m_turboDuration;
				}
				return result;
			}
		}

		public float FuelPercentage
		{
			get
			{
				return Mathf.InverseLerp(0f, m_turboData.TurboPointsMax, m_currentTurboPoints);
			}
		}

		public bool IsTurboActive
		{
			get
			{
				return DeviceState == TurboState.ENGAGED;
			}
		}

		public event OnTurboPointsChanged PointsChanged;

		public event OnTurboStateChanged StateChanged;

		public void UpdateTurboBurnTime(float _timeToSpendInTurbo)
		{
			m_turboDrainRate = (0f - m_turboData.TurboPointsMax) / _timeToSpendInTurbo;
		}

		private void Awake()
		{
			m_turboData = new mg_jr_TurboData();
			UpdateTurboBurnTime(m_turboDuration);
			LoadInitialState();
		}

		public void LoadInitialState()
		{
			m_currentTurboPoints = 0f;
			if (this.PointsChanged != null)
			{
				this.PointsChanged(0f);
			}
			ChangeState(TurboState.INACTIVE);
		}

		private void ChangeState(TurboState _newState)
		{
			DeviceState = _newState;
		}

		private void Update()
		{
			if (MinigameManager.IsPaused)
			{
				return;
			}
			switch (m_tuboCollectionState)
			{
			case TurboState.INACTIVE:
				break;
			case TurboState.FULL:
				break;
			case TurboState.FILLING:
				AdjustTurboPoints(Time.deltaTime * m_turboData.TurboPointGainRate);
				if (m_currentTurboPoints >= m_turboData.TurboPointsMax)
				{
					MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_JETPACK_FULL.ClipName());
					m_currentTurboPoints = m_turboData.TurboPointsMax;
					ChangeState(TurboState.FULL);
				}
				break;
			case TurboState.ENGAGED:
				AdjustTurboPoints(Time.deltaTime * m_turboDrainRate);
				break;
			default:
				Assert.IsTrue(false, "Unhandled m_tuboCollectionState");
				break;
			}
		}

		public void BeginTracking()
		{
			ChangeState(TurboState.FILLING);
		}

		public bool ActivateTurbo()
		{
			if (m_tuboCollectionState != TurboState.FULL)
			{
				return false;
			}
			StartCoroutine(DoTurbo());
			return true;
		}

		private IEnumerator DoTurbo()
		{
			ChangeState(TurboState.ENGAGED);
			while (m_currentTurboPoints > 0f && m_tuboCollectionState == TurboState.ENGAGED)
			{
				yield return 0;
			}
			if (DeviceState == TurboState.ENGAGED)
			{
				ChangeState(TurboState.FILLING);
			}
		}

		public void AdjustTurboPoints(float _pointAdjustment)
		{
			if (DeviceState != TurboState.FILLING && _pointAdjustment > 0f)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempted to increase turbo points while not filling State: " + DeviceState);
				return;
			}
			float currentTurboPoints = m_currentTurboPoints;
			m_currentTurboPoints += _pointAdjustment;
			m_currentTurboPoints = Mathf.Clamp(m_currentTurboPoints, 0f, m_turboData.TurboPointsMax);
			if (this.PointsChanged != null && !Mathf.Approximately(currentTurboPoints, m_currentTurboPoints))
			{
				this.PointsChanged(FuelPercentage);
			}
		}
	}
}
