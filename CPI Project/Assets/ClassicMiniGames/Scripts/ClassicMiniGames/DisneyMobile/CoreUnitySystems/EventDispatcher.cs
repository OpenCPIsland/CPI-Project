using DisneyMobile.CoreUnitySystems.Utility.Collections;
using System;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public class EventDispatcher
	{
		public enum Priority
		{
			LAST = -9999,
			LOW = -999,
			DEFAULT = 0,
			HIGH = 999,
			FIRST = 9999
		}

		public const bool CONTINUE_PROCESSING = false;

		public const bool TERMINATE_PROCESSING = true;

		private Dictionary<Type, PriorityLinkedList<Delegate>> m_handlerTable = new Dictionary<Type, PriorityLinkedList<Delegate>>();

		public void AddListener<T>(EventHandlerDelegate<T> handler, Priority priority = Priority.DEFAULT) where T : BaseEvent
		{
			Type typeFromHandle = typeof(T);
			if (!m_handlerTable.ContainsKey(typeFromHandle))
			{
				m_handlerTable.Add(typeFromHandle, new PriorityLinkedList<Delegate>());
			}
			PriorityLinkedList<Delegate> priorityLinkedList = m_handlerTable[typeFromHandle];
			priorityLinkedList.Add(handler, (int)priority);
		}

		public bool RemoveListener<T>(EventHandlerDelegate<T> handler) where T : BaseEvent
		{
			Type typeFromHandle = typeof(T);
			if (!m_handlerTable.ContainsKey(typeFromHandle))
			{
				return false;
			}
			PriorityLinkedList<Delegate> priorityLinkedList = m_handlerTable[typeFromHandle];
			return priorityLinkedList.Remove(handler);
		}

		public bool DispatchEvent<T>(T evt) where T : BaseEvent
		{
			Type type = evt.GetType();
			if (!m_handlerTable.ContainsKey(type))
			{
				return false;
			}
			PriorityLinkedList<Delegate> priorityLinkedList = m_handlerTable[type];
			LinkedListNode<ElementPriorityPair<Delegate>> linkedListNode = priorityLinkedList.First();
			while (linkedListNode != null)
			{
				LinkedListNode<ElementPriorityPair<Delegate>> next = linkedListNode.Next;
				Delegate element = linkedListNode.Value.Element;
				if (element.Target.Equals(null))
				{
					priorityLinkedList.Remove(linkedListNode.Value.Element);
					linkedListNode = next;
					continue;
				}
				if ((bool)element.DynamicInvoke(evt))
				{
					return true;
				}
				linkedListNode = linkedListNode.Next;
			}
			return false;
		}

		public void ClearAll()
		{
			m_handlerTable.Clear();
		}

		public void OnApplicationQuit()
		{
			ClearAll();
		}
	}
}
