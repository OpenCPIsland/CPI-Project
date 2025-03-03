using System.Diagnostics;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class Validate
	{
		[Conditional("UNITY_EDITOR")]
		public static void IsNotNull(Object context, object obj, string format = "Expected non-null value", params object[] args)
		{
			if (obj != null)
			{
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void IsNull(Object context, object obj, string format = "Expected null value", params object[] args)
		{
			if (obj == null)
			{
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void IsTrue(Object context, bool expression, string format = "Expected true value", params object[] args)
		{
			if (expression)
			{
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void IsFalse(Object context, bool expression, string format = "Expected false value", params object[] args)
		{
			if (!expression)
			{
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void AreEqual<T>(Object context, T expected, T expression, string format = "Expected {0} of type {1}, got {2} instead", params object[] args)
		{
			if (!expression.Equals(expected) && args.Length == 0)
			{
				args = new object[3]
				{
					expected,
					typeof(T),
					expression
				};
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void AreNoEqual<T>(Object context, T expected, T expression, string format = "Expected {0} of type {1}, got {2} instead", params object[] args)
		{
			if (expression.Equals(expected) && args.Length == 0)
			{
				args = new object[3]
				{
					expected,
					typeof(T),
					expression
				};
			}
		}

		[Conditional("UNITY_EDITOR")]
		private static void Report(Object context, string format, params object[] args)
		{
			string text = ".";
			string format2 = string.Format("Validation failure for {0} ({1}):\n{2}{3}", context.name, context.GetType(), format, text);
			UnityEngine.Debug.LogErrorFormat(context, format2, args);
		}
	}
}
