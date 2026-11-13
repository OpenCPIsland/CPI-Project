using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ConstraintExpression : ConstraintExpressionBase
	{
		public ConstraintExpression Not
		{
			get
			{
				return Append(new NotOperator());
			}
		}

		public ConstraintExpression No
		{
			get
			{
				return Append(new NotOperator());
			}
		}

		public ConstraintExpression All
		{
			get
			{
				return Append(new AllOperator());
			}
		}

		public ConstraintExpression Some
		{
			get
			{
				return Append(new SomeOperator());
			}
		}

		public ConstraintExpression None
		{
			get
			{
				return Append(new NoneOperator());
			}
		}

		public ResolvableConstraintExpression Length
		{
			get
			{
				return Property("Length");
			}
		}

		public ResolvableConstraintExpression Count
		{
			get
			{
				return Property("Count");
			}
		}

		public ResolvableConstraintExpression Message
		{
			get
			{
				return Property("Message");
			}
		}

		public ResolvableConstraintExpression InnerException
		{
			get
			{
				return Property("InnerException");
			}
		}

		public ConstraintExpression With
		{
			get
			{
				return Append(new WithOperator());
			}
		}

		public NullConstraint Null
		{
			get
			{
				return (NullConstraint)Append(new NullConstraint());
			}
		}

		public TrueConstraint True
		{
			get
			{
				return (TrueConstraint)Append(new TrueConstraint());
			}
		}

		public FalseConstraint False
		{
			get
			{
				return (FalseConstraint)Append(new FalseConstraint());
			}
		}

		public GreaterThanConstraint Positive
		{
			get
			{
				return (GreaterThanConstraint)Append(new GreaterThanConstraint(0));
			}
		}

		public LessThanConstraint Negative
		{
			get
			{
				return (LessThanConstraint)Append(new LessThanConstraint(0));
			}
		}

		public NaNConstraint NaN
		{
			get
			{
				return (NaNConstraint)Append(new NaNConstraint());
			}
		}

		public EmptyConstraint Empty
		{
			get
			{
				return (EmptyConstraint)Append(new EmptyConstraint());
			}
		}

		public UniqueItemsConstraint Unique
		{
			get
			{
				return (UniqueItemsConstraint)Append(new UniqueItemsConstraint());
			}
		}

		public BinarySerializableConstraint BinarySerializable
		{
			get
			{
				return (BinarySerializableConstraint)Append(new BinarySerializableConstraint());
			}
		}

		public XmlSerializableConstraint XmlSerializable
		{
			get
			{
				return (XmlSerializableConstraint)Append(new XmlSerializableConstraint());
			}
		}

		public CollectionOrderedConstraint Ordered
		{
			get
			{
				return (CollectionOrderedConstraint)Append(new CollectionOrderedConstraint());
			}
		}

		public ConstraintExpression()
		{
		}

		public ConstraintExpression(ConstraintBuilder builder)
			: base(builder)
		{
		}

		public ConstraintExpression Exactly(int expectedCount)
		{
			return Append(new ExactCountOperator(expectedCount));
		}

		public ResolvableConstraintExpression Property(string name)
		{
			return Append(new PropOperator(name));
		}

		public ResolvableConstraintExpression Attribute(Type expectedType)
		{
			return Append(new AttributeOperator(expectedType));
		}

		public ResolvableConstraintExpression Attribute<T>()
		{
			return Attribute(typeof(T));
		}

		public Constraint Matches(Constraint constraint)
		{
			return Append(constraint);
		}

		public EqualConstraint EqualTo(object expected)
		{
			return (EqualConstraint)Append(new EqualConstraint(expected));
		}

		public SameAsConstraint SameAs(object expected)
		{
			return (SameAsConstraint)Append(new SameAsConstraint(expected));
		}

		public GreaterThanConstraint GreaterThan(object expected)
		{
			return (GreaterThanConstraint)Append(new GreaterThanConstraint(expected));
		}

		public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
		{
			return (GreaterThanOrEqualConstraint)Append(new GreaterThanOrEqualConstraint(expected));
		}

		public GreaterThanOrEqualConstraint AtLeast(object expected)
		{
			return (GreaterThanOrEqualConstraint)Append(new GreaterThanOrEqualConstraint(expected));
		}

		public LessThanConstraint LessThan(object expected)
		{
			return (LessThanConstraint)Append(new LessThanConstraint(expected));
		}

		public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
		{
			return (LessThanOrEqualConstraint)Append(new LessThanOrEqualConstraint(expected));
		}

		public LessThanOrEqualConstraint AtMost(object expected)
		{
			return (LessThanOrEqualConstraint)Append(new LessThanOrEqualConstraint(expected));
		}

		public ExactTypeConstraint TypeOf(Type expectedType)
		{
			return (ExactTypeConstraint)Append(new ExactTypeConstraint(expectedType));
		}

		public ExactTypeConstraint TypeOf<T>()
		{
			return (ExactTypeConstraint)Append(new ExactTypeConstraint(typeof(T)));
		}

		public InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return (InstanceOfTypeConstraint)Append(new InstanceOfTypeConstraint(expectedType));
		}

		public InstanceOfTypeConstraint InstanceOf<T>()
		{
			return (InstanceOfTypeConstraint)Append(new InstanceOfTypeConstraint(typeof(T)));
		}

		public AssignableFromConstraint AssignableFrom(Type expectedType)
		{
			return (AssignableFromConstraint)Append(new AssignableFromConstraint(expectedType));
		}

		public AssignableFromConstraint AssignableFrom<T>()
		{
			return (AssignableFromConstraint)Append(new AssignableFromConstraint(typeof(T)));
		}

		public AssignableToConstraint AssignableTo(Type expectedType)
		{
			return (AssignableToConstraint)Append(new AssignableToConstraint(expectedType));
		}

		public AssignableToConstraint AssignableTo<T>()
		{
			return (AssignableToConstraint)Append(new AssignableToConstraint(typeof(T)));
		}

		public CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
		{
			return (CollectionEquivalentConstraint)Append(new CollectionEquivalentConstraint(expected));
		}

		public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
		{
			return (CollectionSubsetConstraint)Append(new CollectionSubsetConstraint(expected));
		}

		public CollectionContainsConstraint Member(object expected)
		{
			return (CollectionContainsConstraint)Append(new CollectionContainsConstraint(expected));
		}

		public CollectionContainsConstraint Contains(object expected)
		{
			return (CollectionContainsConstraint)Append(new CollectionContainsConstraint(expected));
		}

		public ContainsConstraint Contains(string expected)
		{
			return (ContainsConstraint)Append(new ContainsConstraint(expected));
		}

		public SubstringConstraint StringContaining(string expected)
		{
			return (SubstringConstraint)Append(new SubstringConstraint(expected));
		}

		public SubstringConstraint ContainsSubstring(string expected)
		{
			return (SubstringConstraint)Append(new SubstringConstraint(expected));
		}

		public StartsWithConstraint StartsWith(string expected)
		{
			return (StartsWithConstraint)Append(new StartsWithConstraint(expected));
		}

		public StartsWithConstraint StringStarting(string expected)
		{
			return (StartsWithConstraint)Append(new StartsWithConstraint(expected));
		}

		public EndsWithConstraint EndsWith(string expected)
		{
			return (EndsWithConstraint)Append(new EndsWithConstraint(expected));
		}

		public EndsWithConstraint StringEnding(string expected)
		{
			return (EndsWithConstraint)Append(new EndsWithConstraint(expected));
		}

		public RegexConstraint Matches(string pattern)
		{
			return (RegexConstraint)Append(new RegexConstraint(pattern));
		}

		public RegexConstraint StringMatching(string pattern)
		{
			return (RegexConstraint)Append(new RegexConstraint(pattern));
		}

		public SamePathConstraint SamePath(string expected)
		{
			return (SamePathConstraint)Append(new SamePathConstraint(expected));
		}

		public SamePathOrUnderConstraint SamePathOrUnder(string expected)
		{
			return (SamePathOrUnderConstraint)Append(new SamePathOrUnderConstraint(expected));
		}

		public RangeConstraint InRange(IComparable from, IComparable to)
		{
			return (RangeConstraint)Append(new RangeConstraint(from, to));
		}
	}
}
