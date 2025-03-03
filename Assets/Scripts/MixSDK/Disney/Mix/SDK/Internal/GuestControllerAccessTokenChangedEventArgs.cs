namespace Disney.Mix.SDK.Internal
{
	public class GuestControllerAccessTokenChangedEventArgs : AbstractGuestControllerAccessTokenChangedEventArgs
	{
		public GuestControllerAccessTokenChangedEventArgs(string guestControllerAccessToken)
		{
			base.GuestControllerAccessToken = guestControllerAccessToken;
		}
	}
}
