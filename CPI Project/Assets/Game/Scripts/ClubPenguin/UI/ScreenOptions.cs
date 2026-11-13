using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	public class ScreenOptions : MonoBehaviour
	{
		public void Logout()
		{
			Service.Get<SessionManager>().Logout();
			GameStateController gameStateController = Service.Get<GameStateController>();
			gameStateController.ResetStateMachine();
			if (SceneManager.GetActiveScene().name != gameStateController.SceneConfig.HomeSceneName)
			{
				Service.Get<SceneTransitionService>().LoadScene(gameStateController.SceneConfig.HomeSceneName, gameStateController.SceneConfig.TransitionSceneName);
			}
		}

		public void ShowTweakerConsole()
		{
		}

		public void OnHomeClicked()
		{
			Service.Get<GameStateController>().ExitWorld();
		}
	}
}
