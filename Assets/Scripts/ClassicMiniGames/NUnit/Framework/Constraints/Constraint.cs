using System.Globalization;

namespace NUnit.Framework.Constraints
{
	public abstract class Constraint : IResolveConstraint
	{
		private class UnsetObject
		{
			public override string ToString()
			{
				return "UNSET";
			}
		}

		protected static object UNSET = new UnsetObject();

		protected object actual = UNSET;

		private string displayName;

		private readonly int argcnt;

		private readonly object arg1;

		private readonly object arg2;

		private ConstraintBuilder builder;

		protected string DisplayName
		{
			get
			{
				if (displayName == null)
				{
					displayName = GetType().Name.ToLower();
					if (displayName.EndsWith("`1") || displayName.EndsWith("`2"))
					{
						displayName = displayName.Substring(0, displayName.Length - 2);
					}
					if (displayName.EndsWith("constraint"))
					{
						displayName = displayName.Substring(0, displayName.Length - 10);
					}
				}
				return displayName;
			}
			set
			{
				displayName = value;
			}
		}

		public ConstraintExpression And
		{
			get
			{
				ConstraintBuilder constraintBuilder = builder;
				if (constraintBuilder == null)
				{
					constraintBuilder = new ConstraintBuilder();
					constraintBuilder.Append(this);
				}
				constraintBuilder.Append(new AndOperator());
				return new ConstraintExpression(constraintBuilder);
			}
		}

		public ConstraintExpression With
		{
			get
			{
				return And;
			}
		}

		public ConstraintExpression Or
		{
			get
			{
				ConstraintBuilder constraintBuilder = builder;
				if (constraintBuilder == null)
				{
					constraintBuilder = new ConstraintBuilder();
					constraintBuilder.Append(this);
				}
				constraintBuilder.Append(new OrOperator());
				return new ConstraintExpression(constraintBuilder);
			}
		}

		protected Constraint()
		{
			argcnt = 0;
		}

		protected Constraint(object arg)
		{
			argcnt = 1;
			arg1 = arg;
		}

		protected Constraint(object arg1, object arg2)
		{
			argcnt = 2;
			this.arg1 = arg1;
			this.arg2 = arg2;
		}

		internal void SetBuilder(ConstraintBuilder builder)
		{
			this.builder = builder;
		}

		public virtual void WriteMessageTo(MessageWriter writer)
		{
			writer.DisplayDifferences(this);
		}

		public abstract bool Matches(object actual);

		public virtual bool Matches(ActualValueDelegate del)
		{
			return Matches(del());
		}

		public virtual bool Matches<T>(ref T actual)
		{
			return Matches(actual);
		}

		public abstract void WriteDescriptionTo(MessageWriter writer);

		public virtual void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(actual);
		}

		public override string ToString()
		{
			string stringRepresentation = GetStringRepresentation();
			return (builder == null) ? stringRepresentation : string.Format("<unresolved {0}>", stringRepresentation);
		}

		protected virtual string GetStringRepresentation()
		{
			switch (argcnt)
			{
			default:
				return string.Format("<{0}>", DisplayName);
			case 1:
				return string.Format("<{0} {1}>", DisplayName, _displayable(arg1));
			case 2:
				return string.Format("<{0} {1} {2}>", DisplayName, _displayable(arg1), _displayable(arg2));
			}
		}

		private static string _displayable(object o)
		{
			if (o == null)
			{
				return "null";
			}
			string format = (o is string) ? "\"{0}\"" : "{0}";
			return string.Format(CultureInfo.InvariantCulture, format, o);
		}

		public static Constraint operator &(Constraint left, Constraint right)
		{
			return new AndConstraint(((IResolveConstraint)left).Resolve(), ((IResolveConstraint)right).Resolve());
		}

		public static Constraint operator |(Constraint left, Constraint right)
		{
			return new OrConstraint(((IResolveConstraint)left).Resolve(), ((IResolveConstraint)right).Resolve());
		}

		public static Constraint operator !(Constraint constraint)
		{
			return new NotConstraint((constraint == null) ? new NullConstraint() : ((IResolveConstraint)constraint).Resolve());
		}

		Constraint IResolveConstraint.Resolve()
		{
			return (builder == null) ? this : builder.Resolve();
		}
	}
}
