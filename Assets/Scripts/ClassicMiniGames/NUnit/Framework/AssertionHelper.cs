using NUnit.Framework.Constraints;
using System.Collections;

namespace NUnit.Framework
{
	public class AssertionHelper : ConstraintFactory
	{
		public void Expect(object actual, IResolveConstraint expression)
		{
			Assert.That(actual, expression, null, null);
		}

		public void Expect(object actual, IResolveConstraint expression, string message)
		{
			Assert.That(actual, expression, message, null);
		}

		public void Expect(object actual, IResolveConstraint expression, string message, params object[] args)
		{
			Assert.That(actual, expression, message, args);
		}

		public void Expect(bool condition, string message, params object[] args)
		{
			Assert.That(condition, Is.True, message, args);
		}

		public void Expect(bool condition, string message)
		{
			Assert.That(condition, Is.True, message, null);
		}

		public void Expect(bool condition)
		{
			Assert.That(condition, Is.True, null, null);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr)
		{
			Assert.That(del, expr.Resolve(), null, null);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr, string message)
		{
			Assert.That(del, expr.Resolve(), message, null);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr, string message, params object[] args)
		{
			Assert.That(del, expr, message, args);
		}

		public void Expect(TestDelegate code, IResolveConstraint constraint)
		{
			Assert.That((object)code, constraint);
		}

		public static void Expect<T>(T actual, IResolveConstraint expression)
		{
			Assert.That(actual, expression, null, null);
		}

		public static void Expect<T>(T actual, IResolveConstraint expression, string message)
		{
			Assert.That(actual, expression, message, null);
		}

		public static void Expect<T>(T actual, IResolveConstraint expression, string message, params object[] args)
		{
			Assert.That(actual, expression, message, args);
		}

		public static void Expect<T>(ref T actual, IResolveConstraint expression)
		{
			Assert.That(ref actual, expression, null, null);
		}

		public static void Expect<T>(ref T actual, IResolveConstraint expression, string message)
		{
			Assert.That(ref actual, expression, message, null);
		}

		public static void Expect<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
		{
			Assert.That(actual, expression, message, args);
		}

		public ListMapper Map(ICollection original)
		{
			return new ListMapper(original);
		}
	}
}
