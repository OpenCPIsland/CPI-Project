using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Utils/ExternalGroupComponent")]
	public class ExternalGroupComponent : MonoBehaviour
	{
		[SerializeField]
		public GameObject[] _prefabs;

		[NonSerialized]
		public List<GameObject> _prefabInstances = new List<GameObject>();

		private void Awake()
		{
			GameObject[] prefabs = _prefabs;
			foreach (GameObject prefab in prefabs)
			{
				RegisterPrefab(prefab);
			}
		}

		private void OnDestroy()
		{
			foreach (GameObject prefabInstance in _prefabInstances)
			{
				if (prefabInstance != null)
				{
					UnregisterPrefab(prefabInstance);
				}
			}
			_prefabInstances.Clear();
		}

		private void RegisterPrefab(GameObject prefab)
		{
			if (prefab == null)
			{
				return;
			}
			GroupComponent component = prefab.GetComponent<GroupComponent>();
			if (component == null)
			{
				return;
			}
			GameObject gameObject = GameObject.Find(prefab.name);
			if (gameObject == null)
			{
				GroupComponent._createProxy = false;
				gameObject = UnityEngine.Object.Instantiate(prefab);
				GroupComponent._createProxy = true;
				gameObject.name = gameObject.name.Replace("(Clone)", "");
			}
			if (gameObject != null)
			{
				GroupComponent component2 = gameObject.GetComponent<GroupComponent>();
				if (component2 != null)
				{
					component2.IncRef();
				}
				_prefabInstances.Add(gameObject);
			}
		}

		private void UnregisterPrefab(GameObject prefab)
		{
			GroupComponent component = prefab.GetComponent<GroupComponent>();
			if (component != null)
			{
				component.DecRef();
				if (component._registeredWithMainRefCount == 0)
				{
					UnityEngine.Object.Destroy(component.gameObject);
				}
			}
		}
	}
}
