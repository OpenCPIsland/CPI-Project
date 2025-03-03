using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Odometer : MonoBehaviour
	{
		public delegate void OnDistanceChanged(int _newDistance);

		private float m_distanceTravelledThisRun = 0f;

		private mg_jr_ScrollingSpeed m_speedData;

		private mg_jr_GameData m_balance = null;

		private mg_jr_GoalManager m_goalManager = null;

		private float m_distanceTravelledSession = 0f;

		public float DistanceTravelledThisRunAfterPenalties
		{
			get;
			private set;
		}

		public float DistanceTravelledInEnvironment
		{
			get;
			private set;
		}

		public float DistanceTravelledThisRun
		{
			get
			{
				return m_distanceTravelledThisRun;
			}
			set
			{
				m_distanceTravelledThisRun = value;
				if (this.DistanceChanged != null)
				{
					this.DistanceChanged((int)m_distanceTravelledThisRun);
				}
			}
		}

		public event OnDistanceChanged DistanceChanged;

		public void ApplyDistancePenalty()
		{
			float num = Mathf.Clamp01(m_balance.DistancePenaltyPercentage);
			DistanceTravelledThisRunAfterPenalties *= num;
		}

		private void Start()
		{
			m_speedData = GetComponent<mg_jr_ScrollingSpeed>();
			Assert.NotNull(m_speedData, "Odometer requires an mg_jr_ScrollingSpeed component on the same object");
			Assert.NotNull(m_balance, "Balance can't be null, did you call Init?");
			Assert.NotNull(m_goalManager, "Goal manager can't be null, did you call Init?");
			Reset();
		}

		public void Init(mg_jr_GameData _balance, mg_jr_GoalManager _goalManager)
		{
			m_balance = _balance;
			m_goalManager = _goalManager;
		}

		public void Reset()
		{
			m_distanceTravelledThisRun = 0f;
			DistanceTravelledThisRunAfterPenalties = 0f;
			DistanceTravelledInEnvironment = 0f;
		}

		public void EnvironmentChanged()
		{
			DistanceTravelledInEnvironment = 0f;
		}

		public virtual void MinigameUpdate(float _deltaTime)
		{
			if (m_speedData.ScrollingEnabled)
			{
				float num = m_speedData.CurrentSpeedFor(mg_jr_SpriteDrawingLayers.DrawingLayers.BOTTOM_BORDER);
				float units = num * _deltaTime;
				float num2 = m_balance.UnitsToMeters(units);
				DistanceTravelledThisRun += num2;
				DistanceTravelledThisRunAfterPenalties += num2;
				m_distanceTravelledSession += num2;
				DistanceTravelledInEnvironment += num2;
				m_goalManager.AddToProgress(mg_jr_Goal.GoalType.DISTANCE_TRAVELLED, num2);
			}
		}
	}
}
