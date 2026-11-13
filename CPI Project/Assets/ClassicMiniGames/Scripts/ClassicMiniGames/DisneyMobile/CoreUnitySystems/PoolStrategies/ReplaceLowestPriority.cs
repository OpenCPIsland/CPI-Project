using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class ReplaceLowestPriority : ObjectPoolFullStrategy
	{
		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			int num = int.MaxValue;
			GameObject gameObject = null;
			for (int i = 0; i < activeInstances.Count; i++)
			{
				PriorityComponent component = activeInstances[i].GetComponent<PriorityComponent>();
				if (component != null)
				{
					int priority = component.Priority;
					if (priority < num)
					{
						num = priority;
						gameObject = activeInstances[i];
					}
				}
				else
				{
					Logger.LogFatal(activeInstances[i], "Attempting to replace pooled instance based on priority, however the game object or prefab is missing the required PooledPriorityComponent!", Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
				}
			}
			if (gameObject != null)
			{
				gameObject.SendMessage("OnStolenFromPool", SendMessageOptions.DontRequireReceiver);
				PriorityComponent component = gameObject.GetComponent<PriorityComponent>();
				if (component != null)
				{
					component.Reset();
				}
			}
			return gameObject;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> m_activeInstances, Stack<T> m_inactiveInstances)
		{
			Logger.LogFatal(pool, string.Format("ObjectPool<{0}> attempting to use replacement strategy {1} but it requires the type to be a GameObject!", typeof(T).Name, GetType().Name), Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
			return null;
		}
	}
}
