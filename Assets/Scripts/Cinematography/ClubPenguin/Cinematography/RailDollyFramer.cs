using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class RailDollyFramer : Framer
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
			Dolly.Active = true;
			Dolly.Timer = 0f;
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

		public override void Aim(ref Setup setup)
		{
			setup.LookAt = Dolly.GetDollyPosition();
		}
	}
}
