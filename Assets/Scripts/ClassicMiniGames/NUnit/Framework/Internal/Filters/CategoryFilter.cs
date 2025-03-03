using NUnit.Framework.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
	[Serializable]
	public class CategoryFilter : TestFilter
	{
		private List<string> categories = new List<string>();

		public CategoryFilter()
		{
		}

		public CategoryFilter(string name)
		{
			if (name != null && name != string.Empty)
			{
				categories.Add(name);
			}
		}

		public CategoryFilter(string[] names)
		{
			if (names != null)
			{
				categories.AddRange(names);
			}
		}

		public void AddCategory(string name)
		{
			categories.Add(name);
		}

		public override bool Match(ITest test)
		{
			IList list = test.Properties[PropertyNames.Category];
			if (list == null || list.Count == 0)
			{
				return false;
			}
			foreach (string category in categories)
			{
				if (list.Contains(category))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < categories.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(categories[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
