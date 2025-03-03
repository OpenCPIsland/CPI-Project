namespace Disney.PushNotificationUnityPlugin.Internal
{
	public class TokenGeneratedEventArgs : AbstractTokenGeneratedEventArgs
	{
		public override string Token
		{
			get;
			protected set;
		}

		public TokenGeneratedEventArgs(string token)
		{
			Token = token;
		}
	}
}
