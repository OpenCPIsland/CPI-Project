using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class EmptyCollectionConstraint : CollectionConstraint
	{
		protected override bool doMatch(IEnumerable collection)
		{
			return CollectionConstraint.IsEmpty(collection);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("<empty>");
		}
	}
}
