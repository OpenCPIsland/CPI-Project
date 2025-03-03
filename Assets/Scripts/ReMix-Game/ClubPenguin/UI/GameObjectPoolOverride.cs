using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class GameObjectPoolOverride : MonoBehaviour
	{
		private const int DEFAULT_POOL_SIZE = 30;

		public PrefabContentKey ObjectPoolPrefab;

		public bool IsReady
		{
			get;
			private set;
		}

		public GameObjectPool ObjectPool
		{
			get;
			private set;
		}

		public event System.Action ObjectPoolReady;

		private void Awake()
		{
			IsReady = false;
			ClubPenguin.Core.SceneRefs.Set(this);
		}

		private void Start()
		{
			Content.LoadAsync(onPoolPrefabLoaded, ObjectPoolPrefab);
		}

		private void onPoolPrefabLoaded(string path, GameObject prefab)
		{
			setUpElementPool(30, prefab);
		}

		private void setUpElementPool(int itemCount, GameObject prefab)
		{
			GameObject gameObject = new GameObject("PoolOverride_" + prefab.name);
			gameObject.transform.SetParent(base.transform);
			ObjectPool = base.gameObject.AddComponent<GameObjectPool>();
			ObjectPool.PrefabToInstance = prefab;
			ObjectPool.Capacity = itemCount;
			ObjectPool.enabled = true;
			IsReady = true;
			if (this.ObjectPoolReady != null)
			{
				this.ObjectPoolReady();
			}
		}
	}
}
