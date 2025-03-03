using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NUnit.Framework.Internal
{
	public abstract class Test : ITest, IXmlNodeBuilder, IComparable
	{
		private static int nextID = 1000;

		private int id;

		private string name;

		private string fullName;

		private RunState runState;

		private ITest parent;

		private PropertyBag properties;

		private Type fixtureType;

		private object fixture;

		protected MethodInfo[] setUpMethods;

		protected MethodInfo[] tearDownMethods;

		internal object[] arguments;

		private TestCommand testCommand;

		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string FullName
		{
			get
			{
				return fullName;
			}
			set
			{
				fullName = value;
			}
		}

		public Type FixtureType
		{
			get
			{
				return fixtureType;
			}
		}

		public RunState RunState
		{
			get
			{
				return runState;
			}
			set
			{
				runState = value;
			}
		}

		public abstract string XmlElementName
		{
			get;
		}

		public virtual string TestType
		{
			get
			{
				return GetType().Name;
			}
		}

		public virtual int TestCaseCount
		{
			get
			{
				return 1;
			}
		}

		public IPropertyBag Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new PropertyBag();
				}
				return properties;
			}
		}

		public abstract bool HasChildren
		{
			get;
		}

		public ITest Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		public abstract IList<ITest> Tests
		{
			get;
		}

		internal object Fixture
		{
			get
			{
				return fixture;
			}
			set
			{
				fixture = value;
			}
		}

		internal virtual MethodInfo[] SetUpMethods
		{
			get
			{
				if (setUpMethods == null && Parent != null)
				{
					TestSuite testSuite = Parent as TestSuite;
					if (testSuite != null)
					{
						setUpMethods = testSuite.SetUpMethods;
					}
				}
				return setUpMethods;
			}
		}

		internal virtual MethodInfo[] TearDownMethods
		{
			get
			{
				if (tearDownMethods == null && Parent != null)
				{
					TestSuite testSuite = Parent as TestSuite;
					if (testSuite != null)
					{
						tearDownMethods = testSuite.TearDownMethods;
					}
				}
				return tearDownMethods;
			}
		}

		protected Test(string name)
		{
			fullName = name;
			this.name = name;
			id = nextID++;
			runState = RunState.Runnable;
		}

		protected Test(string pathName, string name)
		{
			fullName = ((pathName == null || pathName == string.Empty) ? name : (pathName + "." + name));
			this.name = name;
			id = nextID++;
			runState = RunState.Runnable;
		}

		protected Test(Type fixtureType)
			: this(fixtureType.FullName)
		{
			this.fixtureType = fixtureType;
		}

		public XmlNode ToXml(bool recursive)
		{
			XmlNode parentNode = XmlHelper.CreateTopLevelElement("dummy");
			return AddToXml(parentNode, recursive);
		}

		public abstract XmlNode AddToXml(XmlNode parentNode, bool recursive);

		public int CompareTo(object obj)
		{
			Test test = obj as Test;
			if (test == null)
			{
				return -1;
			}
			return FullName.CompareTo(test.FullName);
		}

		public abstract TestResult MakeTestResult();

		public TestCommand GetTestCommand(ITestFilter filter)
		{
			if (testCommand == null)
			{
				testCommand = ((runState != RunState.Runnable && runState != RunState.Explicit) ? new SkipCommand(this) : MakeTestCommand(filter));
			}
			return testCommand;
		}

		public void ApplyCommonAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(typeof(NUnitAttribute), true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Attribute attribute = (Attribute)customAttributes[i];
				IApplyToTest applyToTest = attribute as IApplyToTest;
				if (applyToTest != null)
				{
					applyToTest.ApplyToTest(this);
				}
			}
		}

		protected abstract TestCommand MakeTestCommand(ITestFilter filter);

		protected void PopulateTestNode(XmlNode thisNode, bool recursive)
		{
			XmlHelper.AddAttribute(thisNode, "id", Id.ToString());
			XmlHelper.AddAttribute(thisNode, "name", Name);
			XmlHelper.AddAttribute(thisNode, "fullname", FullName);
			if (Properties.Count > 0)
			{
				Properties.AddToXml(thisNode, recursive);
			}
		}
	}
}
