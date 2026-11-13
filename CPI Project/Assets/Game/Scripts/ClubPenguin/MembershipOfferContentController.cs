using ClubPenguin.Analytics;
using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class MembershipOfferContentController : MonoBehaviour
	{
		public Button ConfirmButton;

		public GameObject PreloaderImage;

		public Text ConfirmButtonText;

		public string LoggingString;

		public bool LogMembershipView;

		public string MembershipViewTrigger;

		private IContentInterruption gate;

		private MembershipController membershipController;

		private MembershipService membershipService;

		private void OnEnable()
		{
			ConfirmButton.onClick.AddListener(onConfirmClicked);
		}

		private void Start()
		{
			membershipService = Service.Get<MembershipService>();
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "01", LoggingString);
			membershipController = GetComponentInParent<MembershipController>();
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Accessibility.Popup.Title.MembershipOffer");
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
			}
			if (membershipService.LoginViaMembership)
			{
				membershipService.LoginViaMembership = false;
				onConfirmClicked();
			}
		}

		private void OnDisable()
		{
			ConfirmButton.onClick.RemoveListener(onConfirmClicked);
		}

		private void OnDestroy()
		{
			if (gate != null)
			{
				gate.OnReturn -= onGateFailed;
				gate.OnContinue -= onGatePassed;
			}
		}

		public void ToggleInteraction(bool isInteractable)
		{
			ConfirmButton.interactable = isInteractable;
			ConfirmButtonText.gameObject.SetActive(isInteractable);
			PreloaderImage.SetActive(!isInteractable);
		}

		private void onConfirmClicked()
		{
			ToggleInteraction(false);
			if (!Service.Get<SessionManager>().HasSession)
			{
				membershipService.LoginViaMembership = true;
				membershipController.MembershipLoginNeeded();
				Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.OfferConfirm", "login_needed");
				return;
			}
			if (LogMembershipView)
			{
				string currentMembershipStatus = Service.Get<MembershipService>().GetCurrentMembershipStatus();
				Service.Get<ICPSwrveService>().Action("game.free_trial", MembershipViewTrigger, currentMembershipStatus, SceneManager.GetActiveScene().name);
			}
			gate = new ParentGate();
			gate.OnReturn += onGateFailed;
			gate.OnContinue += onGatePassed;
			gate.Show(base.transform);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "02", "agegate_triggered");
			Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.OfferConfirm", "trigger_agegate");
			base.gameObject.SetActive(false);
		}

		private void onGateFailed()
		{
			gate.OnReturn -= onGateFailed;
			gate.OnContinue -= onGatePassed;
			if (!base.gameObject.IsDestroyed())
			{
				base.gameObject.SetActive(true);
				ToggleInteraction(true);
				membershipController.MembershipBackToStart();
			}
		}

		private void onGatePassed()
		{
			gate.OnReturn -= onGateFailed;
			gate.OnContinue -= onGatePassed;
			membershipController.MembershipOfferContinueClick();
		}
	}
}
