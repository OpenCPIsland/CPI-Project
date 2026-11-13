using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[Serializable]
	public class SceneObject
	{
		public string name;

		public string prefabName;

		public string id;

		public string idParent;

		public bool active;

		public Vector3 position;

		public Vector3 localScale;

		public Quaternion rotation;

		public List<ObjectComponent> objectComponents = new List<ObjectComponent>();
	}
}
