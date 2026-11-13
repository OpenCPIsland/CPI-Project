using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class SingletonAcrossScenes : MonoBehaviour
	{
		private static Dictionary<string, GameObject> instances;

		public void Awake()
		{
			if (instances == null)
			{
				instances = new Dictionary<string, GameObject>();
			}
			if (instances.ContainsKey(base.gameObject.name))
			{
				Object.Destroy(base.gameObject);
				return;
			}
			instances[base.gameObject.name] = base.gameObject;
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
