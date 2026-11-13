using System;

namespace Disney.LaunchPadFramework.PoolStrategies
{
	[Serializable]
	public class DoubleInSize : ObjectPoolGrowthStrategy
	{
		public override void Grow(GameObjectPool pool)
		{
			pool.Capacity *= 2;
		}

		public override void Grow<T>(ObjectPool<T> pool)
		{
			pool.Capacity *= 2;
		}
	}
}
