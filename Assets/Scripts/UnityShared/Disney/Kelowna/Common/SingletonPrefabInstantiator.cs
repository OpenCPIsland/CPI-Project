using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class SingletonPrefabInstantiator : MonoBehaviour
	{
		public GameObject Prefab;

		public void Start()
		{
			if (GameObject.Find("/" + Prefab.name) == null)
			{
				GameObject gameObject = Object.Instantiate(Prefab);
				gameObject.name = Prefab.name;
			}
			Object.Destroy(base.gameObject);
		}
	}
}
