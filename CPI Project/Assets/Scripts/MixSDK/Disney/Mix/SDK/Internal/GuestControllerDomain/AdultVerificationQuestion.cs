using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class AdultVerificationQuestion
	{
		public int questionId
		{
			get;
			set;
		}

		public string questionText
		{
			get;
			set;
		}

		public List<string> choices
		{
			get;
			set;
		}
	}
}
