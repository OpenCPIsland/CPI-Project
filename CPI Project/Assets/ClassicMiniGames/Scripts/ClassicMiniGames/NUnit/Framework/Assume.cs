using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System.ComponentModel;

namespace NUnit.Framework
{
	public class Assume
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new AssertionException("Assert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
		}

		public static void That(object actual, IResolveConstraint expression)
		{
			That(actual, expression, null, null);
		}

		public static void That(object actual, IResolveConstraint expression, string message)
		{
			That(actual, expression, message, null);
		}

		public static void That(object actual, IResolveConstraint expression, string message, params object[] args)
		{
			Constraint constraint = expression.Resolve();
			if (!constraint.Matches(actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}

		public static void That(ActualValueDelegate del, IResolveConstraint expr)
		{
			That(del, expr.Resolve(), null, null);
		}

		public static void That(ActualValueDelegate del, IResolveConstraint expr, string message)
		{
			That(del, expr.Resolve(), message, null);
		}

		public static void That(ActualValueDelegate del, IResolveConstraint expr, string message, params object[] args)
		{
			Constraint constraint = expr.Resolve();
			if (!constraint.Matches(del))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}

		public static void That(ref bool actual, IResolveConstraint expression)
		{
			That(ref actual, expression.Resolve(), null, null);
		}

		public static void That(ref bool actual, IResolveConstraint expression, string message)
		{
			That(ref actual, expression.Resolve(), message, null);
		}

		public static void That(ref bool actual, IResolveConstraint expression, string message, params object[] args)
		{
			Constraint constraint = expression.Resolve();
			if (!constraint.Matches(ref actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}

		public static void That(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void That(bool condition, string message)
		{
			That(condition, Is.True, message, null);
		}

		public static void That(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void That(TestDelegate code, IResolveConstraint constraint)
		{
			That((object)code, constraint);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression)
		{
			That(ref actual, expression.Resolve(), null, null);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression, string message)
		{
			That(ref actual, expression.Resolve(), message, null);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
		{
			Constraint constraint = expression.Resolve();
			if (!constraint.Matches(ref actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new InconclusiveException(messageWriter.ToString());
			}
		}
	}
}
