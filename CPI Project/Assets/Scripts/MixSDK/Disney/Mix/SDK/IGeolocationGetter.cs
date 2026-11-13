using System;

namespace Disney.Mix.SDK
{
	public interface IGeolocationGetter
	{
		void Get(Action<IGetGeolocationResult> callback);
	}
}
