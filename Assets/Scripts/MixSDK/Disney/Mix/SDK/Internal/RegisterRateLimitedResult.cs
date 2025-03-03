using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class RegisterRateLimitedResult : IRegisterRateLimitedResult, IRegisterResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}

		public ISession Session
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<IInvalidProfileItemError> Errors
		{
			get;
			private set;
		}

		public RegisterRateLimitedResult()
		{
			Errors = new IInvalidProfileItemError[0];
		}
	}
}
