using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class WelcomeContentController : MonoBehaviour
	{
		[Header("Penguin Details")]
		public Text PenguinName;

		private AvatarRenderTextureComponent avatarRenderer;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "06", "welcome");
			PenguinName.text = Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayName;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!(SceneManager.GetActiveScene().name == "Boot"))
			{
				avatarRenderer = GetComponentInChildren<AvatarRenderTextureComponent>();
				DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
				AvatarAnimationFrame avatarFrame = new AvatarAnimationFrame("Base Layer.Interactions.PassPortPoses_CelebrateAnimation", 0f);
				AvatarDetailsData component;
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
				{
					avatarRenderer.RenderAvatar(component, avatarFrame);
				}
				else
				{
					avatarRenderer.RenderAvatar(new DCustomEquipment[0], avatarFrame);
				}
			}
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.SkipWelcome = true;
			StartCoroutine(AutoTransition());
		}

		private IEnumerator AutoTransition()
		{
			yield return new WaitForSeconds(4.15f);
			StateMachineContext smc = GetComponentInParent<StateMachineContext>();
			smc.SendEvent(new ExternalEvent("AccountRootFSM", "closepopup"));
		}
	}
}
