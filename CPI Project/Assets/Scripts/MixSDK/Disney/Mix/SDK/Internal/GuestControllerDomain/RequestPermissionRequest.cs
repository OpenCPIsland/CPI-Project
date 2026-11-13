namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class RequestPermissionRequest : AbstractGuestControllerWebCallRequest
	{
		public string activityCode
		{
			get;
			set;
		}
	}
}
