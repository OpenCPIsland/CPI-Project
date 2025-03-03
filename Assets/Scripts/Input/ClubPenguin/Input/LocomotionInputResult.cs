using UnityEngine;

namespace ClubPenguin.Input
{
	public class LocomotionInputResult : InputResult<LocomotionInputResult>
	{
		public string LogType;

		public Vector2 Direction;

		public Vector2 Rotation;

		public override void CopyTo(LocomotionInputResult copyToInputResult)
		{
			copyToInputResult.LogType = LogType;
			copyToInputResult.Direction = Direction;
			copyToInputResult.Rotation = Rotation;
		}

		public override void Reset()
		{
			Direction = Vector2.zero;
			Rotation = Vector2.zero;
		}
	}
}
