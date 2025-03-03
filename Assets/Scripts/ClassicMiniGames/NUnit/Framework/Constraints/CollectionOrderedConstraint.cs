using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Constraints
{
	public class CollectionOrderedConstraint : CollectionConstraint
	{
		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		private string comparerName;

		private string propertyName;

		private bool descending;

		public CollectionOrderedConstraint Descending
		{
			get
			{
				descending = true;
				return this;
			}
		}

		public CollectionOrderedConstraint()
		{
			base.DisplayName = "ordered";
		}

		public CollectionOrderedConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			comparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			comparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			comparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint By(string propertyName)
		{
			this.propertyName = propertyName;
			return this;
		}

		protected override bool doMatch(IEnumerable actual)
		{
			object obj = null;
			int num = 0;
			foreach (object item in actual)
			{
				object obj2 = item;
				if (item == null)
				{
					throw new ArgumentNullException("actual", "Null value at index " + num);
				}
				if (propertyName != null)
				{
					PropertyInfo property = item.GetType().GetProperty(propertyName);
					obj2 = property.GetValue(item, null);
					if (obj2 == null)
					{
						throw new ArgumentNullException("actual", "Null property value at index " + num);
					}
				}
				if (obj != null)
				{
					int num2 = comparer.Compare(obj, obj2);
					if (descending && num2 < 0)
					{
						return false;
					}
					if (!descending && num2 > 0)
					{
						return false;
					}
				}
				obj = obj2;
				num++;
			}
			return true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			if (propertyName == null)
			{
				writer.Write("collection ordered");
			}
			else
			{
				writer.WritePredicate("collection ordered by");
				writer.WriteExpectedValue(propertyName);
			}
			if (descending)
			{
				writer.WriteModifier("descending");
			}
		}

		protected override string GetStringRepresentation()
		{
			StringBuilder stringBuilder = new StringBuilder("<ordered");
			if (propertyName != null)
			{
				stringBuilder.Append("by " + propertyName);
			}
			if (descending)
			{
				stringBuilder.Append(" descending");
			}
			if (comparerName != null)
			{
				stringBuilder.Append(" " + comparerName);
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}
	}
}
