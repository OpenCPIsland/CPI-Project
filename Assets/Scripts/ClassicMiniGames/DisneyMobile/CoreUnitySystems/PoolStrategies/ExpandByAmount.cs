using System;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class ExpandByAmount : ObjectPoolGrowthStrategy
	{
		[SerializeField]
		private int m_amount = 8;

		public int Amount
		{
			get
			{
				return m_amount;
			}
			set
			{
				m_amount = value;
			}
		}

		public override void Grow(GameObjectPool pool)
		{
			pool.Capacity += Amount;
		}

		public override void Grow<T>(ObjectPool<T> pool)
		{
			pool.Capacity += Amount;
		}
	}
}
