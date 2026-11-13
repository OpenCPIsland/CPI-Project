#define UNITY_ASSERTIONS
using ClubPenguin.Breadcrumbs;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitGuiAction : InitActionComponent
	{
		public Canvas SystemCanvas;

		public GameObject HudLoadingIndicator;

		public PromptManager PromptManagerRef;

		private GameObject splashScreen;

		private GameObject loadingImage;

		private GameObject logoImage;

		private NotificationBreadcrumbController notificationBreadcrumbController;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return true;
			}
		}

		public void OnValidate()
		{
			Assert.IsTrue(SystemCanvas != null);
		}

		public override IEnumerator PerformFirstPass()
		{
			splashScreen = SystemCanvas.transform.Find("SplashScreen").gameObject;
			Service.Set(SystemCanvas);
			LoadingController instance = splashScreen.AddComponent<LoadingController>();
			Service.Set(instance);
			Service.Set(new ActionIndicatorController());
			Service.Set(new UIElementDisablerManager());
			notificationBreadcrumbController = new NotificationBreadcrumbController();
			Service.Set(notificationBreadcrumbController);
			TutorialBreadcrumbController tutorialBreadcrumbController = new TutorialBreadcrumbController();
			tutorialBreadcrumbController.Init();
			Service.Set(tutorialBreadcrumbController);
			GameObject gameObject = new GameObject();
			NativeSystemBarsManager instance2 = gameObject.AddComponent<NativeSystemBarsManager>();
			gameObject.transform.SetParent(Service.Get<GameObject>().transform);
			Service.Set(instance2);
			InAppRatingsPrompt instance3 = Service.Get<GameObject>().AddComponent<InAppRatingsPrompt>();
			Service.Set(instance3);
			PromptManagerRef.SetEventDispatcher(Service.Get<EventDispatcher>());
			yield break;
		}

		public override void OnInitializationComplete()
		{
			notificationBreadcrumbController.SetAvailableFeatureLabels(Service.Get<ContentSchedulerService>().ScheduledEventDate());
		}
	}
}
