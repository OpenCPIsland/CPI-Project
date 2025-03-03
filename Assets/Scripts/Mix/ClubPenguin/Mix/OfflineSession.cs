using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using System;

namespace ClubPenguin.Mix
{
	internal class OfflineSession : ISession, IDisposable
	{
		private string userName;

		private ILocalUser localUser;

		public string GuestControllerAccessToken
		{
			get
			{
				return MD5HashUtil.GetHash(userName.ToLowerInvariant());
			}
		}

		public bool IsDisposed
		{
			get
			{
				return true;
			}
		}

		public ILocalUser LocalUser
		{
			get
			{
				return localUser;
			}
		}

		public TimeSpan ServerTimeOffset
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost;

		public event EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> OnGuestControllerAccessTokenChanged;

		public event EventHandler<AbstractSessionPausedEventArgs> OnPaused;

		public event EventHandler<AbstractSessionTerminatedEventArgs> OnTerminated;

		public OfflineSession(string userName)
		{
			this.userName = userName;
			localUser = new OfflineLocalUser(userName);
		}

		public void Dispose()
		{
		}

		public void Expire(Action<IExpireSessionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void LogOut(Action<ISessionLogOutResult> callback)
		{
			throw new NotImplementedException();
		}

		public void Pause(Action<IPauseSessionResult> callback)
		{
			throw new NotImplementedException();
		}

		public void Resume(Action<IResumeSessionResult> callback)
		{
			throw new NotImplementedException();
		}
	}
}
