using System;
using UnityEngine;

namespace UnityTest
{
	public interface ITestComponent : IComparable<ITestComponent>
	{
		GameObject gameObject
		{
			get;
		}

		string Name
		{
			get;
		}

		void EnableTest(bool enable);

		bool IsTestGroup();

		ITestComponent GetTestGroup();

		bool IsExceptionExpected(string exceptionType);

		bool ShouldSucceedOnException();

		double GetTimeout();

		bool IsIgnored();

		bool ShouldSucceedOnAssertions();

		bool IsExludedOnThisPlatform();
	}
}
