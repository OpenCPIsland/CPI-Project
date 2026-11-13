using ClubPenguin.Analytics;
using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class MembershipRequiredContentController : MonoBehaviour
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
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "06", "membership_required");
			membershipController = GetComponentInParent<MembershipController>();
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
			gate = new AgeGate();
			gate.OnReturn += onGateFailed;
			gate.OnContinue += onGatePassed;
			gate.Show(base.transform);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "02", "agegate_triggered");
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
