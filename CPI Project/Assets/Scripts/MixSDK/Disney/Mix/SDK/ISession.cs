using System;

namespace Disney.Mix.SDK
{
	public interface ISession : IDisposable
	{
		ILocalUser LocalUser
		{
			get;
		}

		string GuestControllerAccessToken
		{
			get;
		}

		TimeSpan ServerTimeOffset
		{
			get;
		}

		bool IsDisposed
		{
			get;
		}

		event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost;

		event EventHandler<AbstractSessionTerminatedEventArgs> OnTerminated;

		event EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> OnGuestControllerAccessTokenChanged;

		event EventHandler<AbstractSessionPausedEventArgs> OnPaused;

		void Pause(Action<IPauseSessionResult> callback);

		void Resume(Action<IResumeSessionResult> callback);

		void LogOut(Action<ISessionLogOutResult> callback);

		void Expire(Action<IExpireSessionResult> callback);
	}
}
