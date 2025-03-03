using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Utility
{
	public static class ComponentHelper
	{
		public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
		{
			T component = obj.GetComponent<T>();
			if ((Object)component == (Object)null)
			{
				Logger.LogFatal(obj, string.Concat("Expected to find component of type ", typeof(T), " but found none"), Logger.TagFlags.GAME | Logger.TagFlags.ASSET);
			}
			return component;
		}

		public static string GetSafeName(this MonoBehaviour obj)
		{
			if (obj != null)
			{
				return obj.name;
			}
			return "(null)";
		}
	}
}
