using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisableIfNoMembershipVendor : MonoBehaviour
	{
		private void Awake()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			SubscriptionData component = cPDataEntityCollection.GetComponent<SubscriptionData>(cPDataEntityCollection.LocalPlayerHandle);
			if (component == null || component.SubscriptionVendor == null)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
