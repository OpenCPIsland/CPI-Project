using DisneyMobile.CoreUnitySystems;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Pooled : MonoBehaviour
	{
		public mg_GameObjectPool ManagingPool
		{
			get;
			private set;
		}

		public bool IntentionallyDestroying
		{
			get;
			set;
		}

		public static void CreateOnGameObject(GameObject _go, mg_GameObjectPool _managingPool)
		{
			Assert.NotNull(_managingPool, "Pooled objects must belong to a pool");
			mg_jr_Pooled mg_jr_Pooled = _go.AddComponent<mg_jr_Pooled>();
			mg_jr_Pooled.ManagingPool = _managingPool;
			mg_jr_Pooled.IntentionallyDestroying = false;
		}

		private void Start()
		{
			Assert.NotNull(ManagingPool, "mg_jr_Pooled components must be created by CreateOnGameObject");
		}

		private void OnDestroy()
		{
			if (!IntentionallyDestroying)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogDebug(this, "Pooled object is being destroyed. Did you mean to mg_jr_Resources.ReturnInstancedResource?");
			}
		}

		public void ReturnToPool()
		{
			ManagingPool.Return(base.gameObject);
		}
	}
}
