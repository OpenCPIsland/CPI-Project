using UnityEngine;

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
			right.ProcessInput(buttonResult);
			float num = buttonResult.IsHeld ? 1f : 0f;
			left.ProcessInput(buttonResult);
			num -= (buttonResult.IsHeld ? 1f : 0f);
			up.ProcessInput(buttonResult);
			float num2 = buttonResult.IsHeld ? 1f : 0f;
			down.ProcessInput(buttonResult);
			num2 -= (buttonResult.IsHeld ? 1f : 0f);
			inputEvent.Direction = new Vector2(num, num2);
			return inputEvent.Direction.sqrMagnitude > float.Epsilon;
		}
	}
}
