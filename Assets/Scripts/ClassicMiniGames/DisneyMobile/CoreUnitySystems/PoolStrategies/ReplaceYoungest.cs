using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class ReplaceYoungest : ObjectPoolFullStrategy
	{
		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			GameObject youngestObject = GetYoungestObject(activeInstances);
			if (youngestObject != null)
			{
				youngestObject.SendMessage("OnStolenFromPool", SendMessageOptions.DontRequireReceiver);
			}
			return youngestObject;
		}

		private GameObject GetYoungestObject(List<GameObject> activeInstances)
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
						component.Reset();
					}
				}
				else
				{
					Logger.LogFatal(activeInstances[i], "Attempting to replace pooled instance based on lifetime (Youngest), however the game object or prefab is missing the required PooledLifetimeComponent!", Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
				}
			}
			return result;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			return GetYoungestObject(activeInstances);
		}

		private T GetYoungestObject<T>(List<T> activeInstances) where T : class, new()
		{
			T val = null;
			if (activeInstances.Count > 0)
			{
				val = activeInstances[activeInstances.Count - 1];
				activeInstances.RemoveAt(activeInstances.Count - 1);
				activeInstances.Insert(0, val);
			}
			return val;
		}
	}
}
