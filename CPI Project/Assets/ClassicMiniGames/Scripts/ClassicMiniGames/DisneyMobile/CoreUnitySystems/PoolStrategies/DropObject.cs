using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class DropObject : ObjectPoolFullStrategy
	{
		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			Logger.LogWarning(this, string.Format("Pool for type {0} was exhausted, dropping the request to replace an object in the pool. Consider expanding the pools initial capacity.", typeof(GameObject)), Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
			return null;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			Logger.LogWarning(this, string.Format("Pool for type {0} was exhausted, dropping the request to replace an object in the pool. Consider expanding the pools initial capacity.", typeof(T).Name), Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
			return null;
		}
	}
}
