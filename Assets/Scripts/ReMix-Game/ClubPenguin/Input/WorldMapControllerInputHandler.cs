using ClubPenguin.WorldMap;
using UnityEngine;

namespace ClubPenguin.Input
{
	[RequireComponent(typeof(WorldMapController))]
	public class WorldMapControllerInputHandler : InputMapHandler<WorldMapControllerInputMap.Result>
	{
		private InputMappedButton btnClose;

		protected override void Awake()
		{
			btnClose = GetComponent<WorldMapController>().BtnClose.GetComponent<InputMappedButton>();
			base.Awake();
		}

		protected override void onHandle(WorldMapControllerInputMap.Result inputResult)
		{
			btnClose.HandleMappedInput(inputResult.Back);
		}

		protected override void onReset()
		{
			btnClose.HandleMappedInput();
		}
	}
}
