using System;

namespace Disney.Mix.SDK.Internal
{
	public interface ISessionRestorer
	{
		void RestoreLastSession(Action<IRestoreLastSessionResult> callback);
	}
}
