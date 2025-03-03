using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework.Extensibility
{
	public interface ISuiteBuilder
	{
		bool CanBuildFrom(Type type);

		Test BuildFrom(Type type);
	}
}
