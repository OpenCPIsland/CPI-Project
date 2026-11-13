using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
	public class ListMapper
	{
		private ICollection original;

		public ListMapper(ICollection original)
		{
			this.original = original;
		}

		public ICollection Property(string name)
		{
			ObjectList objectList = new ObjectList();
			foreach (object item in original)
			{
				PropertyInfo property = item.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property == null)
				{
					throw new ArgumentException(string.Format("{0} does not have a {1} property", item, name));
				}
				objectList.Add(property.GetValue(item, null));
			}
			return objectList;
		}
	}
}
