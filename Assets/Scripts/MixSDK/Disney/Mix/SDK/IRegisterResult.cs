using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IRegisterResult
	{
		bool Success
		{
			get;
		}

		ISession Session
		{
			get;
		}

		IEnumerable<IInvalidProfileItemError> Errors
		{
			get;
		}
	}
}
