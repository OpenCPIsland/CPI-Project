using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class RestoreSubscriptionsController : MonoBehaviour
	{
		private MembershipService membershipService;

		public PrefabContentKey MembershipPromptPrefabContentKey;

		public RestoreSuccessContentController SuccessPrefab;

		public Button RestoreButton;

		public Text RestoreButtonText;

		public GameObject RestoreButtonPreloader;

		public string MembershipEvent;

		public string NoMembershipEvent;

		public string AllAccessEventMembershipEvent;

		private void OnEnable()
		{
			RestoreButton.gameObject.SetActive(false);
		}

		public void Start()
		{
			membershipService = Service.Get<MembershipService>();
		}

		public void OnLoginClicked()
		{
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.LoginViaRestore = true;
			Service.Get<EventDispatcher>().AddListener<SessionEvents.ReturnToRestorePurchases>(checkMembership);
			LoginController.SkipAutoLogin = true;
			Service.Get<MixLoginCreateService>().LogoutLastSession();
			Service.Get<GameStateController>().ShowAccountSystemLogin();
		}

		public void OnRestoreClicked()
		{
			if (!Service.Get<SessionManager>().HasSession)
			{
				DPrompt data = new DPrompt("Membership.Purchase.RestoreMessageTitle", "Membership.Purchase.Restore.LoginRequired");
				Content.LoadAsync(delegate(string path, GameObject prefab)
				{
					onMembershipPromptLoaded(data, prefab);
				}, MembershipPromptPrefabContentKey);
			}
			else
			{
				string format = "Membership.Purchase.Restore.NothingToRestore";
				ApplicationService.Error error = new ApplicationService.Error("Membership.Purchase.RestoreMessageTitle", format);
				onRestoreFailed(error);
			}
		}

		private void onMembershipPromptLoaded(DPrompt data, GameObject membershipPromptPrefab)
		{
			PromptController component = membershipPromptPrefab.GetComponent<PromptController>();
			Service.Get<PromptManager>().ShowPrompt(data, null, component);
		}

		private bool checkMembership(SessionEvents.ReturnToRestorePurchases evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.ReturnToRestorePurchases>(checkMembership);
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				switch (component.MembershipType)
				{
				case MembershipType.Member:
					componentInParent.SendEvent(new ExternalEvent("SettingsMain", MembershipEvent));
					return false;
				case MembershipType.AllAccessEventMember:
					componentInParent.SendEvent(new ExternalEvent("SettingsMain", AllAccessEventMembershipEvent));
					return false;
				}
			}
			componentInParent.SendEvent(new ExternalEvent("SettingsMain", NoMembershipEvent));
			return false;
		}

		private void showGate()
		{
			IContentInterruption contentInterruption = new ParentGate();
			contentInterruption.OnReturn += onGateFailed;
			contentInterruption.OnContinue += onGatePassed;
			contentInterruption.Show(base.transform);
		}

		private void onGateFailed()
		{
			toggleRestoreButton(true);
		}

		private void onGatePassed()
		{
			membershipService.OnRestoreSuccess += onRestoreSuccess;
			membershipService.OnRestoreFailed += onRestoreFailed;
			membershipService.TriggerRestorePlayerPurchases();
		}

		private void onRestoreSuccess()
		{
			toggleRestoreButton(true);
			membershipService.OnRestoreSuccess -= onRestoreSuccess;
			membershipService.OnRestoreFailed -= onRestoreFailed;
			GameObject popup = Object.Instantiate(SuccessPrefab.gameObject);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup, true, true, "Accessibility.Popup.Title.RestoreSubscriptionSuccess"));
		}

		private void onRestoreFailed(ApplicationService.Error error)
		{
			toggleRestoreButton(true);
			membershipService.OnRestoreSuccess -= onRestoreSuccess;
			membershipService.OnRestoreFailed -= onRestoreFailed;
			DPrompt data = new DPrompt(error.Type, error.Message);
			Content.LoadAsync(delegate(string path, GameObject prefab)
			{
				onMembershipPromptLoaded(data, prefab);
			}, MembershipPromptPrefabContentKey);
		}

		private void toggleRestoreButton(bool value)
		{
			RestoreButton.interactable = value;
			RestoreButtonText.enabled = value;
			RestoreButtonPreloader.SetActive(!value);
		}

		private void OnDisable()
		{
		}
	}
}
