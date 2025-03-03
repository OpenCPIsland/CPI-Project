using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Reflection;

namespace NUnit.Framework.Builders
{
	public class NUnitTestCaseBuilder : ITestCaseBuilder2, ITestCaseBuilder
	{
		private ITestCaseProvider testCaseProvider = new TestCaseProviders();

		public bool CanBuildFrom(MethodInfo method)
		{
			return method.IsDefined(typeof(TestAttribute), false) || method.IsDefined(typeof(ITestCaseSource), false);
		}

		public Test BuildFrom(MethodInfo method)
		{
			return BuildFrom(method, null);
		}

		public bool CanBuildFrom(MethodInfo method, Test parentSuite)
		{
			return CanBuildFrom(method);
		}

		public Test BuildFrom(MethodInfo method, Test parentSuite)
		{
			return testCaseProvider.HasTestCasesFor(method) ? BuildParameterizedMethodSuite(method, parentSuite) : BuildSingleTestMethod(method, parentSuite, null);
		}

		public Test BuildParameterizedMethodSuite(MethodInfo method, Test parentSuite)
		{
			ParameterizedMethodSuite parameterizedMethodSuite = new ParameterizedMethodSuite(method);
			parameterizedMethodSuite.ApplyCommonAttributes(method);
			foreach (ITestCaseData item in testCaseProvider.GetTestCasesFor(method))
			{
				ParameterSet parameterSet = item as ParameterSet;
				if (parameterSet == null)
				{
					parameterSet = new ParameterSet(item);
				}
				TestMethod test = BuildSingleTestMethod(method, parentSuite, parameterSet);
				parameterizedMethodSuite.Add(test);
			}
			return parameterizedMethodSuite;
		}

		public static TestMethod BuildSingleTestMethod(MethodInfo method, Test parentSuite, ParameterSet parms)
		{
			TestMethod testMethod = new TestMethod(method, parentSuite);
			string fullName = method.ReflectedType.FullName;
			if (parentSuite != null)
			{
				fullName = parentSuite.FullName;
			}
			if (CheckTestMethodSignature(testMethod, parms))
			{
				if (parms == null)
				{
					testMethod.ApplyCommonAttributes(method);
				}
				object[] customAttributes = method.GetCustomAttributes(typeof(ICommandDecorator), true);
				for (int i = 0; i < customAttributes.Length; i++)
				{
					ICommandDecorator item = (ICommandDecorator)customAttributes[i];
					testMethod.CustomDecorators.Add(item);
				}
				ExpectedExceptionAttribute[] array = (ExpectedExceptionAttribute[])method.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false);
				if (array.Length > 0)
				{
					ExpectedExceptionAttribute expectedExceptionAttribute = array[0];
					string handler = expectedExceptionAttribute.Handler;
					if (handler != null && GetExceptionHandler(testMethod.FixtureType, handler) == null)
					{
						MarkAsNotRunnable(testMethod, string.Format("The specified exception handler {0} was not found", handler));
					}
					testMethod.CustomDecorators.Add(new ExpectedExceptionDecorator(expectedExceptionAttribute.ExceptionData));
				}
			}
			if (parms != null)
			{
				method = testMethod.Method;
				if (parms.TestName != null)
				{
					testMethod.Name = parms.TestName;
					testMethod.FullName = fullName + "." + parms.TestName;
				}
				else if (parms.OriginalArguments != null)
				{
					string str = testMethod.Name = MethodHelper.GetDisplayName(method, parms.OriginalArguments);
					testMethod.FullName = fullName + "." + str;
				}
				parms.ApplyToTest(testMethod);
			}
			return testMethod;
		}

		private static bool CheckTestMethodSignature(TestMethod testMethod, ParameterSet parms)
		{
			if (testMethod.Method.IsAbstract)
			{
				return MarkAsNotRunnable(testMethod, "Method is abstract");
			}
			if (!testMethod.Method.IsPublic)
			{
				return MarkAsNotRunnable(testMethod, "Method is not public");
			}
			ParameterInfo[] parameters = testMethod.Method.GetParameters();
			int num = parameters.Length;
			object[] array = null;
			int num2 = 0;
			if (parms != null)
			{
				testMethod.arguments = parms.Arguments;
				testMethod.expectedResult = parms.ExpectedResult;
				testMethod.hasExpectedResult = parms.HasExpectedResult;
				testMethod.RunState = parms.RunState;
				array = parms.Arguments;
				if (array != null)
				{
					num2 = array.Length;
				}
				if (testMethod.RunState != RunState.Runnable)
				{
					return false;
				}
			}
			if (!testMethod.Method.ReturnType.Equals(typeof(void)) && (parms == null || (!parms.HasExpectedResult && !parms.ExceptionExpected)))
			{
				return MarkAsNotRunnable(testMethod, "Method has non-void return value");
			}
			if (num2 > 0 && num == 0)
			{
				return MarkAsNotRunnable(testMethod, "Arguments provided for method not taking any");
			}
			if (num2 == 0 && num > 0)
			{
				return MarkAsNotRunnable(testMethod, "No arguments were provided");
			}
			if (num2 != num)
			{
				return MarkAsNotRunnable(testMethod, "Wrong number of arguments provided");
			}
			if (testMethod.Method.IsGenericMethodDefinition)
			{
				Type[] typeArgumentsForMethod = GetTypeArgumentsForMethod(testMethod.Method, array);
				Type[] array2 = typeArgumentsForMethod;
				foreach (object obj in array2)
				{
					if (obj == null)
					{
						return MarkAsNotRunnable(testMethod, "Unable to determine type arguments for method");
					}
				}
				testMethod.method = testMethod.Method.MakeGenericMethod(typeArgumentsForMethod);
				parameters = testMethod.Method.GetParameters();
			}
			if (array != null && parameters != null)
			{
				TypeHelper.ConvertArgumentList(array, parameters);
			}
			return true;
		}

		private static Type[] GetTypeArgumentsForMethod(MethodInfo method, object[] arglist)
		{
			Type[] genericArguments = method.GetGenericArguments();
			Type[] array = new Type[genericArguments.Length];
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < array.Length; i++)
			{
				Type o = genericArguments[i];
				for (int j = 0; j < parameters.Length; j++)
				{
					if (parameters[j].ParameterType.Equals(o))
					{
						array[i] = TypeHelper.BestCommonType(array[i], arglist[j].GetType());
					}
				}
			}
			return array;
		}

		private static MethodInfo GetExceptionHandler(Type fixtureType, string name)
		{
			return fixtureType.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1]
			{
				typeof(Exception)
			}, null);
		}

		private static bool MarkAsNotRunnable(TestMethod testMethod, string reason)
		{
			testMethod.RunState = RunState.NotRunnable;
			testMethod.Properties.Set(PropertyNames.SkipReason, reason);
			return false;
		}
	}
}
