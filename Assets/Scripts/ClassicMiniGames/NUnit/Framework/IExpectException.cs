using System;

namespace NUnit.Framework
{
	public interface IExpectException
	{
		void HandleException(Exception ex);
	}
}
