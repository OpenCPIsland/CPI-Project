using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class StartGameStateHandler : AbstractAccountStateHandler
	{
		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				Service.Get<GameStateController>().StartGame();
				AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
				componentInParent.OnClosePopup();
			}
		}
	}
}
