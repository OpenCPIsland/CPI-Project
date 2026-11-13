using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class GeolocationGetter : IGeolocationGetter
	{
		private readonly AbstractLogger logger;

		private readonly IMixWebCallFactory mixWebCallFactory;

		public GeolocationGetter(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory)
		{
			this.logger = logger;
			this.mixWebCallFactory = mixWebCallFactory;
		}

		public void Get(Action<IGetGeolocationResult> callback)
		{
			IWebCall<BaseUserRequest, GetGeolocationResponse> webCall = mixWebCallFactory.GeolocationPost(new BaseUserRequest());
			webCall.OnResponse += delegate(object sender, WebCallEventArgs<GetGeolocationResponse> e)
			{
				callback(new GetGeolocationResult(true, e.Response.CountryCode));
			};
			webCall.OnError += delegate
			{
				logger.Critical("Failed to get geolocation info");
				callback(new GetGeolocationResult(false, null));
			};
			webCall.Execute();
		}
	}
}
