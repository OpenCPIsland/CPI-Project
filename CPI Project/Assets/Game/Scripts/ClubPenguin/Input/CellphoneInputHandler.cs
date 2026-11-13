using UnityEngine;

namespace ClubPenguin.Input
{
	public class CellphoneInputHandler : InputMapHandler<CellphoneInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton btnClose;

		protected override void onHandle(CellphoneInputMap.Result inputResult)
		{
			btnClose.HandleMappedInput(inputResult.Back);
		}

		protected override void onReset()
		{
			btnClose.HandleMappedInput();
		}
	}
}
