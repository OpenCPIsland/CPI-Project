namespace NUnit.Framework.Constraints
{
	public class AndConstraint : BinaryConstraint
	{
		private enum FailurePoint
		{
			None,
			Left,
			Right
		}

		private FailurePoint failurePoint;

		public AndConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			failurePoint = ((!Left.Matches(actual)) ? FailurePoint.Left : ((!Right.Matches(actual)) ? FailurePoint.Right : FailurePoint.None));
			return failurePoint == FailurePoint.None;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			Left.WriteDescriptionTo(writer);
			writer.WriteConnector("and");
			Right.WriteDescriptionTo(writer);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			switch (failurePoint)
			{
			case FailurePoint.Left:
				Left.WriteActualValueTo(writer);
				break;
			case FailurePoint.Right:
				Right.WriteActualValueTo(writer);
				break;
			default:
				base.WriteActualValueTo(writer);
				break;
			}
		}
	}
}
