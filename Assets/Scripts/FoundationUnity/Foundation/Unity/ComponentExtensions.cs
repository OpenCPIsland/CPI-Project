using UnityEngine;

namespace Foundation.Unity
{
	public static class ComponentExtensions
	{
		public static void DestroyIfInstance(Object obj)
		{
			if (obj != null && obj.GetInstanceID() < 0)
			{
				Object.Destroy(obj);
			}
		}

		public static void DestroyIfAsset(Object obj)
		{
			if (obj != null && obj.GetInstanceID() >= 0 && obj.GetType() != typeof(GameObject))
			{
				Resources.UnloadAsset(obj);
			}
		}

		public static void DestroyResource(Object obj)
		{
			if (obj != null)
			{
				if (obj.GetInstanceID() < 0)
				{
					Object.Destroy(obj);
				}
				else if (obj.GetType() != typeof(GameObject))
				{
					Resources.UnloadAsset(obj);
				}
			}
		}

		public static void UnloadAssets(this GameObject go)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				cleanupMaterials(componentsInChildren[i].sharedMaterials);
			}
		}

		public static void DestroyResources(this Component component)
		{
			Renderer[] componentsInChildren = component.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				cleanupMaterials(componentsInChildren[i].sharedMaterials);
				cleanupMaterials(componentsInChildren[i].materials);
			}
		}

		private static void cleanupMaterials(Material[] materials)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				DestroyResource(materials[i]);
			}
		}
	}
}
