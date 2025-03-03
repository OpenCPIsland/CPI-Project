using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class UniqueItemsConstraint : CollectionItemsEqualConstraint
	{
		protected override bool doMatch(IEnumerable actual)
		{
			ObjectList objectList = new ObjectList();
			foreach (object item in actual)
			{
				foreach (object item2 in objectList)
				{
					if (ItemsEqual(item, item2))
					{
						return false;
					}
				}
				objectList.Add(item);
			}
			return true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("all items unique");
		}
	}
}
