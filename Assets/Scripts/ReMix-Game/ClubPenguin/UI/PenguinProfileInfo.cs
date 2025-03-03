using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PenguinProfileInfo : MonoBehaviour
	{
		private const string MEMBER_TOKEN = "MyProfile.MemberStatus.MemberText";

		private const string NON_MEMBER_TOKEN = "MyProfile.MemberStatus.NonMemberText";

		public Text AgeText;

		public Text CoinsText;

		public Text MembershipStatusText;

		public SpriteSelector MembershipSpriteSelector;

		private CoinsData coinsData;

		public void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out coinsData))
			{
				setCoins(coinsData.Coins);
				coinsData.OnCoinsChanged += setCoins;
			}
			else
			{
				Log.LogError(this, "Could not find CoinsData on local player handle");
			}
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				AgeText.text = component.PenguinAgeInDays.ToString();
			}
			else
			{
				Log.LogError(this, "Could not find ProfileData on local player handle");
			}
			MembershipData component2;
			if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component2))
			{
				int index = 0;
				switch (component2.MembershipType)
				{
				case MembershipType.Member:
					index = 1;
					break;
				case MembershipType.AllAccessEventMember:
					index = 2;
					break;
				}
				MembershipSpriteSelector.SelectSprite(index);
				string token = component2.IsMember ? "MyProfile.MemberStatus.MemberText" : "MyProfile.MemberStatus.NonMemberText";
				MembershipStatusText.text = Service.Get<Localizer>().GetTokenTranslation(token);
			}
			else
			{
				Log.LogError(this, "Could not find MembershipData on local player handle");
			}
		}

		private void setCoins(int coins)
		{
			CoinsText.text = coins.ToString();
		}

		private void OnDestroy()
		{
			coinsData.OnCoinsChanged -= setCoins;
		}
	}
}
