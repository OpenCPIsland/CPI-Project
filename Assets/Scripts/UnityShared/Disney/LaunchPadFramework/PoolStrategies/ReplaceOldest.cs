using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework.PoolStrategies
{
	[Serializable]
	public class ReplaceOldest : ObjectPoolFullStrategy
	{
		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			GameObject oldestObject = GetOldestObject(activeInstances);
			if (oldestObject != null)
			{
				oldestObject.SendMessage("OnStolenFromPool", SendMessageOptions.DontRequireReceiver);
			}
			return oldestObject;
		}

		private GameObject GetOldestObject(List<GameObject> activeInstances)
		{
			float num = float.MaxValue;
			GameObject result = null;
			for (int i = 0; i < activeInstances.Count; i++)
			{
				PooledComponent component = activeInstances[i].GetComponent<PooledComponent>();
				if (component != null)
				{
					if (component.SpawnTime < num)
					{
						num = component.SpawnTime;
						result = activeInstances[i];
					}
				}
				else
				{
					Log.LogFatal(activeInstances[i], "Attempting to replace pooled instance based on lifetime (Oldest), however the game object or prefab is missing the required PooledLifetimeComponent!");
				}
			}
			return result;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			return GetOldestObject(activeInstances);
		}

		private T GetOldestObject<T>(List<T> activeInstances) where T : class, new()
		{
			T val = null;
			if (activeInstances.Count > 0)
			{
				val = activeInstances[0];
				activeInstances.RemoveAt(0);
				activeInstances.Add(val);
			}
			return val;
		}
	}
}
