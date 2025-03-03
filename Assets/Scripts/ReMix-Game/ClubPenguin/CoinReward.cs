using ClubPenguin.Net.Domain;
using LitJson;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class CoinReward : IRewardable
	{
		[SerializeField]
		private int coins;

		public int Coins
		{
			get
			{
				return coins;
			}
		}

		public object Reward
		{
			get
			{
				return coins;
			}
		}

		public string RewardType
		{
			get
			{
				return "coins";
			}
		}

		public CoinReward()
		{
			coins = 0;
		}

		public CoinReward(int coins)
		{
			this.coins = coins;
		}

		public void FromJson(JsonData jsonData)
		{
			coins = (int)jsonData;
		}

		public void Add(IRewardable reward)
		{
			CoinReward coinReward = (CoinReward)reward;
			coins += coinReward.coins;
		}

		public bool IsEmpty()
		{
			return coins == 0;
		}
	}
}
