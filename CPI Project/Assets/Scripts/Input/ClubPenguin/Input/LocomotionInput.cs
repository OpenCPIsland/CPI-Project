using UnityEngine;

namespace ClubPenguin.Input
{
	public abstract class LocomotionInput : Input<LocomotionInputResult>
	{
		public enum FilterType
		{
			Mouse,
			Keyboard
		}

		[SerializeField]
		private string logType = string.Empty;

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			base.Initialize(keyCodeRemapper);
			inputEvent.LogType = logType;
		}
	}
}
