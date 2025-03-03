using NUnit.Framework.Api;
using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class AndFilter : TestFilter
	{
		private List<ITestFilter> filters = new List<ITestFilter>();

		public AndFilter()
		{
		}

		public AndFilter(params ITestFilter[] filters)
		{
			this.filters.AddRange(filters);
		}

		public void Add(ITestFilter filter)
		{
			filters.Add(filter);
		}

		public override bool Pass(ITest test)
		{
			foreach (ITestFilter filter in filters)
			{
				if (!filter.Pass(test))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Match(ITest test)
		{
			foreach (TestFilter filter in filters)
			{
				if (!filter.Match(test))
				{
					return false;
				}
			}
			return true;
		}
	}
}
