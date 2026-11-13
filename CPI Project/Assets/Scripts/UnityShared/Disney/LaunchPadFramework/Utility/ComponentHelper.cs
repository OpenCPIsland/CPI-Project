using UnityEngine;

namespace Disney.LaunchPadFramework.Utility
{
	public static class ComponentHelper
	{
		public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
		{
			T component = obj.GetComponent<T>();
			if ((Object)component == (Object)null)
			{
				Log.LogFatal(obj, string.Concat("Expected to find component of type ", typeof(T), " but found none"));
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
