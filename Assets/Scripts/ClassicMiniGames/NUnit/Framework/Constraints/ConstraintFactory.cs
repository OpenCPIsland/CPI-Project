using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ConstraintFactory
	{
		public ConstraintExpression Not
		{
			get
			{
				return Is.Not;
			}
		}

		public ConstraintExpression No
		{
			get
			{
				return Has.No;
			}
		}

		public ConstraintExpression All
		{
			get
			{
				return Is.All;
			}
		}

		public ConstraintExpression Some
		{
			get
			{
				return Has.Some;
			}
		}

		public ConstraintExpression None
		{
			get
			{
				return Has.None;
			}
		}

		public ResolvableConstraintExpression Length
		{
			get
			{
				return Has.Length;
			}
		}

		public ResolvableConstraintExpression Count
		{
			get
			{
				return Has.Count;
			}
		}

		public ResolvableConstraintExpression Message
		{
			get
			{
				return Has.Message;
			}
		}

		public ResolvableConstraintExpression InnerException
		{
			get
			{
				return Has.InnerException;
			}
		}

		public NullConstraint Null
		{
			get
			{
				return new NullConstraint();
			}
		}

		public TrueConstraint True
		{
			get
			{
				return new TrueConstraint();
			}
		}

		public FalseConstraint False
		{
			get
			{
				return new FalseConstraint();
			}
		}

		public GreaterThanConstraint Positive
		{
			get
			{
				return new GreaterThanConstraint(0);
			}
		}

		public LessThanConstraint Negative
		{
			get
			{
				return new LessThanConstraint(0);
			}
		}

		public NaNConstraint NaN
		{
			get
			{
				return new NaNConstraint();
			}
		}

		public EmptyConstraint Empty
		{
			get
			{
				return new EmptyConstraint();
			}
		}

		public UniqueItemsConstraint Unique
		{
			get
			{
				return new UniqueItemsConstraint();
			}
		}

		public BinarySerializableConstraint BinarySerializable
		{
			get
			{
				return new BinarySerializableConstraint();
			}
		}

		public XmlSerializableConstraint XmlSerializable
		{
			get
			{
				return new XmlSerializableConstraint();
			}
		}

		public CollectionOrderedConstraint Ordered
		{
			get
			{
				return new CollectionOrderedConstraint();
			}
		}

		public static ConstraintExpression Exactly(int expectedCount)
		{
			return Has.Exactly(expectedCount);
		}

		public ResolvableConstraintExpression Property(string name)
		{
			return Has.Property(name);
		}

		public ResolvableConstraintExpression Attribute(Type expectedType)
		{
			return Has.Attribute(expectedType);
		}

		public ResolvableConstraintExpression Attribute<T>()
		{
			return Attribute(typeof(T));
		}

		public EqualConstraint EqualTo(object expected)
		{
			return new EqualConstraint(expected);
		}

		public SameAsConstraint SameAs(object expected)
		{
			return new SameAsConstraint(expected);
		}

		public GreaterThanConstraint GreaterThan(object expected)
		{
			return new GreaterThanConstraint(expected);
		}

		public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public GreaterThanOrEqualConstraint AtLeast(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public LessThanConstraint LessThan(object expected)
		{
			return new LessThanConstraint(expected);
		}

		public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public LessThanOrEqualConstraint AtMost(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public ExactTypeConstraint TypeOf(Type expectedType)
		{
			return new ExactTypeConstraint(expectedType);
		}

		public ExactTypeConstraint TypeOf<T>()
		{
			return new ExactTypeConstraint(typeof(T));
		}

		public InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return new InstanceOfTypeConstraint(expectedType);
		}

		public InstanceOfTypeConstraint InstanceOf<T>()
		{
			return new InstanceOfTypeConstraint(typeof(T));
		}

		public AssignableFromConstraint AssignableFrom(Type expectedType)
		{
			return new AssignableFromConstraint(expectedType);
		}

		public AssignableFromConstraint AssignableFrom<T>()
		{
			return new AssignableFromConstraint(typeof(T));
		}

		public AssignableToConstraint AssignableTo(Type expectedType)
		{
			return new AssignableToConstraint(expectedType);
		}

		public AssignableToConstraint AssignableTo<T>()
		{
			return new AssignableToConstraint(typeof(T));
		}

		public CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
		{
			return new CollectionEquivalentConstraint(expected);
		}

		public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
		{
			return new CollectionSubsetConstraint(expected);
		}

		public CollectionContainsConstraint Member(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public CollectionContainsConstraint Contains(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public ContainsConstraint Contains(string expected)
		{
			return new ContainsConstraint(expected);
		}

		public SubstringConstraint StringContaining(string expected)
		{
			return new SubstringConstraint(expected);
		}

		public SubstringConstraint ContainsSubstring(string expected)
		{
			return new SubstringConstraint(expected);
		}

		public SubstringConstraint DoesNotContain(string expected)
		{
			return new ConstraintExpression().Not.ContainsSubstring(expected);
		}

		public StartsWithConstraint StartsWith(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public StartsWithConstraint StringStarting(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public StartsWithConstraint DoesNotStartWith(string expected)
		{
			return new ConstraintExpression().Not.StartsWith(expected);
		}

		public EndsWithConstraint EndsWith(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public EndsWithConstraint StringEnding(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public EndsWithConstraint DoesNotEndWith(string expected)
		{
			return new ConstraintExpression().Not.EndsWith(expected);
		}

		public RegexConstraint Matches(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		public RegexConstraint StringMatching(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		public RegexConstraint DoesNotMatch(string pattern)
		{
			return new ConstraintExpression().Not.Matches(pattern);
		}

		public SamePathConstraint SamePath(string expected)
		{
			return new SamePathConstraint(expected);
		}

		public SamePathOrUnderConstraint SamePathOrUnder(string expected)
		{
			return new SamePathOrUnderConstraint(expected);
		}

		public RangeConstraint InRange(IComparable from, IComparable to)
		{
			return new RangeConstraint(from, to);
		}
	}
}
