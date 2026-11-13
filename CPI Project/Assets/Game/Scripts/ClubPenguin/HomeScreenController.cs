using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public abstract class HomeScreenController : MonoBehaviour
	{
		public Button SettingsButton;

		public Button MembershipButton;

		protected GameStateController gameStateController;

		protected virtual void Awake()
		{
			gameStateController = Service.Get<GameStateController>();
		}

		protected virtual void OnEnable()
		{
			MembershipButton.onClick.AddListener(onMembershipClicked);
		}

		protected virtual void Start()
		{
			Service.Get<LoadingController>().AddLoadingSystem(this);
			StartCoroutine(updateContentManifest());
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (!isPaused)
			{
				StartCoroutine(updateContentManifest());
			}
		}

		protected virtual void OnDisable()
		{
			MembershipButton.onClick.RemoveListener(onMembershipClicked);
		}

		private IEnumerator updateContentManifest()
		{
			ContentSystemManager contentSystemManager = Service.Get<ContentSystemManager>();
			yield return contentSystemManager.UpdateContentSystem(false);
			startScreen();
		}

		private void startScreen()
		{
			if (Service.Get<ApplicationService>().RequiresUpdate)
			{
				ForcedUpgrade.OpenForcedUpgradePrompt();
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				return;
			}
			if (PlatformUtils.GetPlatformType() == PlatformType.Standalone && Service.Get<ApplicationService>().UpdateAvailable)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<UpgradeAvailablePromptData>();
				UpgradeAvailablePromptData component = cPDataEntityCollection.GetComponent<UpgradeAvailablePromptData>(entityByType);
				if (!component.HasSeenUpgradeAvailablePrompt)
				{
					component.HasSeenUpgradeAvailablePrompt = true;
					ForcedUpgrade.OpenOptionalUpgradePrompt();
				}
			}
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			if (!Service.Get<SessionManager>().HasSession)
			{
				firstTimeSetup();
			}
			else
			{
				setupScreen();
			}
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
		}

		protected virtual void firstTimeSetup()
		{
			setupScreen();
		}

		protected void setupScreen()
		{
			if (Service.Get<MixLoginCreateService>().RegistrationConfigIsNotSet)
			{
				Service.Get<MixLoginCreateService>().OnRegistrationConfigUpdated += checkAppGeogated;
			}
			else
			{
				checkAppGeogated(Service.Get<MixLoginCreateService>().RegistrationConfig);
			}
			if (Service.Get<SessionManager>().HasSession && !Service.Get<SessionManager>().IsLoggingOut)
			{
				showContinueFlow();
			}
			else
			{
				showPlayFlow();
			}
		}

		private void checkAppGeogated(IRegistrationConfiguration registrationConfig)
		{
			if (Service.Get<MixLoginCreateService>().IsEmbargoed)
			{
				disableAllButtons();
				Service.Get<ICPSwrveService>().Error("error_prompt", "GeoGate_EmbargoedCountry", null, SceneManager.GetActiveScene().name);
				Service.Get<PromptManager>().ShowError("GlobalUI.Homescreen.GeoGate.Title", "GlobalUI.Homescreen.GeoGate.Body", delegate
				{
					Application.Quit();
				});
			}
		}

		protected virtual void disableAllButtons()
		{
			MembershipButton.interactable = false;
			SettingsButton.interactable = false;
		}

		protected virtual void showPlayFlow()
		{
			MembershipButton.gameObject.SetActive(Service.Get<MembershipService>().IsPurchaseFunnelAvailable());
			SettingsButton.gameObject.SetActive(true);
		}

		protected virtual void showContinueFlow()
		{
			MembershipButton.gameObject.SetActive(Service.Get<MembershipService>().IsPurchaseFunnelAvailable() && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember());
			SettingsButton.gameObject.SetActive(true);
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			setupScreen();
			return false;
		}

		private void onMembershipClicked()
		{
			if (Service.Get<ApplicationService>().RequiresUpdate)
			{
				ForcedUpgrade.OpenForcedUpgradePrompt();
			}
			else
			{
				gameStateController.ShowAccountSystemMembership("home_screen");
			}
		}

		protected virtual void Destroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
		}
	}
}
