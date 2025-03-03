using UnityEngine;

namespace ClubPenguin.Actions
{
	public class GetDirectionAction : Action
	{
		public Transform Direction;

		public Transform To;

		protected override void CopyTo(Action _destWarper)
		{
			GetDirectionAction getDirectionAction = _destWarper as GetDirectionAction;
			getDirectionAction.To = To;
			getDirectionAction.Direction = Direction;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			Vector3 vector = GetTarget().transform.forward;
			if (Direction != null)
			{
				vector = Direction.forward;
			}
			else if (To != null)
			{
				vector = (To.position - GetTarget().transform.position).normalized;
			}
			Completed(vector);
		}
	}
}
