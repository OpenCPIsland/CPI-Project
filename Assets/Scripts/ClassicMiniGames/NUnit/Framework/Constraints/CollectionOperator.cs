namespace NUnit.Framework.Constraints
{
	public abstract class CollectionOperator : PrefixOperator
	{
		protected CollectionOperator()
		{
			left_precedence = 1;
			right_precedence = 10;
		}
	}
}
