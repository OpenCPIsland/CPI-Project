using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class HomeScreenLandscapeController : HomeScreenController
	{
		public Button ExitGameButton;

		public GameObject SettingsPrefab;

		private GameObject settingsPanel;

		private Transform popupCanvas;

		private int activeAccountSystemsCount = 0;

		private BundlePrecacheManager bundlePrecacheManager;

		protected override void Awake()
		{
			base.Awake();
			bundlePrecacheManager = Service.Get<BundlePrecacheManager>();
		}

		protected override void Start()
		{
			base.Start();
			Service.Get<EventDispatcher>().AddListener<AccountSystemEvents.AccountSystemCreated>(onAccountSystemCreated);
			Service.Get<EventDispatcher>().AddListener<AccountSystemEvents.AccountSystemDestroyed>(onAccountSystemDestroyed);
			gameStateController.ShowAccountSystemLogin();
			popupCanvas = GameObject.Find("PopupCanvas").transform;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ExitGameButton.onClick.AddListener(onExitClicked);
			SettingsButton.onClick.AddListener(onSettingsButtonClicked);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			ExitGameButton.onClick.RemoveListener(onExitClicked);
			SettingsButton.onClick.RemoveListener(onSettingsButtonClicked);
		}

		protected override void firstTimeSetup()
		{
			base.firstTimeSetup();
			StartCoroutine(precacheBundles());
		}

		private IEnumerator precacheBundles()
		{
			Service.Get<LoadingController>().AddLoadingSystem(this);
			bundlePrecacheManager.StartCaching(precacheComplete);
			yield return new WaitForSeconds(bundlePrecacheManager.Config.BundlePrecacheSeconds);
			precacheComplete();
		}

		private void precacheComplete()
		{
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
			bundlePrecacheManager.MoveToBackground();
		}

		private void onExitClicked()
		{
			Service.Get<PromptManager>().ShowPrompt("ExitAndLogoutGamePrompt", onExitConfirmationClicked);
		}

		private void onExitConfirmationClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				ShutdownHelper shutdownHelper = new ShutdownHelper(this);
				shutdownHelper.Shutdown();
			}
		}

		private void onSettingsButtonClicked()
		{
			if (settingsPanel == null)
			{
				settingsPanel = Object.Instantiate(SettingsPrefab, popupCanvas, false);
				settingsPanel.SetActive(true);
			}
			else
			{
				settingsPanel.SetActive(!settingsPanel.activeSelf);
			}
		}

		private bool onAccountSystemCreated(AccountSystemEvents.AccountSystemCreated evt)
		{
			activeAccountSystemsCount++;
			return false;
		}

		private bool onAccountSystemDestroyed(AccountSystemEvents.AccountSystemDestroyed evt)
		{
			activeAccountSystemsCount--;
			if (activeAccountSystemsCount <= 0 && Service.Get<GameStateController>().CurrentState() == "Default")
			{
				gameStateController.ShowAccountSystemLogin();
			}
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<AccountSystemEvents.AccountSystemCreated>(onAccountSystemCreated);
			Service.Get<EventDispatcher>().RemoveListener<AccountSystemEvents.AccountSystemDestroyed>(onAccountSystemDestroyed);
		}
	}
}
