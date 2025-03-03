using ClubPenguin.Analytics;
using ClubPenguin.Commerce;
using ClubPenguin.ContentGates;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;

namespace ClubPenguin.UI
{
	public class MembershipExpiringHandler : AbstractDataModelService
	{
		private static MembershipExpiringHandler instance;

		private EventChannel eventChannel;

		private ParentGate gate;

		private string subscriptionVendor;

		private void Start()
		{
			instance = this;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			gate = new ParentGate();
			gate.OnContinue += onGatePassed;
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
			eventChannel.RemoveAllListeners();
			if (gate != null)
			{
				gate.OnContinue -= onGatePassed;
			}
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			SubscriptionData component;
			dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component);
			if (component.SubscriptionPaymentPending)
			{
				subscriptionVendor = component.SubscriptionVendor;
				showMembershipExpiringPrompt();
				eventChannel.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			}
			return false;
		}

		private void showMembershipExpiringPrompt()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("MembershipExpiringPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onMembershipExpiringPromptLoaded);
			promptLoaderCMD.Execute();
		}

		private void onMembershipExpiringPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onMembershipExpiringPromptButtonClicked, promptLoader.Prefab);
			Service.Get<ICPSwrveService>().Action("game.google_account_hold_prompt", "view");
		}

		private void onMembershipExpiringPromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.OK)
			{
				gate.Show();
				Service.Get<ICPSwrveService>().Action("game.google_account_hold_prompt", "update", subscriptionVendor.ToLower());
			}
			else
			{
				Service.Get<ICPSwrveService>().Action("game.google_account_hold_prompt", "close");
			}
		}

		private void onGatePassed()
		{
			Service.Get<CommerceService>().TriggerManageAccount(subscriptionVendor.ToLower());
		}

		[Invokable("UI.Prompts.MembershipExpiringPrompt", Description = "Shows the Membership Expiring prompt")]
		private static void tweakerShowMembershipExpiringPrompt(string subscriptionVendor = "google")
		{
			if (instance != null)
			{
				instance.subscriptionVendor = subscriptionVendor;
				instance.showMembershipExpiringPrompt();
			}
			else
			{
				Log.LogError(null, "Membership Expiring Handler instance does not exist.");
			}
		}
	}
}
