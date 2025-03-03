using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class ApplicationService : MonoBehaviour
	{
		public struct Error
		{
			public string Type;

			public string Message;

			public Error(string type, string format, params object[] args)
			{
				Type = type;
				Message = string.Format(format, args);
			}
		}

		public bool IsAppPaused = false;

		public bool IsAppFocused = true;

		public bool IsAppResuming = false;

		public bool RequiresUpdate
		{
			get;
			set;
		}

		public bool UpdateAvailable
		{
			get;
			set;
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
		}

		private bool onSessionResumed(SessionEvents.SessionResumedEvent evt)
		{
			IsAppResuming = false;
			return false;
		}

		private void OnApplicationPause(bool isPaused)
		{
			IsAppPaused = isPaused;
			applicationPause(isPaused);
		}

		private void applicationPause(bool isPaused)
		{
			if (SceneManager.GetActiveScene().name == "Boot" || (Service.IsSet<MembershipService>() && Service.Get<MembershipService>().IsPurchaseInProgress))
			{
				return;
			}
			LoadingController loadingController = Service.Get<LoadingController>();
			if (Service.IsSet<SessionManager>() && Service.Get<SessionManager>().HasSession)
			{
				if (isPaused)
				{
					Service.Get<SessionManager>().PauseSession(true);
					if (!loadingController.HasLoadingSystem(this))
					{
						loadingController.AddLoadingSystem(this);
					}
				}
				else
				{
					IsAppResuming = true;
					Service.Get<SessionManager>().ResumeSession();
				}
			}
			if (Service.IsSet<ICPSwrveService>())
			{
				if (isPaused)
				{
					Service.Get<ICPSwrveService>().Pause();
				}
				else
				{
					Service.Get<ICPSwrveService>().Resume();
				}
			}
			if (!isPaused)
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}

		private void Update()
		{
			if (!IsAppPaused)
			{
			}
		}

		public void OnApplicationQuit()
		{
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (sessionManager != null)
			{
				sessionManager.DisposeSession();
			}
			if (Service.IsSet<ICPSwrveService>())
			{
				Service.Get<ICPSwrveService>().Quit();
			}
			if (Service.IsSet<IStandaloneErrorLogger>())
			{
				Service.Get<IStandaloneErrorLogger>().Flush();
			}
		}
	}
}
