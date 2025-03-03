using System;

namespace Disney.Mix.SDK.Internal
{
	public interface ISessionLogin
	{
		void Login(string username, string password, Action<ILoginResult> callback);
	}
}
