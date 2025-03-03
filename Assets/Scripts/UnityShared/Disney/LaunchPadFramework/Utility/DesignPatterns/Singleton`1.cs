using UnityEngine;

namespace Disney.LaunchPadFramework.Utility.DesignPatterns
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
					return null;
				}
				lock (m_lock)
				{
					if ((Object)m_instance == (Object)null)
					{
						m_instance = (T)Object.FindObjectOfType(typeof(T));
						if (Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Log.LogFatal(m_instance, "[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return m_instance;
						}
						if ((Object)m_instance == (Object)null)
						{
							GameObject gameObject = new GameObject();
							m_instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T).ToString();
							Object.DontDestroyOnLoad(gameObject);
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
