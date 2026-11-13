using UnityEngine;

namespace Disney.LaunchPadFramework
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
				CancelInvoke("killGameObject");
				Invoke("killGameObject", lifetimeInSeconds);
			}
		}

		public void OnSpawn()
		{
			Reset();
			Invoke("killGameObject", lifetimeInSeconds);
		}

		public void Reset()
		{
			CancelInvoke("killGameObject");
		}

		public void KillImmediately()
		{
			Reset();
			killGameObject();
		}

		private void killGameObject()
		{
			GameObjectPool gameObjectPool = null;
			PooledComponent component = base.gameObject.GetComponent<PooledComponent>();
			if (component != null)
			{
				gameObjectPool = component.Pool;
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
