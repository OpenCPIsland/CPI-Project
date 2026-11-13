using NUnit.Framework.Constraints;
using System;

namespace NUnit.Framework
{
	public class Has
	{
		public static ConstraintExpression No
		{
			get
			{
				return new ConstraintExpression().Not;
			}
		}

		public static ConstraintExpression All
		{
			get
			{
				return new ConstraintExpression().All;
			}
		}

		public static ConstraintExpression Some
		{
			get
			{
				return new ConstraintExpression().Some;
			}
		}

		public static ConstraintExpression None
		{
			get
			{
				return new ConstraintExpression().None;
			}
		}

		public static ResolvableConstraintExpression Length
		{
			get
			{
				return Property("Length");
			}
		}

		public static ResolvableConstraintExpression Count
		{
			get
			{
				return Property("Count");
			}
		}

		public static ResolvableConstraintExpression Message
		{
			get
			{
				return Property("Message");
			}
		}

		public static ResolvableConstraintExpression InnerException
		{
			get
			{
				return Property("InnerException");
			}
		}

		public static ConstraintExpression Exactly(int expectedCount)
		{
			return new ConstraintExpression().Exactly(expectedCount);
		}

		public static ResolvableConstraintExpression Property(string name)
		{
			return new ConstraintExpression().Property(name);
		}

		public static ResolvableConstraintExpression Attribute(Type expectedType)
		{
			return new ConstraintExpression().Attribute(expectedType);
		}

		public static ResolvableConstraintExpression Attribute<T>()
		{
			return Attribute(typeof(T));
		}

		public static CollectionContainsConstraint Member(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}
	}
}
