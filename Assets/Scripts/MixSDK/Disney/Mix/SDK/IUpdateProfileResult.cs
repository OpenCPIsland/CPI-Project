using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IUpdateProfileResult
	{
		bool Success
		{
			get;
		}

		IEnumerable<IInvalidProfileItemError> Errors
		{
			get;
		}
	}
}
