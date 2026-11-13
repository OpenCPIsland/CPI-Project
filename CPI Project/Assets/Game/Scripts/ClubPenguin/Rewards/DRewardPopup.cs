using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	public class DRewardPopup
	{
		public enum RewardPopupType
		{
			questComplete,
			levelUp,
			generic,
			replay
		}

		public RewardPopupType PopupType;

		public Reward RewardData;

		public string MascotName;

		public int XP;

		public string SplashTitleToken;

		public string SourceID;

		public bool ShowXpAndCoinsUI;

		public string RewardPopupPrefabOverride;

		public List<PrefabContentKey> CustomScreenKeys;

		public override string ToString()
		{
			return string.Format("[DRewardPopup] Type: {0} RewardData: {1} Mascot: {2} XP: {3} SplashTitleToken: {4} ShowXpAndCoinsUI: {5} RewardPopupPrefabOverride: {6}", PopupType, RewardData, MascotName, XP, SplashTitleToken, ShowXpAndCoinsUI, RewardPopupPrefabOverride);
		}
	}
}
