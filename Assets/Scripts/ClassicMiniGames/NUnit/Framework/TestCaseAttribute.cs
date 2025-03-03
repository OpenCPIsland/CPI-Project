using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseAttribute : DataAttribute, ITestCaseData, ITestCaseSource
	{
		private object[] arguments;

		private ExpectedExceptionData exceptionData;

		private object expectedResult;

		private bool hasExpectedResult;

		private IPropertyBag properties;

		private RunState runState;

		private string testName;

		public object[] Arguments
		{
			get
			{
				return arguments;
			}
		}

		public object ExpectedResult
		{
			get
			{
				return expectedResult;
			}
			set
			{
				expectedResult = value;
				hasExpectedResult = true;
			}
		}

		public bool HasExpectedResult
		{
			get
			{
				return hasExpectedResult;
			}
		}

		public ExpectedExceptionData ExceptionData
		{
			get
			{
				return exceptionData;
			}
		}

		public Type ExpectedException
		{
			get
			{
				return exceptionData.ExpectedExceptionType;
			}
			set
			{
				exceptionData.ExpectedExceptionType = value;
			}
		}

		public string ExpectedExceptionName
		{
			get
			{
				return exceptionData.ExpectedExceptionName;
			}
			set
			{
				exceptionData.ExpectedExceptionName = value;
			}
		}

		public string ExpectedMessage
		{
			get
			{
				return exceptionData.ExpectedMessage;
			}
			set
			{
				exceptionData.ExpectedMessage = value;
			}
		}

		public MessageMatch MatchType
		{
			get
			{
				return exceptionData.MatchType;
			}
			set
			{
				exceptionData.MatchType = value;
			}
		}

		public string Description
		{
			get
			{
				return Properties.Get(PropertyNames.Description) as string;
			}
			set
			{
				Properties.Set(PropertyNames.Description, value);
			}
		}

		public string TestName
		{
			get
			{
				return testName;
			}
			set
			{
				testName = value;
			}
		}

		public bool Ignore
		{
			get
			{
				return RunState == RunState.Ignored;
			}
			set
			{
				runState = ((!value) ? RunState.Runnable : RunState.Ignored);
			}
		}

		public bool Explicit
		{
			get
			{
				return RunState == RunState.Explicit;
			}
			set
			{
				runState = ((!value) ? RunState.Runnable : RunState.Explicit);
			}
		}

		public RunState RunState
		{
			get
			{
				return runState;
			}
		}

		public string Reason
		{
			get
			{
				return Properties.Get(PropertyNames.SkipReason) as string;
			}
			set
			{
				Properties.Set(PropertyNames.SkipReason, value);
			}
		}

		public string IgnoreReason
		{
			get
			{
				return Reason;
			}
			set
			{
				runState = RunState.Ignored;
				Reason = value;
			}
		}

		public string Category
		{
			get
			{
				return Properties.Get(PropertyNames.Category) as string;
			}
			set
			{
				string[] array = value.Split(',');
				foreach (string value2 in array)
				{
					Properties.Add(PropertyNames.Category, value2);
				}
			}
		}

		public IList Categories
		{
			get
			{
				return Properties[PropertyNames.Category];
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

		public TestCaseAttribute(params object[] arguments)
		{
			runState = RunState.Runnable;
			if (arguments == null)
			{
				object[] array = this.arguments = new object[1];
			}
			else
			{
				this.arguments = arguments;
			}
		}

		public TestCaseAttribute(object arg)
		{
			runState = RunState.Runnable;
			arguments = new object[1]
			{
				arg
			};
		}

		public TestCaseAttribute(object arg1, object arg2)
		{
			runState = RunState.Runnable;
			arguments = new object[2]
			{
				arg1,
				arg2
			};
		}

		public TestCaseAttribute(object arg1, object arg2, object arg3)
		{
			runState = RunState.Runnable;
			arguments = new object[3]
			{
				arg1,
				arg2,
				arg3
			};
		}

		public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
		{
			ParameterSet parameterSet;
			try
			{
				ParameterInfo[] parameters = method.GetParameters();
				int num = parameters.Length;
				int num2 = Arguments.Length;
				parameterSet = new ParameterSet(this);
				if (num > 0 && num2 >= num - 1)
				{
					ParameterInfo parameterInfo = parameters[num - 1];
					Type parameterType = parameterInfo.ParameterType;
					Type elementType = parameterType.GetElementType();
					if (parameterType.IsArray && parameterInfo.IsDefined(typeof(ParamArrayAttribute), false))
					{
						if (num2 == num)
						{
							Type type = parameterSet.Arguments[num2 - 1].GetType();
							if (!parameterType.IsAssignableFrom(type))
							{
								Array array = Array.CreateInstance(elementType, 1);
								array.SetValue(parameterSet.Arguments[num2 - 1], 0);
								parameterSet.Arguments[num2 - 1] = array;
							}
						}
						else
						{
							object[] array2 = new object[num];
							for (int i = 0; i < num && i < num2; i++)
							{
								array2[i] = parameterSet.Arguments[i];
							}
							int num3 = num2 - num + 1;
							Array array = Array.CreateInstance(elementType, num3);
							for (int i = 0; i < num3; i++)
							{
								array.SetValue(parameterSet.Arguments[num + i - 1], i);
							}
							array2[num - 1] = array;
							parameterSet.Arguments = array2;
							num2 = num;
						}
					}
				}
				if (num == 1 && method.GetParameters()[0].ParameterType == typeof(object[]) && (num2 > 1 || (num2 == 1 && parameterSet.Arguments[0].GetType() != typeof(object[]))))
				{
					parameterSet.Arguments = new object[1]
					{
						parameterSet.Arguments
					};
				}
				if (num2 == num)
				{
					PerformSpecialConversions(parameterSet.Arguments, parameters);
				}
			}
			catch (Exception exception)
			{
				parameterSet = new ParameterSet(exception);
			}
			return new ITestCaseData[1]
			{
				parameterSet
			};
		}

		private static void PerformSpecialConversions(object[] arglist, ParameterInfo[] parameters)
		{
			for (int i = 0; i < arglist.Length; i++)
			{
				object obj = arglist[i];
				Type parameterType = parameters[i].ParameterType;
				if (obj == null)
				{
					continue;
				}
				if (obj is SpecialValue && (SpecialValue)obj == SpecialValue.Null)
				{
					arglist[i] = null;
				}
				else
				{
					if (parameterType.IsAssignableFrom(obj.GetType()))
					{
						continue;
					}
					if (obj is DBNull)
					{
						arglist[i] = null;
						continue;
					}
					bool flag = false;
					if (parameterType == typeof(short) || parameterType == typeof(byte) || parameterType == typeof(sbyte))
					{
						flag = (obj is int);
					}
					else if (parameterType == typeof(decimal))
					{
						flag = (obj is double || obj is string || obj is int);
					}
					else if (parameterType == typeof(DateTime) || parameterType == typeof(TimeSpan))
					{
						flag = (obj is string);
					}
					if (flag)
					{
						arglist[i] = Convert.ChangeType(obj, parameterType, CultureInfo.InvariantCulture);
					}
				}
			}
		}
	}
}
