using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Utility.DesignPatterns
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static volatile T m_instance;

		private static object m_lock = new object();

		private static bool applicationIsQuitting = false;

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					Logger.LogWarning(m_instance, string.Concat("[Singleton] Instance '", typeof(T), "' already destroyed on application quit. Won't create again - returning null."));
					return null;
				}
				lock (m_lock)
				{
					if ((Object)m_instance == (Object)null)
					{
						m_instance = (T)Object.FindObjectOfType(typeof(T));
						if (Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Logger.LogFatal(m_instance, "[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return m_instance;
						}
						if ((Object)m_instance == (Object)null)
						{
							GameObject gameObject = new GameObject();
							m_instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T).ToString();
							Object.DontDestroyOnLoad(gameObject);
							Logger.LogWarning(m_instance, string.Concat("[Singleton] An instance of ", typeof(T), " is needed in the scene, so '", gameObject, "' was created with DontDestroyOnLoad."));
						}
						else
						{
							Logger.LogTrace(m_instance, "[Singleton] Using instance already created: " + m_instance.gameObject.name);
						}
					}
					return m_instance;
				}
			}
		}

		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}
}
