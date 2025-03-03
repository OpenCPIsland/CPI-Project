using System;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class ObjectPoolGrowthStrategy : ScriptableObject, IObjectPoolGrowthStrategy
	{
		public virtual void Grow(GameObjectPool pool)
		{
		}

		public virtual void Grow<T>(ObjectPool<T> pool) where T : class, new()
		{
		}
	}
}
