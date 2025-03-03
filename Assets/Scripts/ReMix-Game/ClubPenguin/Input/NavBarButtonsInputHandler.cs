using UnityEngine;

namespace ClubPenguin.Input
{
	public class NavBarButtonsInputHandler : InputMapHandler<NavBarButtonsInputMap.Result>
	{
		[SerializeField]
		private InputMappedButton btnProfile = null;

		[SerializeField]
		private InputMappedButton btnConsumables = null;

		[SerializeField]
		private InputMappedButton btnQuest = null;

		[SerializeField]
		private InputMappedButton btnMap = null;

		private new void OnValidate()
		{
		}

		protected override void onHandle(NavBarButtonsInputMap.Result inputResult)
		{
			btnProfile.HandleMappedInput(inputResult.Profile);
			btnConsumables.HandleMappedInput(inputResult.Consumables);
			btnQuest.HandleMappedInput(inputResult.Quest);
			btnMap.HandleMappedInput(inputResult.Map);
		}

		protected override void onReset()
		{
			btnProfile.HandleMappedInput();
			btnConsumables.HandleMappedInput();
			btnQuest.HandleMappedInput();
			btnMap.HandleMappedInput();
		}
	}
}
