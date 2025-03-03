using System;
using System.Collections;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility.Collections
{
	public class PriorityLinkedList<T> : IEnumerable<ElementPriorityPair<T>>, IEnumerable
	{
		public bool CopyOnWrite = false;

		private LinkedList<ElementPriorityPair<T>> _list;

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
			if (CopyOnWrite)
			{
				_list = new LinkedList<ElementPriorityPair<T>>(_list);
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
			if (CopyOnWrite)
			{
				_list = new LinkedList<ElementPriorityPair<T>>(_list);
			}
			for (LinkedListNode<ElementPriorityPair<T>> linkedListNode = _list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (AreEqual(linkedListNode.Value.Element, element))
				{
					_list.Remove(linkedListNode);
					return true;
				}
			}
			return false;
		}

		public bool Contains(T element)
		{
			for (LinkedListNode<ElementPriorityPair<T>> linkedListNode = _list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (AreEqual(linkedListNode.Value.Element, element))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool AreEqual(T elem1, T elem2)
		{
			return object.Equals(elem1, elem2);
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
			if (CopyOnWrite)
			{
				_list = new LinkedList<ElementPriorityPair<T>>(_list);
			}
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
