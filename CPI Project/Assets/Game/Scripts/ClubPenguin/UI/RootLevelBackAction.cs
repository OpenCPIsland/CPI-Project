using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(BackButtonController))]
	public class RootLevelBackAction : MonoBehaviour, IRootLevelBackAction
	{
		private BackButtonController backButtonController;

		private bool showingPrompt = false;

		private void Awake()
		{
			backButtonController = GetComponent<BackButtonController>();
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<AppWindowUtilEvents.WindowCloseClickedEvent>(onWindowCloseClicked);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<AppWindowUtilEvents.WindowCloseClickedEvent>(onWindowCloseClicked);
		}

		private void OnEnable()
		{
			backButtonController.Add(onRootLevelBackButtonClicked);
			backButtonController.rootLevelBackAction = this;
		}

		private void OnDisable()
		{
			backButtonController.Remove(onRootLevelBackButtonClicked);
			backButtonController.rootLevelBackAction = null;
		}

		private void onRootLevelBackButtonClicked()
		{
			if (!Service.Get<LoadingController>().IsLoading && !Service.Get<LoadingController>().gameObject.activeSelf && !showingPrompt)
			{
				if (SceneManager.GetActiveScene().name == Service.Get<GameStateController>().SceneConfig.HomeSceneName)
				{
					showQuitAppPrompt();
				}
				else
				{
					showExitWorldPrompt();
				}
			}
		}

		private void showExitWorldPrompt()
		{
			showingPrompt = true;
			Service.Get<PromptManager>().ShowPrompt("ExitWorldPrompt", onShowExitWorldPromptCallback);
		}

		private void onShowExitWorldPromptCallback(DPrompt.ButtonFlags pressed)
		{
			showingPrompt = false;
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				Service.Get<GameStateController>().ExitWorld();
			}
		}

		private bool showQuitAppPrompt()
		{
			bool flag = false;
			if (!showingPrompt)
			{
				flag = Service.Get<PromptManager>().ShowPrompt("ExitAndLogoutGamePrompt", onShowQuitAppPromptCallback);
				if (flag)
				{
					showingPrompt = true;
				}
			}
			return flag;
		}

		private void onShowQuitAppPromptCallback(DPrompt.ButtonFlags pressed)
		{
			showingPrompt = false;
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				ShutdownHelper shutdownHelper = new ShutdownHelper(this);
				shutdownHelper.Shutdown();
			}
		}

		private bool onWindowCloseClicked(AppWindowUtilEvents.WindowCloseClickedEvent evt)
		{
			bool flag = true;
			try
			{
				flag = (!showingPrompt && !showQuitAppPrompt());
			}
			finally
			{
				if (flag)
				{
					Application.Quit();
				}
			}
			return true;
		}
	}
}
