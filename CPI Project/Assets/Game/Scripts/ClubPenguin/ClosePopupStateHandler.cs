using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class ClosePopupStateHandler : AbstractAccountStateHandler
	{
		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
				componentInParent.OnClosePopup();
				GameStateController gameStateController = Service.Get<GameStateController>();
				string to_location = SceneManager.GetActiveScene().name;
				if (!Service.Get<SessionManager>().HasSession && SceneManager.GetActiveScene().name != gameStateController.SceneConfig.HomeSceneName && !Service.Get<MembershipService>().LoginViaMembership)
				{
					Service.Get<GameStateController>().ResetStateMachine();
					Service.Get<SceneTransitionService>().LoadScene(gameStateController.SceneConfig.HomeSceneName, gameStateController.SceneConfig.TransitionSceneName);
					to_location = gameStateController.SceneConfig.HomeSceneName;
				}
				else if (!Service.Get<SessionManager>().HasSession && SceneManager.GetActiveScene().name == gameStateController.SceneConfig.HomeSceneName)
				{
					Service.Get<GameStateController>().ResetStateMachine();
					to_location = gameStateController.SceneConfig.HomeSceneName;
				}
				Service.Get<ICPSwrveService>().NavigationAction("account_statemachine.close_button", SceneManager.GetActiveScene().name, to_location);
			}
		}
	}
}
