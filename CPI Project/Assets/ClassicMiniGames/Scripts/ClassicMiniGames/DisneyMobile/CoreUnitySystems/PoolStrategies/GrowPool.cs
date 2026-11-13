using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class GrowPool : ObjectPoolFullStrategy
	{
		[SerializeField]
		private ObjectPoolGrowthStrategy m_growthStrategy;

		public ObjectPoolGrowthStrategy GrowthStrategy
		{
			get
			{
				return m_growthStrategy;
			}
			set
			{
				m_growthStrategy = value;
			}
		}

		public void OnEnable()
		{
			if (m_growthStrategy == null)
			{
				m_growthStrategy = ScriptableObject.CreateInstance<DoubleInSize>();
			}
		}

		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			GameObject result = null;
			if (m_growthStrategy != null)
			{
				m_growthStrategy.Grow(pool);
				result = pool.Spawn();
			}
			return result;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			T result = null;
			if (m_growthStrategy != null)
			{
				m_growthStrategy.Grow(pool);
				return pool.Spawn();
			}
			return result;
		}
	}
}
