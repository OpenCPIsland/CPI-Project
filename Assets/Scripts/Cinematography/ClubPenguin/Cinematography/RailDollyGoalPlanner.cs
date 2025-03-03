using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class RailDollyGoalPlanner : GoalPlanner
	{
		public RailDolly Dolly;

		private float dollyTimer = -1f;

		public override bool IsFinished
		{
			get
			{
				return Dolly.IsComplete;
			}
		}

		public void OnEnable()
		{
			Dolly.Timer = 0f;
			Dolly.Active = true;
		}

		public void Awake()
		{
			if (Dolly == null)
			{
				Dolly = GetComponent<RailDolly>();
			}
		}

		public void Update()
		{
			Dirty = !Mathf.Approximately(dollyTimer, Dolly.Timer);
			dollyTimer = Dolly.Timer;
		}

		public override void Plan(ref Setup setup)
		{
			setup.Goal = Dolly.GetDollyPosition();
		}
	}
}
