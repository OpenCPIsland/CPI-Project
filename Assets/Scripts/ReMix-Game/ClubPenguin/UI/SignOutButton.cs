using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class SignOutButton : MonoBehaviour
	{
		public Text buttonText;

		public GameObject preloader;

		private Button signOutButton;

		private void Start()
		{
			signOutButton = GetComponent<Button>();
			signOutButton.onClick.AddListener(onButtonClick);
		}

		private void onButtonClick()
		{
			signOutButton.interactable = false;
			if (buttonText != null)
			{
				buttonText.gameObject.SetActive(false);
			}
			if (preloader != null)
			{
				preloader.SetActive(true);
			}
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (sessionManager != null && sessionManager.HasSession)
			{
				Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEndedEvent);
				sessionManager.Logout();
			}
		}

		private bool onSessionEndedEvent(SessionEvents.SessionEndedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionEndedEvent>(onSessionEndedEvent);
			try
			{
				Service.Get<SceneTransitionService>().LoadScene("Home", "Loading");
			}
			finally
			{
				Service.Get<GameStateController>().ResetStateMachine();
			}
			return false;
		}

		private void OnDestroy()
		{
			GetComponent<Button>().onClick.RemoveListener(onButtonClick);
		}
	}
}
