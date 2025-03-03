using System;

namespace UnityEngine.UI.Extensions
{
	public class ObjectIdentifier : MonoBehaviour
	{
		public string prefabName;

		public string id;

		public string idParent;

		public bool dontSave = false;

		public void SetID()
		{
			id = Guid.NewGuid().ToString();
			CheckForRelatives();
		}

		private void CheckForRelatives()
		{
			if (base.transform.parent == null)
			{
				idParent = null;
				return;
			}
			ObjectIdentifier[] componentsInChildren = GetComponentsInChildren<ObjectIdentifier>();
			ObjectIdentifier[] array = componentsInChildren;
			foreach (ObjectIdentifier objectIdentifier in array)
			{
				if (objectIdentifier.transform.gameObject != base.gameObject)
				{
					objectIdentifier.idParent = id;
					objectIdentifier.SetID();
				}
			}
		}
	}
}
