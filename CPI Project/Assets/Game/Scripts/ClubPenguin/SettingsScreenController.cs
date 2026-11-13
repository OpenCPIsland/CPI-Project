using ClubPenguin.Net;
using Disney.MobileNetwork;
using Tweaker.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class SettingsScreenController : MonoBehaviour
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
			Service.Get<TweakerConsoleController>().gameObject.SetActive(true);
		}

		public void OnHomeClicked()
		{
			Service.Get<SessionManager>().PauseSession();
		}
	}
}
