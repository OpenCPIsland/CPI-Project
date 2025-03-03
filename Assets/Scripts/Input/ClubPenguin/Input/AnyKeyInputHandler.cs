using UnityEngine;

namespace ClubPenguin.Input
{
	public class AnyKeyInputHandler : InputMapHandler<AnyKeyInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton button = null;

		protected override void OnValidate()
		{
			base.OnValidate();
		}

		protected override void onHandle(AnyKeyInputMap.Result inputResult)
		{
			button.HandleMappedInput(inputResult.AnyKey);
		}

		protected override void onReset()
		{
			button.HandleMappedInput();
		}
	}
}
