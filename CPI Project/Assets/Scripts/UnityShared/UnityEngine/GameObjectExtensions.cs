using System.Collections.Generic;

namespace UnityEngine
{
	public static class GameObjectExtensions
	{
		public static void ChangeLayersRecursively(this Transform trans, int layer, int layerOnly = -1)
		{
			if (trans.gameObject.layer == layerOnly || layerOnly == -1)
			{
				trans.gameObject.layer = layer;
			}
			foreach (Transform tran in trans)
			{
				tran.ChangeLayersRecursively(layer, layerOnly);
			}
		}

		public static string GetPath(this Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return current.parent.GetPath() + "/" + current.name;
		}

		public static string GetPath(this GameObject go)
		{
			return (go.transform != null) ? go.transform.GetPath() : "";
		}

		public static string GetPath(this Component component)
		{
			if (component.transform != null)
			{
				return component.transform.GetPath() + "/" + component.GetType();
			}
			return "";
		}

		public static IList<Transform> GetChildren(this GameObject gameObject, bool includeInactive = true, bool includeSelf = false)
		{
			List<Transform> list = new List<Transform>();
			Transform[] componentsInChildren = gameObject.gameObject.GetComponentsInChildren<Transform>(includeInactive);
			foreach (Transform transform in componentsInChildren)
			{
				if (gameObject.transform == transform)
				{
					if (includeSelf)
					{
						list.Add(transform);
					}
				}
				else
				{
					list.Add(transform);
				}
			}
			return list;
		}

		public static IList<string> GetChildrenPaths(this GameObject gameObject, bool includeInactive = true, bool includeSelf = false)
		{
			IList<Transform> children = gameObject.GetChildren(includeInactive, includeSelf);
			List<string> list = new List<string>(children.Count);
			foreach (Transform item in children)
			{
				list.Add(item.GetPath());
			}
			return list;
		}

		public static bool IsDestroyed(this GameObject gameObject)
		{
			try
			{
				return !gameObject || gameObject == null || object.ReferenceEquals(gameObject, null) || gameObject.Equals(null);
			}
			catch (MissingReferenceException)
			{
			}
			return true;
		}

		public static bool IsDestroyed(this Transform transform)
		{
			try
			{
				return !transform || transform == null || object.ReferenceEquals(transform, null);
			}
			catch (MissingReferenceException)
			{
			}
			return true;
		}

		public static void DestroySafe(this GameObject gameObject)
		{
			Object.Destroy(gameObject);
		}

		public static int GetChildCount<T>(this GameObject gameObject)
		{
			int num = 0;
			foreach (Transform child in gameObject.GetChildren())
			{
				if (child.GetComponent<T>() != null)
				{
					num++;
				}
			}
			return num;
		}

		public static T AddComponentIfMissing<T>(this GameObject gameObject) where T : Component
		{
			T val = gameObject.GetComponent<T>();
			if ((Object)val == (Object)null)
			{
				val = gameObject.AddComponent<T>();
			}
			return val;
		}
	}
}
