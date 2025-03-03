using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractAccountBannedEventArgs : AbstractAuthenticationLostEventArgs
	{
		public DateTime? ExpirationDate
		{
			get;
			protected set;
		}
	}
}
