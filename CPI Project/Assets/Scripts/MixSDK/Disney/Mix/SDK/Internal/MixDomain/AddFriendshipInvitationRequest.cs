namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class AddFriendshipInvitationRequest : BaseUserRequest
	{
		public string InviteeDisplayName;

		public bool? IsTrusted;
	}
}
