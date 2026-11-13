using ClubPenguin.Net.Domain;
using LitJson;
using System;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class IglooSlotsReward : IRewardable
	{
		[SerializeField]
		private int iglooSlots;

		public int IglooSlots
		{
			get
			{
				return iglooSlots;
			}
		}

		public object Reward
		{
			get
			{
				return iglooSlots;
			}
		}

		public string RewardType
		{
			get
			{
				return "iglooSlots";
			}
		}

		public IglooSlotsReward()
		{
			iglooSlots = 0;
		}

		public IglooSlotsReward(int iglooSlots)
		{
			this.iglooSlots = iglooSlots;
		}

		public void FromJson(JsonData jsonData)
		{
			iglooSlots = (int)jsonData;
		}

		public void Add(IRewardable reward)
		{
			IglooSlotsReward iglooSlotsReward = (IglooSlotsReward)reward;
			iglooSlots += iglooSlotsReward.iglooSlots;
		}

		public bool IsEmpty()
		{
			return iglooSlots == 0;
		}
	}
}
