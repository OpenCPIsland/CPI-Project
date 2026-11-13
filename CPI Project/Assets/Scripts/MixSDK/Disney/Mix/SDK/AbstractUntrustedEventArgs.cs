using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractUntrustedEventArgs : EventArgs
	{
		public abstract IFriend ExTrustedFriend
		{
			get;
			protected set;
		}
	}
}
