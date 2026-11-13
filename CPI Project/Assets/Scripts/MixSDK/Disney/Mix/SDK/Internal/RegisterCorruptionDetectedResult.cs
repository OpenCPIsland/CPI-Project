using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class RegisterCorruptionDetectedResult : IRegisterCorruptionDetectedResult, IRegisterResult
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

		public RegisterCorruptionDetectedResult()
		{
			Errors = new IInvalidProfileItemError[0];
		}
	}
}
