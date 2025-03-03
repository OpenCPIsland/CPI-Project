using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NUnit.Framework.Internal
{
	public class TestMethod : Test
	{
		internal MethodInfo method;

		private List<ICommandDecorator> decorators = new List<ICommandDecorator>();

		internal bool hasExpectedResult;

		internal object expectedResult;

		public MethodInfo Method
		{
			get
			{
				return method;
			}
		}

		public IList<ICommandDecorator> CustomDecorators
		{
			get
			{
				return decorators;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return false;
			}
		}

		public override IList<ITest> Tests
		{
			get
			{
				return new ITest[0];
			}
		}

		public override string XmlElementName
		{
			get
			{
				return "test-case";
			}
		}

		public TestMethod(MethodInfo method)
			: this(method, null)
		{
		}

		public TestMethod(MethodInfo method, Test parentSuite)
			: base(method.ReflectedType)
		{
			base.Name = method.Name;
			base.FullName = base.FullName + "." + base.Name;
			if (method.DeclaringType != method.ReflectedType)
			{
				base.Name = method.DeclaringType.Name + "." + method.Name;
			}
			string fullName = method.ReflectedType.FullName;
			if (parentSuite != null)
			{
				fullName = parentSuite.FullName;
				base.FullName = fullName + "." + base.Name;
			}
			this.method = method;
		}

		public override TestResult MakeTestResult()
		{
			return new TestCaseResult(this);
		}

		public override XmlNode AddToXml(XmlNode parentNode, bool recursive)
		{
			XmlNode xmlNode = XmlHelper.AddElement(parentNode, XmlElementName);
			PopulateTestNode(xmlNode, recursive);
			return xmlNode;
		}

		protected override TestCommand MakeTestCommand(ITestFilter filter)
		{
			TestCommand command = new TestMethodCommand(this);
			return ApplyDecoratorsToCommand(command);
		}

		private TestCommand ApplyDecoratorsToCommand(TestCommand command)
		{
			CommandDecoratorList commandDecoratorList = new CommandDecoratorList();
			commandDecoratorList.Add(new SetUpTearDownDecorator());
			foreach (ICommandDecorator customDecorator in CustomDecorators)
			{
				commandDecoratorList.Add(customDecorator);
			}
			commandDecoratorList.OrderByStage();
			foreach (ICommandDecorator item in commandDecoratorList)
			{
				command = item.Decorate(command);
			}
			return command;
		}
	}
}
