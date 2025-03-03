namespace Disney.Mix.SDK.Internal
{
	internal class UntrustedEventArgs : AbstractUntrustedEventArgs
	{
		public override IFriend ExTrustedFriend
		{
			get;
			protected set;
		}

		public UntrustedEventArgs(IFriend exTrustedFriend)
		{
			ExTrustedFriend = exTrustedFriend;
		}
	}
}
