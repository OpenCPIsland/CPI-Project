using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework.PoolStrategies
{
	[Serializable]
	public class DropObject : ObjectPoolFullStrategy
	{
		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			return null;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			return null;
		}
	}
}
