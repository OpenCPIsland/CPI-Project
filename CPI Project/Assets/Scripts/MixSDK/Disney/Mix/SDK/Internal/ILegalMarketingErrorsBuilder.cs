using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public interface ILegalMarketingErrorsBuilder
	{
		void BuildErrors(ISession session, GuestApiErrorCollection errors, Action<BuildLegalMarketingErrorsResult> successCallback, Action failureCallback);
	}
}
