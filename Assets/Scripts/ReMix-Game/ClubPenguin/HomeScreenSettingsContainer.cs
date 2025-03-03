using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class HomeScreenSettingsContainer : MonoBehaviour
	{
		public Button BackButton;

		private GameObject settingsPrefabCloseButton;

		private Transform settingsLoader;

		private Transform subscreen;

		private void OnEnable()
		{
			settingsLoader = base.transform.Find("SettingsLoader");
			BackButton.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			subscreen = null;
			settingsPrefabCloseButton = null;
		}

		private void Update()
		{
			if (subscreen == null)
			{
				GameObject gameObject = GameObject.Find("SettingsSubScreen");
				if (gameObject != null)
				{
					subscreen = gameObject.transform;
				}
			}
			else
			{
				int num = subscreen.GetComponentsInChildren<StateMachine>(false).Length;
				if (num > 0)
				{
					BackButton.gameObject.SetActive(true);
				}
				else
				{
					BackButton.gameObject.SetActive(false);
				}
			}
			if (settingsPrefabCloseButton == null)
			{
				SettingsCloseButton componentInChildren = settingsLoader.GetComponentInChildren<SettingsCloseButton>();
				if (componentInChildren != null)
				{
					settingsPrefabCloseButton = componentInChildren.gameObject;
					settingsPrefabCloseButton.SetActive(false);
				}
			}
		}

		public void OnBackButtonPressed()
		{
			StateMachineContext componentInChildren = settingsLoader.GetComponentInChildren<StateMachineContext>();
			componentInChildren.SendEvent(new ExternalEvent("Settings", "back"));
		}
	}
}
