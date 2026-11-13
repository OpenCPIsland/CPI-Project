using System;
using System.Collections.Generic;

namespace NUnit.Framework.Api
{
	public interface ITest : IXmlNodeBuilder
	{
		int Id
		{
			get;
			set;
		}

		string Name
		{
			get;
		}

		string FullName
		{
			get;
		}

		Type FixtureType
		{
			get;
		}

		RunState RunState
		{
			get;
			set;
		}

		int TestCaseCount
		{
			get;
		}

		IPropertyBag Properties
		{
			get;
		}

		ITest Parent
		{
			get;
		}

		bool HasChildren
		{
			get;
		}

		IList<ITest> Tests
		{
			get;
		}
	}
}
