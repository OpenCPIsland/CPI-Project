using Disney.LaunchPadFramework.Utility.Collections;
using System;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework
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

		private Dictionary<Type, object> m_handlerTable = new Dictionary<Type, object>();

		public PriorityLinkedList<EventHandlerDelegate<TEvent>> GetHandlerList<TEvent>() where TEvent : struct
		{
			Type typeFromHandle = typeof(TEvent);
			object value;
			if (!m_handlerTable.TryGetValue(typeFromHandle, out value))
			{
				value = new PriorityLinkedList<EventHandlerDelegate<TEvent>>();
				m_handlerTable.Add(typeFromHandle, value);
			}
			return (PriorityLinkedList<EventHandlerDelegate<TEvent>>)value;
		}

		public void AddListener<T>(EventHandlerDelegate<T> handler, Priority priority = Priority.DEFAULT) where T : struct
		{
			GetHandlerList<T>().Add(handler, (int)priority);
		}

		public bool RemoveListener<T>(EventHandlerDelegate<T> handler) where T : struct
		{
			if (!m_handlerTable.ContainsKey(typeof(T)))
			{
				return false;
			}
			return GetHandlerList<T>().Remove(handler);
		}

		public bool DispatchEvent<T>(T evt) where T : struct
		{
			Type typeFromHandle = typeof(T);
			if (!m_handlerTable.ContainsKey(typeFromHandle))
			{
				return false;
			}
			PriorityLinkedList<EventHandlerDelegate<T>> handlerList = GetHandlerList<T>();
			bool copyOnWrite = handlerList.CopyOnWrite;
			handlerList.CopyOnWrite = true;
			LinkedListNode<ElementPriorityPair<EventHandlerDelegate<T>>> linkedListNode = handlerList.First();
			while (linkedListNode != null)
			{
				LinkedListNode<ElementPriorityPair<EventHandlerDelegate<T>>> next = linkedListNode.Next;
				EventHandlerDelegate<T> element = linkedListNode.Value.Element;
				if (element.Target != null && element.Target.Equals(null))
				{
					handlerList.Remove(linkedListNode.Value.Element);
					linkedListNode = next;
				}
				else
				{
					try
					{
						if (handlerList.Contains(element) && element(evt))
						{
							handlerList.CopyOnWrite = copyOnWrite;
							return true;
						}
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "The event handler for event '{0}' threw an exception. Execution will continue but the game may be in a broken state.", typeof(T).FullName);
						Log.LogException(this, ex);
					}
					linkedListNode = next;
				}
			}
			handlerList.CopyOnWrite = copyOnWrite;
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
