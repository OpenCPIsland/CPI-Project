using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework.PoolStrategies
{
	[Serializable]
	public class ObjectPoolFullStrategy : ScriptableObject, IObjectPoolFullStrategy
	{
		public virtual GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			return null;
		}

		public virtual T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances) where T : class, new()
		{
			return null;
		}
	}
}
