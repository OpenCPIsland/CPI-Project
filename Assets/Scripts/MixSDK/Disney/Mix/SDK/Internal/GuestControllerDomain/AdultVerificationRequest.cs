namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class AdultVerificationRequest : AbstractGuestControllerWebCallRequest
	{
		public string ssn
		{
			get;
			set;
		}

		public string dateOfBirth
		{
			get;
			set;
		}

		public string firstName
		{
			get;
			set;
		}

		public string lastName
		{
			get;
			set;
		}

		public string refId
		{
			get;
			set;
		}

		public AdultVerificationAddress address
		{
			get;
			set;
		}
	}
}
