namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class RecoverRequest : AbstractGuestControllerWebCallRequest
	{
		public string lookupValue
		{
			get;
			set;
		}

		public string referenceId
		{
			get;
			set;
		}
	}
}
