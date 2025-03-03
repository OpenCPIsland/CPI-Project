using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractUnfriendedEventArgs : EventArgs
	{
		public abstract IFriend ExFriend
		{
			get;
			protected set;
		}
	}
}
