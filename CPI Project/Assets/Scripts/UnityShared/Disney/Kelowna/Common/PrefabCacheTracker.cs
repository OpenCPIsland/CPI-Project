using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class PrefabCacheTracker : MonoBehaviour
	{
		public class PrefabRequest
		{
			public bool IsComplete;

			public PrefabContentKey ContentKey;

			public GameObject Prefab;
		}

		private ContentCache<GameObject> gameObjectContentCache;

		private List<PrefabCacheTrackerComponent> cachedPrefabs;

		public event Action ReferencesRemoved;

		private void Awake()
		{
			gameObjectContentCache = new ContentCache<GameObject>(releasePrefabGameObject);
			cachedPrefabs = new List<PrefabCacheTrackerComponent>();
		}

		private void releasePrefabGameObject(GameObject prefabGameObject, List<UnityEngine.Object> subList)
		{
			if (this.ReferencesRemoved != null)
			{
				this.ReferencesRemoved();
			}
		}

		public PrefabRequest Acquire(PrefabContentKey contentKey, Action<GameObject, PrefabRequest> callback = null)
		{
			PrefabRequest prefabRequest = new PrefabRequest
			{
				IsComplete = false,
				ContentKey = contentKey
			};
			CoroutineRunner.Start(gameObjectContentCache.Acquire(contentKey, delegate(GameObject x)
			{
				onPrefabLoaded(x, prefabRequest, callback);
			}), this, "Loading Prefab Asset");
			return prefabRequest;
		}

		private void onPrefabLoaded(GameObject prefab, PrefabRequest request, Action<GameObject, PrefabRequest> callback)
		{
			request.Prefab = prefab;
			request.IsComplete = true;
			if (callback != null)
			{
				callback(prefab, request);
			}
		}

		public void SetCache(GameObject prefabInstance, PrefabContentKey contentKey)
		{
			PrefabCacheTrackerComponent prefabCacheTrackerComponent = prefabInstance.AddComponent<PrefabCacheTrackerComponent>();
			prefabCacheTrackerComponent.SetContentKey(contentKey);
			prefabCacheTrackerComponent.ObjectDestroyed += onPrefabInstanceDestroyed;
			cachedPrefabs.Add(prefabCacheTrackerComponent);
		}

		private void onPrefabInstanceDestroyed(PrefabCacheTrackerComponent trackerComponent)
		{
			trackerComponent.ObjectDestroyed -= onPrefabInstanceDestroyed;
			cachedPrefabs.Remove(trackerComponent);
			gameObjectContentCache.Release(trackerComponent.ContentKey);
		}

		public void Release(PrefabContentKey contentKey)
		{
			gameObjectContentCache.Release(contentKey);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			for (int i = 0; i < cachedPrefabs.Count; i++)
			{
				cachedPrefabs[i].ObjectDestroyed -= onPrefabInstanceDestroyed;
				gameObjectContentCache.Release(cachedPrefabs[i].ContentKey);
			}
			cachedPrefabs.Clear();
			this.ReferencesRemoved = null;
		}
	}
}
