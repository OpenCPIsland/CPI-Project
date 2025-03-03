using DisneyMobile.CoreUnitySystems.Utility.DesignPatterns;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class LifetimeComponent : MonoBehaviour
	{
		public float lifetimeInSeconds = 10f;

		public void SetLifetime(float newLifetime)
		{
			lifetimeInSeconds = newLifetime;
			PooledComponent component = base.gameObject.GetComponent<PooledComponent>();
			if (component != null && component.IsSpawned)
			{
				CancelInvoke("KillGameObject");
				Invoke("KillGameObject", lifetimeInSeconds);
			}
		}

		public void OnSpawn()
		{
			Reset();
			Invoke("KillGameObject", lifetimeInSeconds);
		}

		public void Reset()
		{
			CancelInvoke("KillGameObject");
		}

		private void KillGameObject()
		{
			GameObjectPool gameObjectPool = null;
			PooledComponent component = base.gameObject.GetComponent<PooledComponent>();
			if (component != null)
			{
				gameObjectPool = Singleton<GameObjectPoolManager>.Instance.GetPool(component.PrefabName);
			}
			if (gameObjectPool != null)
			{
				gameObjectPool.Unspawn(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
