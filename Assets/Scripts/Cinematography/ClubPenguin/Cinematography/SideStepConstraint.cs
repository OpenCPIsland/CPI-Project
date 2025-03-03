using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class SideStepConstraint : Constraint
	{
		public float Distance = 1f;

		public float Angle = 10f;

		public float Speed = 10f;

		public bool XZ = true;

		public override void Apply(ref Setup setup)
		{
			Vector3 a = setup.Goal - setup.Focus.position;
			if (XZ)
			{
				a.y = 0f;
			}
			float magnitude = a.magnitude;
			if (magnitude < Distance)
			{
				float f = Mathf.DeltaAngle(setup.Focus.rotation.eulerAngles.y, setup.Camera.rotation.eulerAngles.y - 180f);
				if (Mathf.Abs(f) > Angle)
				{
					setup.Goal += setup.Camera.right * (0f - Mathf.Sign(f)) * Speed * Time.deltaTime;
					return;
				}
				float d = Distance / magnitude;
				setup.Goal = setup.Focus.position + a * d;
			}
		}
	}
}
