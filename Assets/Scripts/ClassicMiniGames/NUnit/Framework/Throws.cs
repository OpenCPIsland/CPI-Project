using NUnit.Framework.Constraints;
using System;
using System.Reflection;

namespace NUnit.Framework
{
	public class Throws
	{
		public static ResolvableConstraintExpression Exception
		{
			get
			{
				return new ConstraintExpression().Append(new ThrowsOperator());
			}
		}

		public static ResolvableConstraintExpression InnerException
		{
			get
			{
				return Exception.InnerException;
			}
		}

		public static ExactTypeConstraint TargetInvocationException
		{
			get
			{
				return TypeOf(typeof(TargetInvocationException));
			}
		}

		public static ExactTypeConstraint ArgumentException
		{
			get
			{
				return TypeOf(typeof(ArgumentException));
			}
		}

		public static ExactTypeConstraint InvalidOperationException
		{
			get
			{
				return TypeOf(typeof(InvalidOperationException));
			}
		}

		public static ThrowsNothingConstraint Nothing
		{
			get
			{
				return new ThrowsNothingConstraint();
			}
		}

		public static ExactTypeConstraint TypeOf(Type expectedType)
		{
			return Exception.TypeOf(expectedType);
		}

		public static ExactTypeConstraint TypeOf<T>()
		{
			return TypeOf(typeof(T));
		}

		public static InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return Exception.InstanceOf(expectedType);
		}

		public static InstanceOfTypeConstraint InstanceOf<T>()
		{
			return InstanceOf(typeof(T));
		}
	}
}
