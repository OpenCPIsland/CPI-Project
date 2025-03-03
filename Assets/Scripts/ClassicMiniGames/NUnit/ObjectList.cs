using System.Collections;
using System.Collections.Generic;

namespace NUnit
{
	public class ObjectList : List<object>
	{
		public void AddRange(ICollection collection)
		{
			foreach (object item in collection)
			{
				Add(item);
			}
		}
	}
}
