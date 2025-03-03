using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System;
using System.ComponentModel;

namespace NUnit.Framework
{
	public class Assert
	{
		protected Assert()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
		}

		public static void Pass(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new SuccessException(message);
		}

		public static void Pass(string message)
		{
			Pass(message, null);
		}

		public static void Pass()
		{
			Pass(string.Empty, null);
		}

		public static void Fail(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new AssertionException(message);
		}

		public static void Fail(string message)
		{
			Fail(message, null);
		}

		public static void Fail()
		{
			Fail(string.Empty, null);
		}

		public static void Ignore(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new IgnoreException(message);
		}

		public static void Ignore(string message)
		{
			Ignore(message, null);
		}

		public static void Ignore()
		{
			Ignore(string.Empty, null);
		}

		public static void Inconclusive(string message, params object[] args)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			else if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			throw new InconclusiveException(message);
		}

		public static void Inconclusive(string message)
		{
			Inconclusive(message, null);
		}

		public static void Inconclusive()
		{
			Inconclusive(string.Empty, null);
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
			TestExecutionContext.CurrentContext.IncrementAssertCount();
			if (!constraint.Matches(actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new AssertionException(messageWriter.ToString());
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

		public static void That(ref bool actual, IResolveConstraint constraint)
		{
			That(ref actual, constraint.Resolve(), null, null);
		}

		public static void That(ref bool actual, IResolveConstraint constraint, string message)
		{
			That(ref actual, constraint.Resolve(), message, null);
		}

		public static void That(ref bool actual, IResolveConstraint expression, string message, params object[] args)
		{
			Constraint constraint = expression.Resolve();
			TestExecutionContext.CurrentContext.IncrementAssertCount();
			if (!constraint.Matches(ref actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new AssertionException(messageWriter.ToString());
			}
		}

		public static void That(TestDelegate code, IResolveConstraint constraint)
		{
			That((object)code, constraint);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression)
		{
			That(ref actual, expression, null, null);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression, string message)
		{
			That(ref actual, expression, message, null);
		}

		public static void That<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
		{
			Constraint constraint = expression.Resolve();
			TestExecutionContext.CurrentContext.IncrementAssertCount();
			if (!constraint.Matches(ref actual))
			{
				MessageWriter messageWriter = new TextMessageWriter(message, args);
				constraint.WriteMessageTo(messageWriter);
				throw new AssertionException(messageWriter.ToString());
			}
		}

		public static void ByVal(object actual, IResolveConstraint expression)
		{
			That(actual, expression, null, null);
		}

		public static void ByVal(object actual, IResolveConstraint expression, string message)
		{
			That(actual, expression, message, null);
		}

		public static void ByVal(object actual, IResolveConstraint expression, string message, params object[] args)
		{
			That(actual, expression, message, args);
		}

		public static Exception Throws(IResolveConstraint expression, TestDelegate code, string message, params object[] args)
		{
			Exception ex = null;
			try
			{
				code();
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			That(ex, expression, message, args);
			return ex;
		}

		public static Exception Throws(IResolveConstraint expression, TestDelegate code, string message)
		{
			return Throws(expression, code, message, null);
		}

		public static Exception Throws(IResolveConstraint expression, TestDelegate code)
		{
			return Throws(expression, code, string.Empty, null);
		}

		public static Exception Throws(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
		{
			return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, message, args);
		}

		public static Exception Throws(Type expectedExceptionType, TestDelegate code, string message)
		{
			return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, message, null);
		}

		public static Exception Throws(Type expectedExceptionType, TestDelegate code)
		{
			return Throws(new ExceptionTypeConstraint(expectedExceptionType), code, string.Empty, null);
		}

		public static T Throws<T>(TestDelegate code, string message, params object[] args) where T : Exception
		{
			return (T)Throws(typeof(T), code, message, args);
		}

		public static T Throws<T>(TestDelegate code, string message) where T : Exception
		{
			return Throws<T>(code, message, null);
		}

		public static T Throws<T>(TestDelegate code) where T : Exception
		{
			return Throws<T>(code, string.Empty, null);
		}

		public static Exception Catch(TestDelegate code, string message, params object[] args)
		{
			return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message, args);
		}

		public static Exception Catch(TestDelegate code, string message)
		{
			return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code, message);
		}

		public static Exception Catch(TestDelegate code)
		{
			return Throws(new InstanceOfTypeConstraint(typeof(Exception)), code);
		}

		public static Exception Catch(Type expectedExceptionType, TestDelegate code, string message, params object[] args)
		{
			return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message, args);
		}

		public static Exception Catch(Type expectedExceptionType, TestDelegate code, string message)
		{
			return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code, message);
		}

		public static Exception Catch(Type expectedExceptionType, TestDelegate code)
		{
			return Throws(new InstanceOfTypeConstraint(expectedExceptionType), code);
		}

		public static T Catch<T>(TestDelegate code, string message, params object[] args) where T : Exception
		{
			return (T)Throws(new InstanceOfTypeConstraint(typeof(T)), code, message, args);
		}

		public static T Catch<T>(TestDelegate code, string message) where T : Exception
		{
			return (T)Throws(new InstanceOfTypeConstraint(typeof(T)), code, message);
		}

		public static T Catch<T>(TestDelegate code) where T : Exception
		{
			return (T)Throws(new InstanceOfTypeConstraint(typeof(T)), code);
		}

		public static void DoesNotThrow(TestDelegate code, string message, params object[] args)
		{
			try
			{
				code();
			}
			catch (Exception ex)
			{
				TextMessageWriter textMessageWriter = new TextMessageWriter(message, args);
				textMessageWriter.WriteLine("Unexpected exception: {0}", ex.GetType());
				Fail(textMessageWriter.ToString());
			}
		}

		public static void DoesNotThrow(TestDelegate code, string message)
		{
			DoesNotThrow(code, message, null);
		}

		public static void DoesNotThrow(TestDelegate code)
		{
			DoesNotThrow(code, string.Empty, null);
		}

		public static void True(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void True(bool condition, string message)
		{
			That(condition, Is.True, message, null);
		}

		public static void True(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void IsTrue(bool condition, string message, params object[] args)
		{
			That(condition, Is.True, message, args);
		}

		public static void IsTrue(bool condition, string message)
		{
			That(condition, Is.True, message, null);
		}

		public static void IsTrue(bool condition)
		{
			That(condition, Is.True, null, null);
		}

		public static void False(bool condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void False(bool condition, string message)
		{
			That(condition, Is.False, message, null);
		}

		public static void False(bool condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void IsFalse(bool condition, string message, params object[] args)
		{
			That(condition, Is.False, message, args);
		}

		public static void IsFalse(bool condition, string message)
		{
			That(condition, Is.False, message, null);
		}

		public static void IsFalse(bool condition)
		{
			That(condition, Is.False, null, null);
		}

		public static void NotNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Not.Null, message, args);
		}

		public static void NotNull(object anObject, string message)
		{
			That(anObject, Is.Not.Null, message, null);
		}

		public static void NotNull(object anObject)
		{
			That(anObject, Is.Not.Null, null, null);
		}

		public static void IsNotNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Not.Null, message, args);
		}

		public static void IsNotNull(object anObject, string message)
		{
			That(anObject, Is.Not.Null, message, null);
		}

		public static void IsNotNull(object anObject)
		{
			That(anObject, Is.Not.Null, null, null);
		}

		public static void Null(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Null, message, args);
		}

		public static void Null(object anObject, string message)
		{
			That(anObject, Is.Null, message, null);
		}

		public static void Null(object anObject)
		{
			That(anObject, Is.Null, null, null);
		}

		public static void IsNull(object anObject, string message, params object[] args)
		{
			That(anObject, Is.Null, message, args);
		}

		public static void IsNull(object anObject, string message)
		{
			That(anObject, Is.Null, message, null);
		}

		public static void IsNull(object anObject)
		{
			That(anObject, Is.Null, null, null);
		}

		public static void AreEqual(int expected, int actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(int expected, int actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(int expected, int actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreEqual(long expected, long actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(long expected, long actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(long expected, long actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreEqual(uint expected, uint actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(uint expected, uint actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(uint expected, uint actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreEqual(ulong expected, ulong actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(ulong expected, ulong actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(ulong expected, ulong actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreEqual(decimal expected, decimal actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(decimal expected, decimal actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(decimal expected, decimal actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreEqual(double expected, double actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, actual, delta, message, args);
		}

		public static void AreEqual(double expected, double actual, double delta, string message)
		{
			AssertDoublesAreEqual(expected, actual, delta, message, null);
		}

		public static void AreEqual(double expected, double actual, double delta)
		{
			AssertDoublesAreEqual(expected, actual, delta, null, null);
		}

		public static void AreEqual(double expected, double? actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, actual.Value, delta, message, args);
		}

		public static void AreEqual(double expected, double? actual, double delta, string message)
		{
			AssertDoublesAreEqual(expected, actual.Value, delta, message, null);
		}

		public static void AreEqual(double expected, double? actual, double delta)
		{
			AssertDoublesAreEqual(expected, actual.Value, delta, null, null);
		}

		public static void AreEqual(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.EqualTo(expected), message, args);
		}

		public static void AreEqual(object expected, object actual, string message)
		{
			That(actual, Is.EqualTo(expected), message, null);
		}

		public static void AreEqual(object expected, object actual)
		{
			That(actual, Is.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(int expected, int actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(int expected, int actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(int expected, int actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(long expected, long actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(long expected, long actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(long expected, long actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(uint expected, uint actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(uint expected, uint actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(uint expected, uint actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(ulong expected, ulong actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(ulong expected, ulong actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(ulong expected, ulong actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(decimal expected, decimal actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(decimal expected, decimal actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(decimal expected, decimal actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(float expected, float actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(float expected, float actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(float expected, float actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(double expected, double actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(double expected, double actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(double expected, double actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreNotEqual(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.EqualTo(expected), message, args);
		}

		public static void AreNotEqual(object expected, object actual, string message)
		{
			That(actual, Is.Not.EqualTo(expected), message, null);
		}

		public static void AreNotEqual(object expected, object actual)
		{
			That(actual, Is.Not.EqualTo(expected), null, null);
		}

		public static void AreSame(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.SameAs(expected), message, args);
		}

		public static void AreSame(object expected, object actual, string message)
		{
			That(actual, Is.SameAs(expected), message, null);
		}

		public static void AreSame(object expected, object actual)
		{
			That(actual, Is.SameAs(expected), null, null);
		}

		public static void AreNotSame(object expected, object actual, string message, params object[] args)
		{
			That(actual, Is.Not.SameAs(expected), message, args);
		}

		public static void AreNotSame(object expected, object actual, string message)
		{
			That(actual, Is.Not.SameAs(expected), message, null);
		}

		public static void AreNotSame(object expected, object actual)
		{
			That(actual, Is.Not.SameAs(expected), null, null);
		}

		protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string message, object[] args)
		{
			if (double.IsNaN(expected) || double.IsInfinity(expected))
			{
				That(actual, Is.EqualTo(expected), message, args);
			}
			else
			{
				That(actual, Is.EqualTo(expected).Within(delta), message, args);
			}
		}
	}
}
