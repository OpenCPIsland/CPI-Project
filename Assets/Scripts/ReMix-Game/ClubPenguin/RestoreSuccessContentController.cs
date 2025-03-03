using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class RestoreSuccessContentController : MonoBehaviour
	{
		[Header("Penguin Details")]
		public Text PenguinName;

		private AvatarRenderTextureComponent avatarRenderer;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "16", "restore_membership_success");
			ModalBackground component = GetComponent<ModalBackground>();
			component.enabled = true;
			PenguinName.text = string.Format(PenguinName.text, Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayName);
			if (!(SceneManager.GetActiveScene().name == "Boot"))
			{
				avatarRenderer = GetComponentInChildren<AvatarRenderTextureComponent>();
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
				AvatarAnimationFrame avatarFrame = new AvatarAnimationFrame("Base Layer.Interactions.PassPortPoses_CelebrateAnimation", 0f);
				AvatarDetailsData component2;
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component2))
				{
					avatarRenderer.RenderAvatar(component2, avatarFrame);
				}
				else
				{
					avatarRenderer.RenderAvatar(new DCustomEquipment[0], avatarFrame);
				}
			}
		}

		public void OnClosePopup()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
