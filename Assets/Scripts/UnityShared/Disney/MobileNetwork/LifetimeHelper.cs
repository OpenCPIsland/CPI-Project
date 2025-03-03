using UnityEngine;

namespace Disney.MobileNetwork
{
	public class LifetimeHelper : MonoBehaviour
	{
		public delegate void ApplicationStartDelegate();

		public delegate void ApplicationPauseDelegate(bool paused);

		public delegate void ApplicationQuitDelegate();

		public event ApplicationStartDelegate OnApplicationStartEvent = delegate
		{
		};

		public event ApplicationPauseDelegate OnApplicationPauseEvent = delegate
		{
		};

		public event ApplicationQuitDelegate OnApplicationQuitEvent = delegate
		{
		};

		private void Awake()
		{
			Service.Set(this);
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Start()
		{
			this.OnApplicationStartEvent();
		}

		private void OnApplicationPause(bool paused)
		{
			this.OnApplicationPauseEvent(paused);
		}

		public void OnApplicationQuit()
		{
			this.OnApplicationQuitEvent();
		}
	}
}
