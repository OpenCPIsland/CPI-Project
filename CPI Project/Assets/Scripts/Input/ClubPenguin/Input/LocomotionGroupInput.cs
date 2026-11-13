using UnityEngine;

namespace ClubPenguin.Input
{
	public class LocomotionGroupInput : LocomotionInput
	{
		[SerializeField]
		private LocomotionInput[] locomotionInputs = new LocomotionInput[0];

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			base.Initialize(keyCodeRemapper);
			LocomotionInput[] array = locomotionInputs;
			foreach (LocomotionInput locomotionInput in array)
			{
				locomotionInput.Initialize(keyCodeRemapper);
			}
		}

		public override void StartFrame()
		{
			base.StartFrame();
			LocomotionInput[] array = locomotionInputs;
			foreach (LocomotionInput locomotionInput in array)
			{
				locomotionInput.StartFrame();
			}
		}

		public override void EndFrame()
		{
			base.EndFrame();
			LocomotionInput[] array = locomotionInputs;
			foreach (LocomotionInput locomotionInput in array)
			{
				locomotionInput.EndFrame();
			}
		}

		protected override bool process(int filter)
		{
			bool flag = false;
			LocomotionInput[] array = locomotionInputs;
			foreach (LocomotionInput locomotionInput in array)
			{
				if (locomotionInput.ProcessInput(inputEvent, filter))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				resetInput();
			}
			return flag;
		}
	}
}
