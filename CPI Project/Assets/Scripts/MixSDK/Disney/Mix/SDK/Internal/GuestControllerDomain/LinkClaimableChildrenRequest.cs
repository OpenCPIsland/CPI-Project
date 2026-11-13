namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class LinkClaimableChildrenRequest : AbstractGuestControllerWebCallRequest
	{
		public string[] swids
		{
			get;
			set;
		}
	}
}
