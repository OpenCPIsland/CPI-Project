#define UNITY_ASSERTIONS
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class CoinsData : ScopedData
	{
		[SerializeField]
		private int coins = 0;

		public int Coins
		{
			get
			{
				return coins;
			}
			internal set
			{
				sendCoinEvents(value);
				coins = value;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(CoinsDataMonoBehaviour);
			}
		}

		public event Action<int, bool> OnCoinsAdded;

		public event Action<int> OnCoinsRemoved;

		public event Action<int> OnCoinsChanged;

		public event Action<int> OnCoinsSet;

		protected override void notifyWillBeDestroyed()
		{
			this.OnCoinsAdded = null;
			this.OnCoinsRemoved = null;
			this.OnCoinsChanged = null;
		}

		public void SetInitialCoins(int coins)
		{
			this.coins = coins;
			if (this.OnCoinsChanged != null)
			{
				this.OnCoinsChanged(coins);
			}
			if (this.OnCoinsSet != null)
			{
				this.OnCoinsSet(coins);
			}
		}

		public void AddCoins(int coins, bool isCollectible = false, bool show = true)
		{
			Debug.Assert(coins >= 0);
			this.coins += coins;
			if (show && this.OnCoinsAdded != null)
			{
				this.OnCoinsAdded(coins, isCollectible);
			}
		}

		public void RemoveCoins(int coins)
		{
			Debug.Assert(coins >= 0);
			Coins = this.coins - coins;
		}

		private void sendCoinEvents(int newCount)
		{
			if (newCount > coins && this.OnCoinsAdded != null)
			{
				this.OnCoinsAdded(newCount - coins, false);
			}
			if (newCount < coins && this.OnCoinsRemoved != null)
			{
				this.OnCoinsRemoved(coins - newCount);
			}
			if (newCount != coins && this.OnCoinsChanged != null)
			{
				this.OnCoinsChanged(newCount);
			}
		}
	}
}
