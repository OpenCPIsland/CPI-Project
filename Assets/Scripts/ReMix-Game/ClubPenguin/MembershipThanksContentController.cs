using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class MembershipThanksContentController : MonoBehaviour
	{
		[Header("Buttons")]
		public Button ConfirmButton;

		private MembershipController membershipController;

		private AvatarRenderTextureComponent avatarRenderer;

		private void OnEnable()
		{
			ConfirmButton.onClick.AddListener(onConfirmClicked);
		}

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().MembershipFunnelName, "05", "membership_thanks");
			membershipController = GetComponentInParent<MembershipController>();
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Accessibility.Popup.Title.MembershipThanks");
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!(SceneManager.GetActiveScene().name == "Boot"))
			{
				avatarRenderer = GetComponentInChildren<AvatarRenderTextureComponent>();
				AvatarDetailsData component;
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
				{
					avatarRenderer.RenderAvatar(component);
				}
				else
				{
					avatarRenderer.RenderAvatar(new DCustomEquipment[0]);
				}
			}
		}

		private void onConfirmClicked()
		{
			membershipController.MembershipThanksContinueClick();
			Service.Get<ICPSwrveService>().NavigationAction("membership_buttons.ThanksContinue");
		}
	}
}
