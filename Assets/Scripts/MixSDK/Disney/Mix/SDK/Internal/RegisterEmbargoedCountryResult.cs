using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class RegisterEmbargoedCountryResult : IRegisterEmbargoedCountryResult, IRegisterResult
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

		public RegisterEmbargoedCountryResult()
		{
			Errors = new IInvalidProfileItemError[0];
		}
	}
}
