using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class TestContext
	{
		public class TestAdapter
		{
			private Test test;

			public int ID
			{
				get
				{
					return test.Id;
				}
			}

			public string Name
			{
				get
				{
					return test.Name;
				}
			}

			public string MethodName
			{
				get
				{
					return (test is TestMethod) ? ((TestMethod)test).Method.Name : null;
				}
			}

			public string FullName
			{
				get
				{
					return test.FullName;
				}
			}

			public IPropertyBag Properties
			{
				get
				{
					return test.Properties;
				}
			}

			public TestAdapter(Test test)
			{
				this.test = test;
			}
		}

		public class ResultAdapter
		{
			private TestResult result;

			public ResultState Outcome
			{
				get
				{
					return result.ResultState;
				}
			}

			public ResultAdapter(TestResult result)
			{
				this.result = result;
			}
		}

		private TestExecutionContext ec;

		private TestAdapter test;

		private ResultAdapter result;

		public static TestContext CurrentContext
		{
			get
			{
				return new TestContext(TestExecutionContext.CurrentContext);
			}
		}

		public TestAdapter Test
		{
			get
			{
				if (test == null)
				{
					test = new TestAdapter(ec.CurrentTest);
				}
				return test;
			}
		}

		public ResultAdapter Result
		{
			get
			{
				if (result == null)
				{
					result = new ResultAdapter(ec.CurrentResult);
				}
				return result;
			}
		}

		public string TestDirectory
		{
			get
			{
				return AssemblyHelper.GetAssemblyPath(ec.CurrentTest.FixtureType.Assembly);
			}
		}

		public string WorkDirectory
		{
			get
			{
				return ec.WorkDirectory;
			}
		}

		public TestContext(TestExecutionContext ec)
		{
			this.ec = ec;
		}
	}
}
