using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameTeamEndGamePopupReward : MonoBehaviour
	{
		public Text CoinText;

		public Text XpText;

		public GameObject XpPanel;

		public GameObject CoinPanel;

		public void SetReward(Reward reward)
		{
			displayCoins(reward);
			displayXP(reward);
		}

		private void displayCoins(Reward reward)
		{
			CoinReward rewardable;
			if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
			{
				CoinPanel.SetActive(true);
				CoinText.text = rewardable.Coins.ToString();
			}
			else
			{
				CoinPanel.SetActive(false);
			}
		}

		private void displayXP(Reward reward)
		{
			MascotXPReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				if (rewardable.XP.Count > 1)
				{
				}
				Dictionary<string, int>.Enumerator enumerator = rewardable.XP.GetEnumerator();
				enumerator.MoveNext();
				KeyValuePair<string, int> current = enumerator.Current;
				XpPanel.SetActive(true);
				XpText.text = current.Value.ToString();
			}
			else
			{
				XpPanel.SetActive(false);
			}
		}
	}
}
