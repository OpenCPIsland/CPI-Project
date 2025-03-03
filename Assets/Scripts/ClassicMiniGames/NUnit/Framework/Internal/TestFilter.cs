using NUnit.Framework.Api;
using NUnit.Framework.Internal.Filters;
using System;
using System.Xml;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public abstract class TestFilter : ITestFilter
	{
		[Serializable]
		private class EmptyFilter : TestFilter
		{
			public override bool Match(ITest test)
			{
				return test.RunState != RunState.Explicit;
			}

			public override bool Pass(ITest test)
			{
				return test.RunState != RunState.Explicit;
			}
		}

		public static TestFilter Empty = new EmptyFilter();

		public virtual bool Pass(ITest test)
		{
			return Match(test) || MatchParent(test) || MatchDescendant(test);
		}

		public abstract bool Match(ITest test);

		protected virtual bool MatchParent(ITest test)
		{
			return test.RunState != RunState.Explicit && test.Parent != null && (Match(test.Parent) || MatchParent(test.Parent));
		}

		protected virtual bool MatchDescendant(ITest test)
		{
			if (test.Tests == null)
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

		public static TestFilter FromXml(string xmlText)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlText);
			XmlNode firstChild = xmlDocument.FirstChild;
			if (firstChild.Name != "filter")
			{
				throw new Exception("Expected filter element at top level");
			}
			TestFilter testFilter = Empty;
			bool flag = true;
			XmlNodeList xmlNodeList = firstChild.SelectNodes("tests/test");
			XmlNodeList xmlNodeList2 = firstChild.SelectNodes("include/category");
			XmlNodeList xmlNodeList3 = firstChild.SelectNodes("exclude/category");
			if (xmlNodeList.Count > 0)
			{
				SimpleNameFilter simpleNameFilter = new SimpleNameFilter();
				foreach (XmlNode item in firstChild.SelectNodes("tests/test"))
				{
					simpleNameFilter.Add(item.InnerText);
				}
				testFilter = simpleNameFilter;
				flag = false;
			}
			if (xmlNodeList2.Count > 0)
			{
				XmlNode xmlNode2 = xmlNodeList2[0];
				TestFilter filter = new CategoryExpression(xmlNode2.InnerText).Filter;
				testFilter = ((!flag) ? new AndFilter(testFilter, filter) : filter);
				flag = false;
			}
			if (xmlNodeList3.Count > 0)
			{
				CategoryFilter categoryFilter = new CategoryFilter();
				foreach (XmlNode item2 in xmlNodeList3)
				{
					categoryFilter.AddCategory(item2.InnerText);
				}
				TestFilter testFilter2 = new NotFilter(categoryFilter);
				testFilter = ((!flag) ? new AndFilter(testFilter, testFilter2) : testFilter2);
				flag = false;
			}
			return testFilter;
		}
	}
}
