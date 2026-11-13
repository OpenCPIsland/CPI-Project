using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractGuestControllerAccessTokenChangedEventArgs : EventArgs
	{
		public string GuestControllerAccessToken
		{
			get;
			protected set;
		}
	}
}
