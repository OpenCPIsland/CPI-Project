using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest.IntegrationTestRunner
{
	internal class IntegrationTestsProvider
	{
		internal Dictionary<ITestComponent, HashSet<ITestComponent>> testCollection = new Dictionary<ITestComponent, HashSet<ITestComponent>>();

		internal ITestComponent currentTestGroup;

		internal IEnumerable<ITestComponent> testToRun;

		public IntegrationTestsProvider(IEnumerable<ITestComponent> tests)
		{
			testToRun = tests;
			foreach (ITestComponent item in tests.OrderBy((ITestComponent component) => component))
			{
				if (item.IsTestGroup())
				{
					throw new Exception(item.Name + " is test a group");
				}
				AddTestToList(item);
			}
			if (currentTestGroup == null)
			{
				currentTestGroup = FindInnerTestGroup(TestComponent.NullTestComponent);
			}
		}

		private void AddTestToList(ITestComponent test)
		{
			ITestComponent testGroup = test.GetTestGroup();
			if (!testCollection.ContainsKey(testGroup))
			{
				testCollection.Add(testGroup, new HashSet<ITestComponent>());
			}
			testCollection[testGroup].Add(test);
			if (testGroup != TestComponent.NullTestComponent)
			{
				AddTestToList(testGroup);
			}
		}

		public ITestComponent GetNextTest()
		{
			ITestComponent testComponent = testCollection[currentTestGroup].First();
			testCollection[currentTestGroup].Remove(testComponent);
			testComponent.EnableTest(true);
			return testComponent;
		}

		public void FinishTest(ITestComponent test)
		{
			try
			{
				test.EnableTest(false);
				currentTestGroup = FindNextTestGroup(currentTestGroup);
			}
			catch (MissingReferenceException exception)
			{
				Debug.LogException(exception);
			}
		}

		private ITestComponent FindNextTestGroup(ITestComponent testGroup)
		{
			if (testGroup == null)
			{
				throw new Exception("No test left");
			}
			if (testCollection[testGroup].Any())
			{
				testGroup.EnableTest(true);
				return FindInnerTestGroup(testGroup);
			}
			testCollection.Remove(testGroup);
			testGroup.EnableTest(false);
			ITestComponent testGroup2 = testGroup.GetTestGroup();
			if (testGroup2 == null)
			{
				return null;
			}
			testCollection[testGroup2].Remove(testGroup);
			return FindNextTestGroup(testGroup2);
		}

		private ITestComponent FindInnerTestGroup(ITestComponent group)
		{
			HashSet<ITestComponent> hashSet = testCollection[group];
			foreach (ITestComponent item in hashSet)
			{
				if (item.IsTestGroup())
				{
					item.EnableTest(true);
					return FindInnerTestGroup(item);
				}
			}
			return group;
		}

		public bool AnyTestsLeft()
		{
			return testCollection.Count != 0;
		}

		public List<ITestComponent> GetRemainingTests()
		{
			List<ITestComponent> list = new List<ITestComponent>();
			foreach (KeyValuePair<ITestComponent, HashSet<ITestComponent>> item in testCollection)
			{
				list.AddRange(item.Value);
			}
			return list;
		}
	}
}
