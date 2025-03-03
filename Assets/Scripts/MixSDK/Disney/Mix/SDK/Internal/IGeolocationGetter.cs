using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IGeolocationGetter
	{
		void Get(Action<IGetGeolocationResult> callback);
	}
}
