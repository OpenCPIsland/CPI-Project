using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IInternalSession : ISession, IDisposable
	{
		IInternalLocalUser InternalLocalUser
		{
			get;
		}
	}
}
