using NUnit.Framework.Api;
using System;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class NotFilter : TestFilter
	{
		private ITestFilter baseFilter;

		private bool topLevel = false;

		public bool TopLevel
		{
			get
			{
				return topLevel;
			}
			set
			{
				topLevel = value;
			}
		}

		public ITestFilter BaseFilter
		{
			get
			{
				return baseFilter;
			}
		}

		public NotFilter(ITestFilter baseFilter)
		{
			this.baseFilter = baseFilter;
		}

		public override bool Match(ITest test)
		{
			if (topLevel && test.RunState == RunState.Explicit)
			{
				return false;
			}
			return !baseFilter.Pass(test);
		}

		protected override bool MatchDescendant(ITest test)
		{
			if (!test.HasChildren || test.Tests == null || (topLevel && test.RunState == RunState.Explicit))
			{
				return false;
			}
			foreach (ITest test2 in test.Tests)
			{
				if (Match(test2) || MatchDescendant(test2))
				{
					return true;
				}
			}
			return false;
		}
	}
}
