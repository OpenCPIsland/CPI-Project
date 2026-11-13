using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class LogoutStateHandler : AbstractAccountStateHandler
	{
		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				Service.Get<SessionManager>().Logout();
				Service.Get<GameStateController>().ResetStateMachine();
				Service.Get<EventDispatcher>().DispatchEvent(default(AccountSystemEvents.AccountSystemEnded));
				AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
				componentInParent.OnClosePopup();
				string to_location = SceneManager.GetActiveScene().name;
				GameStateController gameStateController = Service.Get<GameStateController>();
				if (SceneManager.GetActiveScene().name != gameStateController.SceneConfig.HomeSceneName)
				{
					Service.Get<SceneTransitionService>().LoadScene(gameStateController.SceneConfig.HomeSceneName, gameStateController.SceneConfig.TransitionSceneName);
					to_location = gameStateController.SceneConfig.HomeSceneName;
				}
				Service.Get<ICPSwrveService>().NavigationAction("account_statemachine.gameStateController.close_button_logout", SceneManager.GetActiveScene().name, to_location);
			}
		}
	}
}
