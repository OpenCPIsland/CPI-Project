using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class AdultVerificationQuizRequest : AbstractGuestControllerWebCallRequest
	{
		public string applicationId
		{
			get;
			set;
		}

		public List<AdultVerificationQuizAnswer> answers
		{
			get;
			set;
		}
	}
}
