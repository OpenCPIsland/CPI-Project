using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class HeightOffGroundConstraint : Constraint
	{
		public float RaycastOffset = 1f;

		public float MinHeight = 1f;

		public int RaycastLayerMask = -1;

		public override void Apply(ref Setup setup)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(setup.Goal + Vector3.up * RaycastOffset, Vector3.down, out hitInfo, MinHeight + RaycastOffset, RaycastLayerMask))
			{
				setup.Goal.y = hitInfo.point.y + MinHeight;
			}
		}
	}
}
