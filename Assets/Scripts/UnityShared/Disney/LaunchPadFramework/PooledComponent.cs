using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class PooledComponent : MonoBehaviour, IPooledComponent
	{
		private string m_prefabName;

		private GameObjectPool m_objectPool;

		private float m_spawnTime;

		public string PrefabName
		{
			get
			{
				return m_prefabName;
			}
			set
			{
				m_prefabName = value;
			}
		}

		public GameObjectPool Pool
		{
			get
			{
				return m_objectPool;
			}
			set
			{
				m_objectPool = value;
			}
		}

		public float SpawnTime
		{
			get
			{
				return m_spawnTime;
			}
		}

		public bool IsSpawned
		{
			get
			{
				return m_spawnTime > 0f;
			}
		}

		public void OnSpawn()
		{
			m_spawnTime = Time.time;
		}

		public void Reset()
		{
			m_spawnTime = 0f;
		}
	}
}
