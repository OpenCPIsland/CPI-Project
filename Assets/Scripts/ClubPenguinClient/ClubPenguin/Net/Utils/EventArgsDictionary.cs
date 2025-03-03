using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Net.Utils
{
	public class EventArgsDictionary<T, V> : EventArgs, IDictionary, ICollection, IDictionary<T, V>, ICollection<KeyValuePair<T, V>>, IEnumerable<KeyValuePair<T, V>>, IEnumerable
	{
		private Dictionary<T, V> dictionary = new Dictionary<T, V>();

		public int Count
		{
			get
			{
				return dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return dictionary.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return dictionary.Values;
			}
		}

		public object this[object key]
		{
			get
			{
				return dictionary[(T)key];
			}
			set
			{
				dictionary[(T)key] = (V)value;
			}
		}

		ICollection<T> IDictionary<T, V>.Keys
		{
			get
			{
				return dictionary.Keys;
			}
		}

		ICollection<V> IDictionary<T, V>.Values
		{
			get
			{
				return dictionary.Values;
			}
		}

		public V this[T key]
		{
			get
			{
				return dictionary[key];
			}
			set
			{
				dictionary[key] = value;
			}
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		public void CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < dictionary.Count)
			{
				throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");
			}
			foreach (KeyValuePair<T, V> item in dictionary)
			{
				array.SetValue(item, arrayIndex++);
			}
		}

		public virtual void Add(object key, object value)
		{
			dictionary.Add((T)key, (V)value);
		}

		public bool Contains(object key)
		{
			return dictionary.ContainsKey((T)key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		public void Remove(object key)
		{
			dictionary.Remove((T)key);
		}

		public void Add(T key, V value)
		{
			dictionary.Add(key, value);
		}

		public bool ContainsKey(T key)
		{
			return dictionary.ContainsKey(key);
		}

		public bool Remove(T key)
		{
			return dictionary.Remove(key);
		}

		public bool TryGetValue(T key, out V value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<T, V> item)
		{
			dictionary.Add(item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<T, V> item)
		{
			return dictionary.ContainsKey(item.Key) && dictionary[item.Key].Equals(item.Value);
		}

		public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < dictionary.Count)
			{
				throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");
			}
			foreach (KeyValuePair<T, V> item in dictionary)
			{
				array.SetValue(item, arrayIndex++);
			}
		}

		public bool Remove(KeyValuePair<T, V> item)
		{
			return dictionary.Remove(item.Key);
		}

		IEnumerator<KeyValuePair<T, V>> IEnumerable<KeyValuePair<T, V>>.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
	}
}
