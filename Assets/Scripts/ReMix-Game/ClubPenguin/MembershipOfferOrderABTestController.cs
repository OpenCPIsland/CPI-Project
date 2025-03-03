using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class MembershipOfferOrderABTestController : MonoBehaviour
	{
		private const string PLAYER_PREFS_KEY = "MembershipOfferOrderABTestCurrentScreen";

		private const string MEMBERSHIP_OFFER_RESOURCE = "MembershipOffer";

		public PrefabContentKey[] ContentKeys;

		private void Awake()
		{
			if (ContentKeys == null || ContentKeys.Length == 0)
			{
				throw new Exception("No available ContentKeys in the MembershipOfferOrderABTestController");
			}
			int num = 0;
			PrefabContentKey key;
			if (num < ContentKeys.Length)
			{
				key = ContentKeys[num];
			}
			else
			{
				int num2 = 0;
				if (PlayerPrefs.HasKey("MembershipOfferOrderABTestCurrentScreen"))
				{
					num2 = PlayerPrefs.GetInt("MembershipOfferOrderABTestCurrentScreen");
					if (num2 >= ContentKeys.Length)
					{
						num2 = 0;
					}
				}
				key = ContentKeys[num2];
				int value = (num2 + 1) % ContentKeys.Length;
				PlayerPrefs.SetInt("MembershipOfferOrderABTestCurrentScreen", value);
			}
			Content.LoadAsync(onPrefabLoaded, key);
		}

		private void onPrefabLoaded(string path, GameObject membershipOfferPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(membershipOfferPrefab);
			gameObject.transform.SetParent(base.transform, false);
		}
	}
}
