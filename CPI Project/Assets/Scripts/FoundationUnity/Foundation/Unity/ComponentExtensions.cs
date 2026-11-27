using UnityEngine;

namespace Foundation.Unity
{
	public static class ComponentExtensions
	{
		public static void DestroyIfInstance(Object obj)
		{
			if (obj != null && IsInstance(obj))
			{
				Object.Destroy(obj);
			}
		}

		public static void DestroyIfAsset(Object obj)
		{
			if (obj != null && !IsInstance(obj) && obj.GetType() != typeof(GameObject))
			{
				Resources.UnloadAsset(obj);
			}
		}

		public static void DestroyResource(Object obj)
		{
			if (obj != null)
			{
				if (IsInstance(obj))
				{
					Object.Destroy(obj);
				}
				else if (obj.GetType() != typeof(GameObject))
				{
					Resources.UnloadAsset(obj);
				}
			}
		}

		private static bool IsInstance(Object obj)
		{
			return EntityId.ToULong(obj.GetEntityId()) > long.MaxValue;
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
				destroyMaterialInstances(componentsInChildren[i].materials);
			}
		}

		private static void destroyMaterialInstances(Material[] materials)
		{
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i] != null)
				{
					Object.Destroy(materials[i]);
				}
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
