using UnityEngine;

namespace ClubPenguin.Actions
{
	public class GetPointToTargetAction : Action
	{
		public Transform Destination;

		public float Offset;

		protected override void CopyTo(Action _destWarper)
		{
			GetPointToTargetAction getPointToTargetAction = _destWarper as GetPointToTargetAction;
			getPointToTargetAction.Destination = Destination;
			getPointToTargetAction.Offset = Offset;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			Vector3 vector = GetTarget().transform.position;
			if (Destination != null)
			{
				Vector3 normalized = (Destination.position - GetTarget().transform.position).normalized;
				vector = Destination.position - normalized * Offset;
			}
			Completed(vector);
		}
	}
}
