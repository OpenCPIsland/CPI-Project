using NUnit.Framework.Constraints;
using System;
using System.Collections;

namespace NUnit.Framework
{
	public class Is
	{
		public static ConstraintExpression Not
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

		public static NullConstraint Null
		{
			get
			{
				return new NullConstraint();
			}
		}

		public static TrueConstraint True
		{
			get
			{
				return new TrueConstraint();
			}
		}

		public static FalseConstraint False
		{
			get
			{
				return new FalseConstraint();
			}
		}

		public static GreaterThanConstraint Positive
		{
			get
			{
				return new GreaterThanConstraint(0);
			}
		}

		public static LessThanConstraint Negative
		{
			get
			{
				return new LessThanConstraint(0);
			}
		}

		public static NaNConstraint NaN
		{
			get
			{
				return new NaNConstraint();
			}
		}

		public static EmptyConstraint Empty
		{
			get
			{
				return new EmptyConstraint();
			}
		}

		public static UniqueItemsConstraint Unique
		{
			get
			{
				return new UniqueItemsConstraint();
			}
		}

		public static BinarySerializableConstraint BinarySerializable
		{
			get
			{
				return new BinarySerializableConstraint();
			}
		}

		public static XmlSerializableConstraint XmlSerializable
		{
			get
			{
				return new XmlSerializableConstraint();
			}
		}

		public static CollectionOrderedConstraint Ordered
		{
			get
			{
				return new CollectionOrderedConstraint();
			}
		}

		public static EqualConstraint EqualTo(object expected)
		{
			return new EqualConstraint(expected);
		}

		public static SameAsConstraint SameAs(object expected)
		{
			return new SameAsConstraint(expected);
		}

		public static GreaterThanConstraint GreaterThan(object expected)
		{
			return new GreaterThanConstraint(expected);
		}

		public static GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public static GreaterThanOrEqualConstraint AtLeast(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public static LessThanConstraint LessThan(object expected)
		{
			return new LessThanConstraint(expected);
		}

		public static LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public static LessThanOrEqualConstraint AtMost(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public static ExactTypeConstraint TypeOf(Type expectedType)
		{
			return new ExactTypeConstraint(expectedType);
		}

		public static ExactTypeConstraint TypeOf<T>()
		{
			return new ExactTypeConstraint(typeof(T));
		}

		public static InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return new InstanceOfTypeConstraint(expectedType);
		}

		public static InstanceOfTypeConstraint InstanceOf<T>()
		{
			return new InstanceOfTypeConstraint(typeof(T));
		}

		public static AssignableFromConstraint AssignableFrom(Type expectedType)
		{
			return new AssignableFromConstraint(expectedType);
		}

		public static AssignableFromConstraint AssignableFrom<T>()
		{
			return new AssignableFromConstraint(typeof(T));
		}

		public static AssignableToConstraint AssignableTo(Type expectedType)
		{
			return new AssignableToConstraint(expectedType);
		}

		public static AssignableToConstraint AssignableTo<T>()
		{
			return new AssignableToConstraint(typeof(T));
		}

		public static CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
		{
			return new CollectionEquivalentConstraint(expected);
		}

		public static CollectionSubsetConstraint SubsetOf(IEnumerable expected)
		{
			return new CollectionSubsetConstraint(expected);
		}

		public static SubstringConstraint StringContaining(string expected)
		{
			return new SubstringConstraint(expected);
		}

		public static StartsWithConstraint StringStarting(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public static EndsWithConstraint StringEnding(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public static RegexConstraint StringMatching(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		public static SamePathConstraint SamePath(string expected)
		{
			return new SamePathConstraint(expected);
		}

		public static SamePathOrUnderConstraint SamePathOrUnder(string expected)
		{
			return new SamePathOrUnderConstraint(expected);
		}

		public static RangeConstraint InRange(IComparable from, IComparable to)
		{
			return new RangeConstraint(from, to);
		}
	}
}
