using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class CreateSuccessContentController : MonoBehaviour
	{
		public Button PlayButton;

		public event Action OnPlay;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "04", "create_success");
		}

		private void OnEnable()
		{
			PlayButton.interactable = true;
		}

		private void OnDisable()
		{
			PlayButton.interactable = false;
		}

		public void OnPlayButtonClicked()
		{
			if (this.OnPlay != null)
			{
				PlayButton.interactable = false;
				this.OnPlay();
			}
		}
	}
}
