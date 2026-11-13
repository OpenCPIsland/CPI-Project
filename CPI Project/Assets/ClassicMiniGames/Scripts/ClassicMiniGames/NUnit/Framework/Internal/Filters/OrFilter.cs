using NUnit.Framework.Api;
using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class OrFilter : TestFilter
	{
		private List<ITestFilter> filters = new List<ITestFilter>();

		public ITestFilter[] Filters
		{
			get
			{
				return filters.ToArray();
			}
		}

		public OrFilter()
		{
		}

		public OrFilter(params ITestFilter[] filters)
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
				if (filter.Pass(test))
				{
					return true;
				}
			}
			return false;
		}

		public override bool Match(ITest test)
		{
			foreach (TestFilter filter in filters)
			{
				if (filter.Match(test))
				{
					return true;
				}
			}
			return false;
		}
	}
}
