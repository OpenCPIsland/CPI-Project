using Foundation.Unity;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[DisallowMultipleComponent]
	public class ResourceCleaner : MonoBehaviour
	{
		public delegate void DestroyCallback(GameObject gameObject);

		public bool CleanOnDisable = false;

		public DestroyCallback OnDestroyCallback;

		public void OnDisable()
		{
			ResourceCleaner componentInParent = GetComponentInParent<ResourceCleaner>();
			if (componentInParent != null && componentInParent.CleanOnDisable)
			{
				this.DestroyResources();
			}
		}

		public void OnDestroy()
		{
			ResourceCleaner componentInParent = GetComponentInParent<ResourceCleaner>();
			if (componentInParent == null)
			{
				this.DestroyResources();
			}
			if (OnDestroyCallback != null)
			{
				OnDestroyCallback(base.gameObject);
			}
		}
	}
}
