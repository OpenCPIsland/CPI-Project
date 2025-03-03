using ClubPenguin.Analytics;
using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class MembershipExpiredContentController : MonoBehaviour
	{
		public Button ConfirmButton;

		public GameObject PreloaderImage;

		public Text ConfirmButtonText;

		private IContentInterruption gate;

		private MembershipController membershipController;

		private void OnEnable()
		{
			ConfirmButton.onClick.AddListener(onConfirmClicked);
		}

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "01", "membership_expired");
			membershipController = GetComponentInParent<MembershipController>();
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Accessibility.Popup.Title.MembershipExpired");
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
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
			gate = new ParentGate();
			gate.OnReturn += onGateFailed;
			gate.OnContinue += onGatePassed;
			gate.Show(base.transform);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "02", "agegate_triggered");
			Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.ExpiredConfirm");
		}

		private void onGateFailed()
		{
			ToggleInteraction(true);
			membershipController.MembershipBackToStart();
		}

		private void onGatePassed()
		{
			membershipController.MembershipOfferContinueClick();
		}
	}
}
