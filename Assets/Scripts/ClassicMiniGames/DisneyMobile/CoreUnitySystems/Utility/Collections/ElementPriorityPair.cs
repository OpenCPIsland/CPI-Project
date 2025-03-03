namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class ElementPriorityPair<T>
	{
		public T Element
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public ElementPriorityPair(T element, int priority)
		{
			Element = element;
			Priority = priority;
		}
	}
}
