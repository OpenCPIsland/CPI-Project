using UnityEngine;

namespace EasyTouchPlugin
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T m_Instance = null;

		public static T instance
		{
			get
			{
				if ((Object)m_Instance == (Object)null)
				{
					m_Instance = (Object.FindObjectOfType(typeof(T)) as T);
					if ((Object)m_Instance == (Object)null)
					{
						m_Instance = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
						m_Instance.Init();
					}
				}
				return m_Instance;
			}
		}

		private void Awake()
		{
			if ((Object)m_Instance == (Object)null)
			{
				m_Instance = (this as T);
			}
		}

		public virtual void Init()
		{
		}

		private void OnApplicationQuit()
		{
			m_Instance = null;
		}
	}
}
