using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.NPC;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupQuestsItem : MonoBehaviour
	{
		public Text QuestTitleText;

		public GameObject LockedOverlay;

		public Text CoinsText;

		public Text XPText;

		public void Init(QuestDefinition quest)
		{
			QuestTitleText.text = Service.Get<Localizer>().GetTokenTranslation(quest.Title);
			CoinsText.text = CoinRewardableDefinition.Coins(quest.CompleteReward).ToString();
			List<MascotXPRewardDefinition> definitions = quest.CompleteReward.GetDefinitions<MascotXPRewardDefinition>();
			XPText.text = definitions[0].XP.ToString();
			if (quest.isMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				LockedOverlay.SetActive(true);
			}
		}
	}
}
