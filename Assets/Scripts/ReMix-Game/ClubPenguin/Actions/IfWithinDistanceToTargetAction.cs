using UnityEngine;

namespace ClubPenguin.Actions
{
	public class IfWithinDistanceToTargetAction : Action
	{
		public Transform TargetObject;

		public float Distance;

		public bool FacingTargetObject = true;

		public float FacingTargetThreshold = 0.5f;

		protected override void CopyTo(Action _destWarper)
		{
			IfWithinDistanceToTargetAction ifWithinDistanceToTargetAction = _destWarper as IfWithinDistanceToTargetAction;
			ifWithinDistanceToTargetAction.TargetObject = TargetObject;
			ifWithinDistanceToTargetAction.Distance = Distance;
			ifWithinDistanceToTargetAction.FacingTargetObject = FacingTargetObject;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			if (TargetObject != null)
			{
				if (Mathf.Abs((TargetObject.position - GetTarget().transform.position).magnitude) <= Distance)
				{
					if (FacingTargetObject)
					{
						Vector3 normalized = (TargetObject.position - GetTarget().transform.position).normalized;
						if (Vector3.Dot(normalized, GetTarget().transform.forward) >= FacingTargetThreshold)
						{
							Completed();
						}
						else
						{
							Completed(null, false);
						}
					}
					else
					{
						Completed();
					}
				}
				else
				{
					Completed(null, false);
				}
			}
			else
			{
				Completed(null, false);
			}
		}
	}
}
