using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class ConstraintBuilder
	{
		public class OperatorStack
		{
			private Stack<ConstraintOperator> stack = new Stack<ConstraintOperator>();

			public bool Empty
			{
				get
				{
					return stack.Count == 0;
				}
			}

			public ConstraintOperator Top
			{
				get
				{
					return stack.Peek();
				}
			}

			public OperatorStack(ConstraintBuilder builder)
			{
			}

			public void Push(ConstraintOperator op)
			{
				stack.Push(op);
			}

			public ConstraintOperator Pop()
			{
				return stack.Pop();
			}
		}

		public class ConstraintStack
		{
			private Stack<Constraint> stack = new Stack<Constraint>();

			private ConstraintBuilder builder;

			public bool Empty
			{
				get
				{
					return stack.Count == 0;
				}
			}

			public Constraint Top
			{
				get
				{
					return stack.Peek();
				}
			}

			public ConstraintStack(ConstraintBuilder builder)
			{
				this.builder = builder;
			}

			public void Push(Constraint constraint)
			{
				stack.Push(constraint);
				constraint.SetBuilder(builder);
			}

			public Constraint Pop()
			{
				Constraint constraint = stack.Pop();
				constraint.SetBuilder(null);
				return constraint;
			}
		}

		private readonly OperatorStack ops;

		private readonly ConstraintStack constraints;

		private object lastPushed;

		public bool IsResolvable
		{
			get
			{
				return lastPushed is Constraint || lastPushed is SelfResolvingOperator;
			}
		}

		public ConstraintBuilder()
		{
			ops = new OperatorStack(this);
			constraints = new ConstraintStack(this);
		}

		public void Append(ConstraintOperator op)
		{
			op.LeftContext = lastPushed;
			if (lastPushed is ConstraintOperator)
			{
				SetTopOperatorRightContext(op);
			}
			ReduceOperatorStack(op.LeftPrecedence);
			ops.Push(op);
			lastPushed = op;
		}

		public void Append(Constraint constraint)
		{
			if (lastPushed is ConstraintOperator)
			{
				SetTopOperatorRightContext(constraint);
			}
			constraints.Push(constraint);
			lastPushed = constraint;
			constraint.SetBuilder(this);
		}

		private void SetTopOperatorRightContext(object rightContext)
		{
			int leftPrecedence = ops.Top.LeftPrecedence;
			ops.Top.RightContext = rightContext;
			if (ops.Top.LeftPrecedence > leftPrecedence)
			{
				ConstraintOperator constraintOperator = ops.Pop();
				ReduceOperatorStack(constraintOperator.LeftPrecedence);
				ops.Push(constraintOperator);
			}
		}

		private void ReduceOperatorStack(int targetPrecedence)
		{
			while (!ops.Empty && ops.Top.RightPrecedence < targetPrecedence)
			{
				ops.Pop().Reduce(constraints);
			}
		}

		public Constraint Resolve()
		{
			if (!IsResolvable)
			{
				throw new InvalidOperationException("A partial expression may not be resolved");
			}
			while (!ops.Empty)
			{
				ConstraintOperator constraintOperator = ops.Pop();
				constraintOperator.Reduce(constraints);
			}
			return constraints.Pop();
		}
	}
}
