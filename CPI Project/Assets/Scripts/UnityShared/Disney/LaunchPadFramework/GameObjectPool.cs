using Disney.LaunchPadFramework.PoolStrategies;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class GameObjectPool : MonoBehaviour
	{
		[SerializeField]
		public GameObject m_prefabToInstace;

		[SerializeField]
		protected int m_capacity = 8;

		[SerializeField]
		private ObjectPoolFullStrategy m_fullStrategy;

		[SerializeField]
		private bool m_sendNotifyMessages = false;

		private ObjectPool<GameObject> m_objectPool;

		public int Capacity
		{
			get
			{
				return m_capacity;
			}
			set
			{
				m_capacity = value;
				if (m_objectPool != null)
				{
					m_objectPool.Capacity = m_capacity;
				}
			}
		}

		public int ActiveInstanceCount
		{
			get
			{
				int result = 0;
				if (m_objectPool != null)
				{
					result = m_objectPool.ActiveInstanceCount;
				}
				return result;
			}
		}

		public float Utilization
		{
			get
			{
				float result = 0f;
				if (m_objectPool != null)
				{
					result = m_objectPool.Utilization;
				}
				return result;
			}
		}

		public ObjectPoolFullStrategy FullStrategy
		{
			get
			{
				return m_fullStrategy;
			}
			set
			{
				m_fullStrategy = value;
				if (m_objectPool != null)
				{
					m_objectPool.FullStrategy = m_fullStrategy;
				}
			}
		}

		public GameObject PrefabToInstance
		{
			get
			{
				return m_prefabToInstace;
			}
			set
			{
				m_prefabToInstace = value;
				if (m_objectPool != null)
				{
				}
				Initialize();
			}
		}

		public bool SendNotifyMessages
		{
			get
			{
				return m_sendNotifyMessages;
			}
			set
			{
				m_sendNotifyMessages = value;
			}
		}

		public void OnEnable()
		{
			if (m_prefabToInstace != null)
			{
				Initialize();
			}
		}

		public void OnDestroy()
		{
			Capacity = 0;
			m_objectPool = null;
		}

		public void UpdatePoolName()
		{
			string arg = "unknown";
			if (PrefabToInstance != null)
			{
				arg = PrefabToInstance.name;
			}
			base.name = string.Format("Pool[{0}]", arg);
		}

		public GameObject Spawn()
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Transform transform)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, transform);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(int priority)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectPriority(gameObject, priority);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(float lifeTimeInSeconds)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectLifetime(gameObject, lifeTimeInSeconds);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Transform transform, int priority)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, transform);
				SetObjectPriority(gameObject, priority);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Transform transform, float lifeTimeInSeconds)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, transform);
				SetObjectLifetime(gameObject, lifeTimeInSeconds);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Transform transform, int priority, float lifeTimeInSeconds)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, transform);
				SetObjectPriority(gameObject, priority);
				SetObjectLifetime(gameObject, lifeTimeInSeconds);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, position, rotation);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation, int priority)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, position, rotation);
				SetObjectPriority(gameObject, priority);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation, float lifeTimeInSeconds)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, position, rotation);
				SetObjectLifetime(gameObject, lifeTimeInSeconds);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public GameObject Spawn(Vector3 position, Quaternion rotation, int priority, float lifeTimeInSeconds)
		{
			GameObject gameObject = m_objectPool.Spawn();
			if (gameObject != null)
			{
				NotifyObjectOfReset(gameObject);
				SetObjectTransform(gameObject, position, rotation);
				SetObjectPriority(gameObject, priority);
				SetObjectLifetime(gameObject, lifeTimeInSeconds);
				gameObject.SetActive(true);
				NotifyObjectOfSpawn(gameObject);
			}
			return gameObject;
		}

		public void Unspawn(GameObject spawnedGameObject)
		{
			m_objectPool.Unspawn(spawnedGameObject);
		}

		public void UnspawnAllObjects()
		{
			if (m_objectPool != null)
			{
				m_objectPool.UnspawnAllObjects();
			}
		}

		protected void Initialize()
		{
			if (m_fullStrategy == null)
			{
				GrowPool growPool = ScriptableObject.CreateInstance<GrowPool>();
				growPool.GrowthStrategy = ScriptableObject.CreateInstance<ExpandByAmount>();
				m_fullStrategy = growPool;
			}
			int capacity = Capacity;
			if (m_objectPool != null)
			{
				Capacity = 0;
				m_objectPool = null;
			}
			if (m_objectPool == null)
			{
				m_objectPool = new ObjectPool<GameObject>(capacity, m_fullStrategy, OnAllocateObject, OnObjectAllocated, OnObjectDeallocated, OnObjectUnspawned, OnReplaceObject);
				Capacity = capacity;
			}
			UpdatePoolName();
		}

		protected GameObject OnAllocateObject()
		{
			GameObject gameObject = null;
			if (PrefabToInstance != null)
			{
				gameObject = Object.Instantiate(PrefabToInstance);
			}
			if (gameObject == null)
			{
				Log.LogError(this, "OnAllocateObject(), newObject is null.");
			}
			return gameObject;
		}

		protected void OnObjectAllocated(GameObject allocatedObject, int count)
		{
			if (allocatedObject != null)
			{
				PooledComponent pooledComponent = allocatedObject.AddComponent<PooledComponent>();
				pooledComponent.PrefabName = PrefabToInstance.name;
				pooledComponent.Pool = this;
				allocatedObject.name = string.Format("[{0}]{1} (Pooled)", count - 1, PrefabToInstance.name);
				SetObjectTransform(allocatedObject, base.gameObject.transform);
				allocatedObject.SetActive(false);
			}
		}

		protected void OnObjectDeallocated(GameObject deallocatedObject)
		{
			if (deallocatedObject != null)
			{
				deallocatedObject.DestroySafe();
			}
		}

		protected void OnObjectUnspawned(GameObject unspawnedObject)
		{
			if (!unspawnedObject.IsDestroyed())
			{
				NotifyObjectOfUnspawn(unspawnedObject);
				unspawnedObject.transform.SetParent(base.transform);
				unspawnedObject.SetActive(false);
			}
		}

		protected GameObject OnReplaceObject(List<GameObject> activeInstances, Stack<GameObject> inactiveInstances)
		{
			GameObject result = null;
			if (m_fullStrategy != null)
			{
				result = m_fullStrategy.GetObjectToReplace(this, activeInstances, inactiveInstances);
			}
			return result;
		}

		protected void SetObjectTransform(GameObject obj, Transform transform)
		{
			obj.transform.position = transform.position;
			obj.transform.rotation = transform.rotation;
			obj.transform.SetParent(base.transform);
		}

		protected void SetObjectTransform(GameObject obj, Vector3 position, Quaternion rotation)
		{
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.transform.SetParent(base.transform);
		}

		private void SetObjectPriority(GameObject obj, int priority)
		{
			PriorityComponent component = obj.GetComponent<PriorityComponent>();
			if (component != null)
			{
				component.Priority = priority;
			}
			else
			{
				Log.LogFatal(this, "Attempting to spawn a pool instance specifying the priority, but its GameObject or Prefab does not contain the PooledPriorityComponent.");
			}
		}

		private void SetObjectLifetime(GameObject obj, float lifeTimeInSeconds)
		{
			LifetimeComponent component = obj.GetComponent<LifetimeComponent>();
			if (component != null)
			{
				component.SetLifetime(lifeTimeInSeconds);
			}
			else
			{
				Log.LogFatal(this, "Attempting to spawn a pool instance specifying the lifetime, but its GameObject or Prefab does not contain the PooledLifetimeComponent.");
			}
		}

		private void NotifyObjectOfReset(GameObject obj)
		{
			if (m_sendNotifyMessages)
			{
				obj.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void NotifyObjectOfSpawn(GameObject obj)
		{
			if (m_sendNotifyMessages)
			{
				obj.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
			}
		}

		private void NotifyObjectOfUnspawn(GameObject obj)
		{
			if (m_sendNotifyMessages)
			{
				obj.SendMessage("OnUnspawn", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
