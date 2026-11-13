using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class AdultVerificationData
	{
		public string applicationId
		{
			get;
			set;
		}

		public string refId
		{
			get;
			set;
		}

		public bool verified
		{
			get;
			set;
		}

		public bool maxAttempts
		{
			get;
			set;
		}

		public List<AdultVerificationQuestion> questions
		{
			get;
			set;
		}
	}
}
