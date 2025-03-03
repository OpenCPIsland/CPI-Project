using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class RailRotationConstraint : Constraint
	{
		public enum LockedValue
		{
			Locked,
			Unlocked
		}

		public LockedValue LockXAxis = LockedValue.Unlocked;

		public LockedValue LockYAxis = LockedValue.Unlocked;

		public LockedValue LockZAxis = LockedValue.Unlocked;

		public Vector3 LockedAxisValues;

		public override void Apply(ref Setup setup)
		{
			if (LockXAxis == LockedValue.Locked || LockYAxis == LockedValue.Locked || LockZAxis == LockedValue.Locked)
			{
				setup.IsAxisLocked = true;
				setup.LockedAxis = new Vector3((float)LockXAxis, (float)LockYAxis, (float)LockZAxis);
				Vector3 lockedAxisValues = LockedAxisValues;
				lockedAxisValues.x *= (float)LockXAxis;
				lockedAxisValues.y *= (float)LockYAxis;
				lockedAxisValues.z *= (float)LockZAxis;
				setup.LockedAxisValues = lockedAxisValues;
			}
		}
	}
}
