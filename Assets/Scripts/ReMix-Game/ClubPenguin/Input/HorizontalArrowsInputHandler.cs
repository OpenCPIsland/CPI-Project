using UnityEngine;

namespace ClubPenguin.Input
{
	public class HorizontalArrowsInputHandler : InputMapHandler<HorizontalArrowsInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton left = null;

		[SerializeField]
		private InputMappedButton right = null;

		private new void OnValidate()
		{
		}

		protected override void onHandle(HorizontalArrowsInputMap.Result inputResult)
		{
			left.HandleMappedInput(inputResult.Left);
			right.HandleMappedInput(inputResult.Right);
		}

		protected override void onReset()
		{
			left.HandleMappedInput();
			right.HandleMappedInput();
		}
	}
}
