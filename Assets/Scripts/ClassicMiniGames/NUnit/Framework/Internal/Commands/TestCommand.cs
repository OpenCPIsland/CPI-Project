using System.Collections.Generic;

namespace NUnit.Framework.Internal.Commands
{
	public abstract class TestCommand
	{
		private Test test;

		private IList<TestCommand> children;

		public Test Test
		{
			get
			{
				return test;
			}
		}

		public IList<TestCommand> Children
		{
			get
			{
				if (children == null)
				{
					children = new List<TestCommand>();
				}
				return children;
			}
		}

		public TestCommand(Test test)
		{
			this.test = test;
		}

		public abstract TestResult Execute(TestExecutionContext context);
	}
}
