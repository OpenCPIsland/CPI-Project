using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardPopupScreen : MonoBehaviour
	{
		private const string MEMBER_NOTIFICATION_ITEMS = "memberdisclaimer.text";

		private const string MEMBER_NOTIFICATION_GEAR = "ExchangeEmpty.EmptyMessage.HeaderText.Gear";

		private const string MEMBER_NOTIFICATION_PARTYSUPPLIES = "GlobalUI.PartySupplies.Empty";

		private const string MEMBER_NOTIFICATION_TUBES = "MemberNotification.RewardScreen.Tubes";

		private const string MEMBER_NOTIFICATION_DECORATION_PURCHASE_RIGHTS = "MemberNotification.RewardScreen.Igloos.Items";

		private const string MEMBER_NOTIFICATION_DECORATION_LOCATIONS = "MemberNotification.RewardScreen.Igloos.Locations";

		private const string MEMBER_NOTIFICATION_DECORATION_BUILDINGS = "MemberNotification.RewardScreen.Igloos.Buildings";

		private const string MEMBER_NOTIFICATION_DECORATION_MUSIC = "MemberNotification.RewardScreen.Igloos.Music";

		private const string MEMBER_NOTIFICATION_DECORATION_LIGHTING = "MemberNotification.RewardScreen.Igloos.Lighting";

		private const string MEMBER_NOTIFICATION_DECORATION_SLOTS = "MemberNotification.RewardScreen.Igloos.Slots";

		private const string MEMBER_NOTIFICATION_DEFAULT = "Membership.LockedReward";

		private const string ANIMATOR_TRIGGER_EXIT = "Exit";

		public Animator ScreenAnimator;

		private bool isClosing = false;

		protected string membershipNotificationText = "";

		public event Action ScreenCompleteAction;

		private void Awake()
		{
			ItemImageBuilder.acquire();
		}

		public virtual void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
		}

		public virtual void OnClick()
		{
		}

		private void playExitAnimation()
		{
			if (!isClosing)
			{
				ScreenAnimator.SetTrigger("Exit");
				isClosing = true;
			}
		}

		protected void screenComplete()
		{
			playExitAnimation();
		}

		public void OnExitAnimationComplete()
		{
			if (this.ScreenCompleteAction != null)
			{
				this.ScreenCompleteAction();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		protected Vector3 getUITweenDestination(GameObject worldObject, float zPosition)
		{
			Vector3 position = CameraHelper.GetCameraByTag("UICamera").transform.position;
			Ray ray = new Ray(position, worldObject.transform.position - position);
			Plane plane = new Plane(Vector3.back, zPosition);
			float enter = 0f;
			plane.Raycast(ray, out enter);
			return ray.GetPoint(enter);
		}

		protected virtual void OnDestroy()
		{
			this.ScreenCompleteAction = null;
			ItemImageBuilder.release();
		}

		protected virtual void checkMembershipDisclaimer()
		{
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				showMembershipDisclaimer(membershipNotificationText);
			}
		}

		protected void showMembershipDisclaimer(string notificationText)
		{
			DNotification dNotification = new DNotification();
			dNotification.PrefabLocation = new PrefabContentKey(TrayNotificationManager.MemberNotificationContentKey);
			dNotification.Message = notificationText;
			dNotification.ContainsButtons = false;
			dNotification.AutoClose = false;
			dNotification.PersistBetweenScenes = false;
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
		}

		protected string getMembershipDisclaimerTokenForRewardCategory(RewardCategory category)
		{
			switch (category)
			{
			case RewardCategory.equipmentTemplates:
			case RewardCategory.fabrics:
			case RewardCategory.decals:
				return "memberdisclaimer.text";
			case RewardCategory.partySupplies:
				return "GlobalUI.PartySupplies.Empty";
			case RewardCategory.durables:
				return "ExchangeEmpty.EmptyMessage.HeaderText.Gear";
			case RewardCategory.tubes:
				return "MemberNotification.RewardScreen.Tubes";
			case RewardCategory.decorationPurchaseRights:
				return "MemberNotification.RewardScreen.Igloos.Items";
			case RewardCategory.lots:
				return "MemberNotification.RewardScreen.Igloos.Locations";
			case RewardCategory.structurePurchaseRights:
				return "MemberNotification.RewardScreen.Igloos.Buildings";
			case RewardCategory.musicTracks:
				return "MemberNotification.RewardScreen.Igloos.Music";
			case RewardCategory.lighting:
				return "MemberNotification.RewardScreen.Igloos.Lighting";
			case RewardCategory.iglooSlots:
				return "MemberNotification.RewardScreen.Igloos.Slots";
			default:
				return "Membership.LockedReward";
			}
		}
	}
}
