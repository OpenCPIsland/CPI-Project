using NUnit.Framework.Api;
using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class SimpleNameFilter : TestFilter
	{
		private List<string> names = new List<string>();

		public SimpleNameFilter()
		{
		}

		public SimpleNameFilter(string namesToAdd)
		{
			Add(namesToAdd);
		}

		public void Add(string name)
		{
			Guard.ArgumentNotNullOrEmpty(name, "name");
			names.Add(name);
		}

		public override bool Match(ITest test)
		{
			foreach (string name in names)
			{
				if (test.FullName == name)
				{
					return true;
				}
			}
			return false;
		}
	}
}
