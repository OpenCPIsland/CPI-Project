using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.PoolStrategies
{
	[Serializable]
	public class ReplaceClosestToTransform : ObjectPoolFullStrategy
	{
		[SerializeField]
		public Transform TargetTransform;

		public override GameObject GetObjectToReplace(GameObjectPool pool, List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			float num = float.MaxValue;
			GameObject gameObject = null;
			for (int i = 0; i < activeInstances.Count; i++)
			{
				float num2 = CalculateDistanceSquared(activeInstances[i].transform);
				if (num2 < num)
				{
					num = num2;
					gameObject = activeInstances[i];
				}
			}
			if (gameObject != null)
			{
				gameObject.SendMessage("OnStolenFromPool", SendMessageOptions.DontRequireReceiver);
			}
			return gameObject;
		}

		public override T GetObjectToReplace<T>(ObjectPool<T> pool, List<T> activeInstances, Stack<T> inactiveInstances)
		{
			Logger.LogFatal(pool, string.Format("ObjectPool<{0}> attempting to use replacement strategy {1} but it requires the type to be a GameObject!", typeof(T).Name, GetType().Name), Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
			return null;
		}

		private float CalculateDistanceSquared(Transform instanceTransform)
		{
			float result = float.MaxValue;
			if (TargetTransform != null)
			{
				result = (TargetTransform.position - instanceTransform.position).sqrMagnitude;
			}
			return result;
		}
	}
}
