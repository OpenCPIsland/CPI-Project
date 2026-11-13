using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class LoginBackButtonListener : MonoBehaviour
	{
		public string LoginStateName;

		public string MembershipOfferStateName;

		private BackButtonStateHandler backButtonStateHandler;

		private void Start()
		{
			backButtonStateHandler = GetComponentInParent<BackButtonStateHandler>();
			if (backButtonStateHandler != null)
			{
				backButtonStateHandler.OnBackStateTransition += onBackStateTransition;
			}
			else
			{
				Log.LogError(this, "Couldn't find BackButtonStateHandler in parent");
			}
		}

		private void onBackStateTransition(string currentStateName, string nextStateName)
		{
			if (currentStateName == LoginStateName && nextStateName == MembershipOfferStateName)
			{
				Service.Get<MembershipService>().LoginViaMembership = false;
			}
		}

		private void OnDestroy()
		{
			if (backButtonStateHandler != null)
			{
				backButtonStateHandler.OnBackStateTransition -= onBackStateTransition;
			}
		}
	}
}
