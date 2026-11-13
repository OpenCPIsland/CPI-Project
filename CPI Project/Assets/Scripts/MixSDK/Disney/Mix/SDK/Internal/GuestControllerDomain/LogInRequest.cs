namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class LogInRequest : AbstractGuestControllerWebCallRequest
	{
		public string loginValue
		{
			get;
			set;
		}

		public string password
		{
			get;
			set;
		}
	}
}
