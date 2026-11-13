using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.Input
{
	public class LocomotionDirectionalInput : LocomotionInput
	{
		[SerializeField]
		private KeyCodeInput left = null;

		[SerializeField]
		private KeyCodeInput right = null;

		[SerializeField]
		private KeyCodeInput up = null;

		[SerializeField]
		private KeyCodeInput down = null;

		private readonly ButtonInputResult buttonResult = new ButtonInputResult();

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			base.Initialize(keyCodeRemapper);
			left.Initialize(keyCodeRemapper);
			right.Initialize(keyCodeRemapper);
			up.Initialize(keyCodeRemapper);
			down.Initialize(keyCodeRemapper);
		}

		public override void StartFrame()
		{
			base.StartFrame();
			left.StartFrame();
			right.StartFrame();
			up.StartFrame();
			down.StartFrame();
		}

		public override void EndFrame()
		{
			base.EndFrame();
			left.EndFrame();
			right.EndFrame();
			up.EndFrame();
			down.EndFrame();
		}

		protected override bool process(int filter)
		{
			if (filter >= 0 && filter != 1)
			{
				return false;
			}

			Vector2 vector = Vector2.zero;
			if (ActiveInputDevice.CurrentKind == ActiveInputDevice.Kind.KeyboardMouse)
			{
				right.ProcessInput(buttonResult);
				float x = buttonResult.IsHeld ? 1f : 0f;

				left.ProcessInput(buttonResult);
				x -= (buttonResult.IsHeld ? 1f : 0f);

				up.ProcessInput(buttonResult);
				float y = buttonResult.IsHeld ? 1f : 0f;

				down.ProcessInput(buttonResult);
				y -= (buttonResult.IsHeld ? 1f : 0f);

				vector = new Vector2(x, y);
			}

			Gamepad gp = Gamepad.current;
			if (gp != null && ActiveInputDevice.CurrentKind == ActiveInputDevice.Kind.Gamepad)
			{
				Vector2 stick = gp.leftStick.ReadValue();
				if (stick.sqrMagnitude >= 0.04f)
				{
					if (stick.sqrMagnitude > vector.sqrMagnitude)
					{
						vector = stick;
					}
				}
			}

			inputEvent.Direction = vector;
			return inputEvent.Direction.sqrMagnitude > float.Epsilon;
		}
	}
}
