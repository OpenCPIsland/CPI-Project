using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class SettingsCloseButton : MonoBehaviour
	{
		private SessionManager sessionManager;

		private GameStateController gameStateController;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(onButtonClick);
			sessionManager = Service.Get<SessionManager>();
			gameStateController = Service.Get<GameStateController>();
		}

		private void onButtonClick()
		{
			GetComponent<Button>().onClick.RemoveListener(onButtonClick);
			if (sessionManager.IsLoggingOut || !sessionManager.HasSession)
			{
				gameStateController.ReturnToHome();
			}
			else if (PlatformUtils.GetPlatformType() == PlatformType.Mobile)
			{
				loadPreviousScene();
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.ReturnToHomeScreen));
			}
		}

		private void loadPreviousScene()
		{
			string text = (string)Service.Get<SceneTransitionService>().GetSceneArg(SceneTransitionService.SceneArgs.ReturnTargetScene.ToString());
			bool flag = Service.Get<SceneTransitionService>().HasSceneArg(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString());
			Dictionary<string, object> dictionary = null;
			if (flag)
			{
				dictionary = new Dictionary<string, object>();
				dictionary.Add(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString(), true);
				Service.Get<LoadingController>().AddLoadingSystem(CellPhoneController.LoadingSystemObject);
			}
			if (text != null)
			{
				Service.Get<SceneTransitionService>().LoadScene(text, "Loading", dictionary);
			}
			else
			{
				gameStateController.ReturnToZoneScene(dictionary);
			}
		}
	}
}
