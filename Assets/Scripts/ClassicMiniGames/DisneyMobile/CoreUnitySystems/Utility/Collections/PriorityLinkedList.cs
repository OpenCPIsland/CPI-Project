using System;
using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class PriorityLinkedList<T> : IEnumerable<ElementPriorityPair<T>>, IEnumerable
	{
		private readonly LinkedList<ElementPriorityPair<T>> _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public PriorityLinkedList()
		{
			_list = new LinkedList<ElementPriorityPair<T>>();
		}

		public void Add(T element, int priority)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "The element can't be null.");
			}
			LinkedListNode<ElementPriorityPair<T>> linkedListNode = _list.First;
			ElementPriorityPair<T> value = new ElementPriorityPair<T>(element, priority);
			while (linkedListNode != null)
			{
				if (priority > linkedListNode.Value.Priority)
				{
					_list.AddBefore(linkedListNode, value);
					return;
				}
				linkedListNode = linkedListNode.Next;
			}
			_list.AddLast(value);
		}

		public bool Remove(T element)
		{
			if (element == null)
			{
				return false;
			}
			for (LinkedListNode<ElementPriorityPair<T>> linkedListNode = _list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (object.Equals(linkedListNode.Value.Element, element))
				{
					_list.Remove(linkedListNode);
					return true;
				}
			}
			return false;
		}

		public IEnumerator<ElementPriorityPair<T>> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T Pop()
		{
			LinkedListNode<ElementPriorityPair<T>> first = _list.First;
			_list.RemoveFirst();
			return first.Value.Element;
		}

		public LinkedListNode<ElementPriorityPair<T>> First()
		{
			return _list.First;
		}
	}
}
