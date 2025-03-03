using Disney.LaunchPadFramework.PoolStrategies;
using Disney.LaunchPadFramework.Utility;
using Disney.LaunchPadFramework.Utility.DesignPatterns;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class GameObjectPoolManager : Singleton<GameObjectPoolManager>
	{
		private List<GameObjectPool> m_pools = new List<GameObjectPool>();

		private static Dictionary<string, GameObjectPool> m_poolLookup = new Dictionary<string, GameObjectPool>();

		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Start()
		{
			UpdatePoolMapping();
		}

		public void UpdatePoolMapping()
		{
			m_pools.Clear();
			m_poolLookup.Clear();
			GameObjectPool[] componentsInChildren = GetComponentsInChildren<GameObjectPool>();
			foreach (GameObjectPool gameObjectPool in componentsInChildren)
			{
				if (gameObjectPool.PrefabToInstance != null)
				{
					m_pools.Add(gameObjectPool);
					if (m_poolLookup.ContainsKey(gameObjectPool.PrefabToInstance.name))
					{
						Log.LogFatal(this, string.Format("Pool Manager contains multiple GameObjectPools for the same Prefab: {0}, to fix this ensure that only one pool exists for each type of GameObject", gameObjectPool.PrefabToInstance.name));
					}
					else
					{
						m_poolLookup.Add(gameObjectPool.PrefabToInstance.name, gameObjectPool);
					}
				}
			}
		}

		public GameObjectPool AddPoolForObject(GameObject prefabObject)
		{
			return AddPoolForObject(prefabObject, null);
		}

		public GameObjectPool AddPoolForObject(GameObject prefabObject, ObjectPoolFullStrategy fullStrategy)
		{
			GameObjectPool gameObjectPool = null;
			if (prefabObject != null && prefabObject.GetType() != typeof(GameObjectPool) && prefabObject.GetType() != typeof(GameObjectPoolManager))
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.parent = base.gameObject.transform;
				gameObjectPool = gameObject.AddComponent<GameObjectPool>();
				if (gameObjectPool != null)
				{
					gameObjectPool.PrefabToInstance = prefabObject;
					gameObjectPool.UpdatePoolName();
				}
				if (fullStrategy != null)
				{
					gameObjectPool.FullStrategy = fullStrategy;
				}
				else
				{
					gameObjectPool.FullStrategy = new DropObject();
				}
				UpdatePoolMapping();
			}
			else if (prefabObject == null)
			{
				Log.LogFatal(this, "Cannot create a pool for null prefab!");
			}
			else
			{
				Log.LogFatal(this, string.Format("Cannot create a pool of type: {0}", prefabObject.GetType()));
			}
			return gameObjectPool;
		}

		public GameObjectPool GetPool(string prefabName)
		{
			GameObjectPool value = null;
			if (!m_poolLookup.SafeTryGetValue(prefabName, out value))
			{
			}
			return value;
		}

		public bool TryGetPool(string prefabName, out GameObjectPool foundPool)
		{
			return m_poolLookup.SafeTryGetValue(prefabName, out foundPool);
		}

		public GameObjectPool GetPool(GameObject prefabObject)
		{
			return GetPool(prefabObject.name);
		}

		public List<GameObjectPool> GetPools()
		{
			return m_pools;
		}

		public void UnspawnAllPools()
		{
			foreach (GameObjectPool pool in m_pools)
			{
				pool.UnspawnAllObjects();
			}
		}

		protected GameObjectPoolManager()
		{
		}
	}
}
