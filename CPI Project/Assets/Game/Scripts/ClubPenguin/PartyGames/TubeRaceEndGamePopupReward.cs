using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.PartyGames
{
	public class TubeRaceEndGamePopupReward : MonoBehaviour
	{
		private const string MAX_XP_TOKEN = "MyProgress.MaxLevelText";

		public Text CoinText;

		public Text XpText;

		public void ShowReward(Dictionary<long, int> playerIdToPlacement, PartyGameDefinition definition)
		{
			PartyGameEndPlacement placementForLocalPlayer = getPlacementForLocalPlayer(playerIdToPlacement);
			Reward reward = getReward(placementForLocalPlayer, definition).ToReward();
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				CoinText.text = rewardable.Coins.ToString();
			}
			MascotXPReward rewardable2;
			if (reward.TryGetValue(out rewardable2))
			{
				Dictionary<string, int>.Enumerator enumerator = rewardable2.XP.GetEnumerator();
				enumerator.MoveNext();
				if (Service.Get<ProgressionService>().IsMascotMaxLevel(enumerator.Current.Key))
				{
					XpText.text = Service.Get<Localizer>().GetTokenTranslation("MyProgress.MaxLevelText");
				}
				else
				{
					XpText.text = enumerator.Current.Value.ToString();
				}
			}
		}

		private RewardDefinition getReward(PartyGameEndPlacement placement, PartyGameDefinition definition)
		{
			RewardDefinition result = null;
			for (int i = 0; i < definition.Rewards.Count; i++)
			{
				if (definition.Rewards[i].Placement == placement)
				{
					result = definition.Rewards[i].Reward;
					break;
				}
			}
			return result;
		}

		private PartyGameEndPlacement getPlacementForLocalPlayer(Dictionary<long, int> playerIdToPlacement)
		{
			PartyGameEndPlacement result = PartyGameEndPlacement.FIFTH;
			long localPlayerSessionId = Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
			if (playerIdToPlacement.ContainsKey(localPlayerSessionId))
			{
				result = (PartyGameEndPlacement)playerIdToPlacement[localPlayerSessionId];
			}
			return result;
		}
	}
}
