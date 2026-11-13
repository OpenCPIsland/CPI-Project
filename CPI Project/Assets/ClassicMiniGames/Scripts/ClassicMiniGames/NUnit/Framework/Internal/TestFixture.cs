using NUnit.Framework.Api;
using System;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class TestFixture : TestSuite
	{
		public TestFixture(Type fixtureType)
			: this(fixtureType, null)
		{
		}

		public TestFixture(Type fixtureType, object[] arguments)
			: base(fixtureType, arguments)
		{
			oneTimeSetUpMethods = GetSetUpTearDownMethods(typeof(TestFixtureSetUpAttribute));
			oneTimeTearDownMethods = GetSetUpTearDownMethods(typeof(TestFixtureTearDownAttribute));
			setUpMethods = GetSetUpTearDownMethods(typeof(SetUpAttribute));
			tearDownMethods = GetSetUpTearDownMethods(typeof(TearDownAttribute));
		}

		private MethodInfo[] GetSetUpTearDownMethods(Type attrType)
		{
			MethodInfo[] methodsWithAttribute = Reflect.GetMethodsWithAttribute(base.FixtureType, attrType, true);
			MethodInfo[] array = methodsWithAttribute;
			foreach (MethodInfo methodInfo in array)
			{
				if (methodInfo.IsAbstract || (!methodInfo.IsPublic && !methodInfo.IsFamily) || methodInfo.GetParameters().Length > 0 || !methodInfo.ReturnType.Equals(typeof(void)))
				{
					base.Properties.Set(PropertyNames.SkipReason, string.Format("Invalid signature for SetUp or TearDown method: {0}", methodInfo.Name));
					base.RunState = RunState.NotRunnable;
					break;
				}
			}
			return methodsWithAttribute;
		}
	}
}
