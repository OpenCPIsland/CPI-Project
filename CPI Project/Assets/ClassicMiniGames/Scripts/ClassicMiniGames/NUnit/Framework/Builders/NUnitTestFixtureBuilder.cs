using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Builders
{
	public class NUnitTestFixtureBuilder : ISuiteBuilder
	{
		private static readonly string NO_TYPE_ARGS_MSG = "Fixture type contains generic parameters. You must either provide Type arguments or specify constructor arguments that allow NUnit to deduce the Type arguments.";

		private TestFixture fixture;

		private ITestCaseBuilder2 testBuilder = new NUnitTestCaseBuilder();

		public bool CanBuildFrom(Type type)
		{
			if (type.IsAbstract && !type.IsSealed)
			{
				return false;
			}
			if (type.IsDefined(typeof(TestFixtureAttribute), true))
			{
				return true;
			}
			if (type.IsGenericTypeDefinition)
			{
				return false;
			}
			return Reflect.HasMethodWithAttribute(type, typeof(TestAttribute), true) || Reflect.HasMethodWithAttribute(type, typeof(TestCaseAttribute), true) || Reflect.HasMethodWithAttribute(type, typeof(TestCaseSourceAttribute), true);
		}

		public Test BuildFrom(Type type)
		{
			TestFixtureAttribute[] testFixtureAttributes = GetTestFixtureAttributes(type);
			if (type.IsGenericType)
			{
				return BuildMultipleFixtures(type, testFixtureAttributes);
			}
			switch (testFixtureAttributes.Length)
			{
			case 0:
				return BuildSingleFixture(type, null);
			case 1:
			{
				object[] arguments = testFixtureAttributes[0].Arguments;
				return (arguments == null || arguments.Length == 0) ? BuildSingleFixture(type, testFixtureAttributes[0]) : BuildMultipleFixtures(type, testFixtureAttributes);
			}
			default:
				return BuildMultipleFixtures(type, testFixtureAttributes);
			}
		}

		private Test BuildMultipleFixtures(Type type, TestFixtureAttribute[] attrs)
		{
			TestSuite testSuite = new ParameterizedFixtureSuite(type);
			if (attrs.Length > 0)
			{
				foreach (TestFixtureAttribute attr in attrs)
				{
					testSuite.Add(BuildSingleFixture(type, attr));
				}
			}
			else
			{
				testSuite.RunState = RunState.NotRunnable;
				testSuite.Properties.Set(PropertyNames.SkipReason, NO_TYPE_ARGS_MSG);
			}
			return testSuite;
		}

		private Test BuildSingleFixture(Type type, TestFixtureAttribute attr)
		{
			object[] array = null;
			if (attr != null)
			{
				array = attr.Arguments;
				if (type.ContainsGenericParameters)
				{
					Type[] typeArgsOut = attr.TypeArgs;
					if (typeArgsOut.Length > 0 || TypeHelper.CanDeduceTypeArgsFromArgs(type, array, ref typeArgsOut))
					{
						type = TypeHelper.MakeGenericType(type, typeArgsOut);
					}
				}
			}
			fixture = new TestFixture(type, array);
			CheckTestFixtureIsValid(fixture);
			fixture.ApplyCommonAttributes(type);
			if (fixture.RunState == RunState.Runnable && attr != null && attr.Ignore)
			{
				fixture.RunState = RunState.Ignored;
				fixture.Properties.Set(PropertyNames.SkipReason, attr.IgnoreReason);
			}
			AddTestCases(type);
			return fixture;
		}

		private void AddTestCases(Type fixtureType)
		{
			IList methods = fixtureType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo item in methods)
			{
				Test test = BuildTestCase(item, fixture);
				if (test != null)
				{
					fixture.Add(test);
				}
			}
		}

		private Test BuildTestCase(MethodInfo method, TestSuite suite)
		{
			return testBuilder.CanBuildFrom(method, suite) ? testBuilder.BuildFrom(method, suite) : null;
		}

		private void CheckTestFixtureIsValid(TestFixture fixture)
		{
			Type fixtureType = fixture.FixtureType;
			string reason = null;
			if (fixture.RunState == RunState.NotRunnable)
			{
				return;
			}
			if (!IsValidFixtureType(fixtureType, ref reason))
			{
				fixture.RunState = RunState.NotRunnable;
				fixture.Properties.Set(PropertyNames.SkipReason, reason);
			}
			else
			{
				if (IsStaticClass(fixtureType))
				{
					return;
				}
				object[] arguments = fixture.arguments;
				Type[] array;
				if (arguments == null)
				{
					array = new Type[0];
				}
				else
				{
					array = new Type[arguments.Length];
					int num = 0;
					object[] array2 = arguments;
					foreach (object obj in array2)
					{
						array[num++] = obj.GetType();
					}
				}
				ConstructorInfo constructor = fixtureType.GetConstructor(array);
				if (constructor == null)
				{
					fixture.RunState = RunState.NotRunnable;
					fixture.Properties.Set(PropertyNames.SkipReason, "No suitable constructor was found");
				}
			}
		}

		private static bool IsStaticClass(Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		private bool IsValidFixtureType(Type fixtureType, ref string reason)
		{
			if (fixtureType.ContainsGenericParameters)
			{
				reason = NO_TYPE_ARGS_MSG;
				return false;
			}
			return true;
		}

		private TestFixtureAttribute[] GetTestFixtureAttributes(Type type)
		{
			TestFixtureAttribute[] array = (TestFixtureAttribute[])type.GetCustomAttributes(typeof(TestFixtureAttribute), true);
			if (array.Length <= 1)
			{
				return array;
			}
			int num = 0;
			bool[] array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				TestFixtureAttribute testFixtureAttribute = array[i];
				if (testFixtureAttribute.Arguments.Length > 0 || testFixtureAttribute.TypeArgs.Length > 0)
				{
					num++;
					array2[i] = true;
				}
			}
			if (num == array.Length)
			{
				return array;
			}
			if (num == 0)
			{
				return new TestFixtureAttribute[1]
				{
					array[0]
				};
			}
			int num2 = 0;
			TestFixtureAttribute[] array3 = new TestFixtureAttribute[num];
			for (int i = 0; i < array.Length; i++)
			{
				if (array2[i])
				{
					array3[num2++] = array[i];
				}
			}
			return array3;
		}
	}
}
