using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NUnit.Framework.Internal
{
	public class TestSuite : Test
	{
		private List<ITest> tests = new List<ITest>();

		protected bool maintainTestOrder;

		protected MethodInfo[] oneTimeSetUpMethods;

		protected MethodInfo[] oneTimeTearDownMethods;

		public override IList<ITest> Tests
		{
			get
			{
				return tests;
			}
		}

		public override int TestCaseCount
		{
			get
			{
				int num = 0;
				foreach (Test test in Tests)
				{
					num += test.TestCaseCount;
				}
				return num;
			}
		}

		internal MethodInfo[] OneTimeSetUpMethods
		{
			get
			{
				return oneTimeSetUpMethods;
			}
		}

		internal MethodInfo[] OneTimeTearDownMethods
		{
			get
			{
				return oneTimeTearDownMethods;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return tests.Count > 0;
			}
		}

		public override string XmlElementName
		{
			get
			{
				return "test-suite";
			}
		}

		public TestSuite(string name)
			: base(name)
		{
		}

		public TestSuite(string parentSuiteName, string name)
			: base(parentSuiteName, name)
		{
		}

		public TestSuite(Type fixtureType)
			: this(fixtureType, null)
		{
		}

		public TestSuite(Type fixtureType, object[] arguments)
			: base(fixtureType)
		{
			string str = base.FullName = (base.Name = TypeHelper.GetDisplayName(fixtureType, arguments));
			string @namespace = fixtureType.Namespace;
			if (@namespace != null && @namespace != "")
			{
				base.FullName = @namespace + "." + str;
			}
			base.arguments = arguments;
		}

		public void Sort()
		{
			if (!maintainTestOrder)
			{
				tests.Sort();
				foreach (Test test in Tests)
				{
					TestSuite testSuite = test as TestSuite;
					if (testSuite != null)
					{
						testSuite.Sort();
					}
				}
			}
		}

		public void Add(Test test)
		{
			test.Parent = this;
			tests.Add(test);
		}

		public override TestResult MakeTestResult()
		{
			return new TestSuiteResult(this);
		}

		protected override TestCommand MakeTestCommand(ITestFilter filter)
		{
			TestCommand testCommand = new TestSuiteCommand(this);
			foreach (Test test in Tests)
			{
				if (filter.Pass(test))
				{
					testCommand.Children.Add(test.GetTestCommand(filter));
				}
			}
			return testCommand;
		}

		public override XmlNode AddToXml(XmlNode parentNode, bool recursive)
		{
			XmlNode xmlNode = XmlHelper.AddElement(parentNode, "test-suite");
			XmlHelper.AddAttribute(xmlNode, "type", TestType);
			PopulateTestNode(xmlNode, recursive);
			XmlHelper.AddAttribute(xmlNode, "testcasecount", TestCaseCount.ToString());
			if (recursive)
			{
				foreach (Test test in Tests)
				{
					test.AddToXml(xmlNode, recursive);
				}
			}
			return xmlNode;
		}
	}
}
