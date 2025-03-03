using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IMixSessionStarter
	{
		void Start(string swid, string guestControllerAccessToken, Action<MixSessionStartResult> successCallback, Action failureCallback);
	}
}
