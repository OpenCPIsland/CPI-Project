using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_GameObjectPool : mg_Pool<GameObject, mg_ICreator<GameObject>>
	{
		public override GameObject Fetch()
		{
			GameObject gameObject = null;
			if (m_collection.Count == 0)
			{
				gameObject = m_creator.Create();
				gameObject.SetActive(true);
				mg_jr_Pooled.CreateOnGameObject(gameObject, this);
			}
			else
			{
				gameObject = m_collection.Pop();
				gameObject.SetActive(true);
				gameObject.transform.parent = null;
			}
			return gameObject;
		}

		public override void Return(GameObject _objectToAddToPool)
		{
			Assert.NotNull(_objectToAddToPool, "Can't add null to a pool");
			mg_jr_Pooled component = _objectToAddToPool.GetComponent<mg_jr_Pooled>();
			Assert.NotNull(component, "Can't add an object to a pool it didn't come from: mg_jr_Pooled component missing");
			Assert.AreSame(this, component.ManagingPool, "Can't add an object to a pool it didn't come from: Managing pool is not this pool");
			_objectToAddToPool.transform.parent = base.transform;
			_objectToAddToPool.SetActive(false);
			m_collection.Push(_objectToAddToPool);
		}

		private void OnDestroy()
		{
			foreach (GameObject item in m_collection)
			{
				if (item != null)
				{
					mg_jr_Pooled component = item.GetComponent<mg_jr_Pooled>();
					component.IntentionallyDestroying = true;
				}
			}
		}
	}
}
