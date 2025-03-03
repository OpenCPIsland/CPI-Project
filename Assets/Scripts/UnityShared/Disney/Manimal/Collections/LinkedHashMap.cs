using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Disney.Manimal.Collections
{
	public class LinkedHashMap<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
	{
		protected class Entry
		{
			private readonly TKey key;

			private TValue evalue;

			private Entry next;

			private Entry prev;

			public TKey Key
			{
				get
				{
					return key;
				}
			}

			public TValue Value
			{
				get
				{
					return evalue;
				}
				set
				{
					evalue = value;
				}
			}

			public Entry Next
			{
				get
				{
					return next;
				}
				set
				{
					next = value;
				}
			}

			public Entry Prev
			{
				get
				{
					return prev;
				}
				set
				{
					prev = value;
				}
			}

			public Entry(TKey key, TValue value)
			{
				this.key = key;
				evalue = value;
			}

			public override int GetHashCode()
			{
				int num;
				if (key != null)
				{
					TKey val = key;
					num = val.GetHashCode();
				}
				else
				{
					num = 0;
				}
				return num ^ ((evalue != null) ? evalue.GetHashCode() : 0);
			}

			public override bool Equals(object obj)
			{
				Entry entry = obj as Entry;
				if (entry == null)
				{
					return false;
				}
				if (entry == this)
				{
					return true;
				}
				bool num;
				if (key != null)
				{
					TKey val = key;
					num = val.Equals(entry.Key);
				}
				else
				{
					num = (entry.Key == null);
				}
				return num && ((evalue == null) ? (entry.Value == null) : evalue.Equals(entry.Value));
			}

			public override string ToString()
			{
				return string.Concat("[", key, "=", evalue, "]");
			}
		}

		private class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
		{
			private class Enumerator : IEnumerator<TKey>, IDisposable, IEnumerator
			{
				protected readonly LinkedHashMap<TKey, TValue> dictionary;

				protected Entry current;

				protected readonly long version;

				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}

				public TKey Current
				{
					get
					{
						if (dictionary.version != version)
						{
							throw new InvalidOperationException("Enumerator was modified");
						}
						return current.Key;
					}
				}

				public Enumerator(LinkedHashMap<TKey, TValue> dictionary)
				{
					this.dictionary = dictionary;
					version = dictionary.version;
					current = dictionary.header;
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					if (current.Next == dictionary.header)
					{
						return false;
					}
					current = current.Next;
					return true;
				}

				public void Reset()
				{
					current = dictionary.header;
				}
			}

			private readonly LinkedHashMap<TKey, TValue> dictionary;

			public int Count
			{
				get
				{
					return dictionary.Count;
				}
			}

			public object SyncRoot
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsSynchronized
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public KeyCollection(LinkedHashMap<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("LinkedHashMap KeyCollection is readonly.");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("LinkedHashMap KeyCollection is readonly.");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				foreach (TKey item2 in (IEnumerable<TKey>)this)
				{
					if (item2.Equals(item))
					{
						return true;
					}
				}
				return false;
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				foreach (TKey item in (IEnumerable<TKey>)this)
				{
					array[arrayIndex++] = item;
				}
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("LinkedHashMap KeyCollection is readonly.");
			}

			public void CopyTo(Array array, int index)
			{
				foreach (TKey item in (IEnumerable<TKey>)this)
				{
					array.SetValue(item, index++);
				}
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return new Enumerator(dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<TKey>)this).GetEnumerator();
			}
		}

		private class ValuesCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
		{
			private class Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				protected readonly LinkedHashMap<TKey, TValue> dictionary;

				protected Entry current;

				protected readonly long version;

				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}

				public TValue Current
				{
					get
					{
						if (dictionary.version != version)
						{
							throw new InvalidOperationException("Enumerator was modified");
						}
						return current.Value;
					}
				}

				public Enumerator(LinkedHashMap<TKey, TValue> dictionary)
				{
					this.dictionary = dictionary;
					version = dictionary.version;
					current = dictionary.header;
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					if (current.Next == dictionary.header)
					{
						return false;
					}
					current = current.Next;
					return true;
				}

				public void Reset()
				{
					current = dictionary.header;
				}
			}

			private readonly LinkedHashMap<TKey, TValue> dictionary;

			public int Count
			{
				get
				{
					return dictionary.Count;
				}
			}

			public object SyncRoot
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsSynchronized
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public ValuesCollection(LinkedHashMap<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("LinkedHashMap ValuesCollection is readonly.");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("LinkedHashMap ValuesCollection is readonly.");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				foreach (TValue item2 in (IEnumerable<TValue>)this)
				{
					if (item2 == null)
					{
						if (item == null)
						{
							return true;
						}
					}
					else if (item2.Equals(item))
					{
						return true;
					}
				}
				return false;
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				foreach (TValue item in (IEnumerable<TValue>)this)
				{
					array[arrayIndex++] = item;
				}
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("LinkedHashMap ValuesCollection is readonly.");
			}

			public void CopyTo(Array array, int index)
			{
				foreach (TValue item in (IEnumerable<TValue>)this)
				{
					array.SetValue(item, index++);
				}
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return new Enumerator(dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<TValue>)this).GetEnumerator();
			}
		}

		private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
		{
			protected readonly LinkedHashMap<TKey, TValue> dictionary;

			protected Entry current;

			protected readonly long version;

			object IEnumerator.Current
			{
				get
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
				}
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					return current.Key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					return current.Value;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					if (dictionary.version != version)
					{
						throw new InvalidOperationException("Enumerator was modified");
					}
					return new DictionaryEntry(current.Key, current.Value);
				}
			}

			public Enumerator(LinkedHashMap<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
				version = dictionary.version;
				current = dictionary.header;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (dictionary.version != version)
				{
					throw new InvalidOperationException("Enumerator was modified");
				}
				if (current.Next == dictionary.header)
				{
					return false;
				}
				current = current.Next;
				return true;
			}

			public void Reset()
			{
				current = dictionary.header;
			}
		}

		private readonly Entry header;

		private readonly Dictionary<TKey, Entry> entries;

		private long version;

		public virtual TValue this[TKey key]
		{
			get
			{
				return entries[key].Value;
			}
			set
			{
				if (entries.ContainsKey(key))
				{
					OverrideEntry(entries[key], value);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		public virtual ICollection<TKey> Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return new ValuesCollection(this);
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		public virtual ICollection<TValue> Values
		{
			get
			{
				return new ValuesCollection(this);
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this[(TKey)key];
			}
			set
			{
				this[(TKey)key] = (TValue)value;
			}
		}

		public virtual int Count
		{
			get
			{
				return entries.Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		private bool IsEmpty
		{
			get
			{
				return header.Next == header;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public virtual TKey FirstKey
		{
			get
			{
				return (First == null) ? default(TKey) : First.Key;
			}
		}

		public virtual TValue FirstValue
		{
			get
			{
				return (First == null) ? default(TValue) : First.Value;
			}
		}

		public virtual TKey LastKey
		{
			get
			{
				return (Last == null) ? default(TKey) : Last.Key;
			}
		}

		public virtual TValue LastValue
		{
			get
			{
				return (Last == null) ? default(TValue) : Last.Value;
			}
		}

		private Entry First
		{
			get
			{
				return IsEmpty ? null : header.Next;
			}
		}

		private Entry Last
		{
			get
			{
				return IsEmpty ? null : header.Prev;
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return Values;
			}
		}

		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				return this[key];
			}
			set
			{
				this[key] = value;
			}
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count
		{
			get
			{
				return Count;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return IsReadOnly;
			}
		}

		public LinkedHashMap()
			: this(0)
		{
		}

		public LinkedHashMap(int capacity)
		{
			header = CreateSentinel();
			entries = new Dictionary<TKey, Entry>(capacity);
		}

		public virtual bool ContainsKey(TKey key)
		{
			return entries.ContainsKey(key);
		}

		public virtual void Add(TKey key, TValue value)
		{
			Entry entry = new Entry(key, value);
			entries.Add(key, entry);
			version++;
			InsertEntry(entry);
		}

		public virtual void AddAfter(TKey previous, TKey key, TValue value)
		{
			Entry entry = new Entry(key, value);
			entries.Add(key, entry);
			version++;
			Entry entry2 = entries[previous];
			Entry next = entry2.Next;
			next.Prev = entry;
			entry.Next = next;
			entry2.Next = entry;
			entry.Prev = entry2;
		}

		public virtual void AddFirst(TKey key, TValue value)
		{
			Entry entry = new Entry(key, value);
			entries.Add(key, entry);
			version++;
			entry.Next = header.Next;
			entry.Next.Prev = entry;
			entry.Prev = header;
			entry.Prev.Next = entry;
		}

		public virtual void AddAfter(object previous, object key, object value)
		{
			AddAfter((TKey)previous, (TKey)key, (TValue)value);
		}

		public TKey GetPreviousKey(TKey key, out bool isFirst)
		{
			Entry entry = entries[key];
			isFirst = (entry.Prev == header);
			return entry.Prev.Key;
		}

		public virtual bool Remove(TKey key)
		{
			return RemoveImpl(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool flag = entries.ContainsKey(key);
			if (flag)
			{
				value = entries[key].Value;
			}
			else
			{
				value = default(TValue);
			}
			return flag;
		}

		private void OverrideEntry(Entry e, TValue value)
		{
			version++;
			e.Value = value;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool IDictionary.Contains(object key)
		{
			return Contains((TKey)key);
		}

		void IDictionary.Add(object key, object value)
		{
			Add((TKey)key, (TValue)value);
		}

		public virtual void Clear()
		{
			version++;
			entries.Clear();
			header.Next = header;
			header.Prev = header;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Enumerator(this);
		}

		void IDictionary.Remove(object key)
		{
			Remove((TKey)key);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item.Key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			IEnumerator<TKey> enumerator = Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TKey current = enumerator.Current;
				TValue value = this[current];
				array[arrayIndex++] = new KeyValuePair<TKey, TValue>(current, value);
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public virtual bool Contains(TKey key)
		{
			return ContainsKey(key);
		}

		public virtual bool ContainsValue(TValue value)
		{
			if (value == null)
			{
				for (Entry next = header.Next; next != header; next = next.Next)
				{
					if (next.Value == null)
					{
						return true;
					}
				}
			}
			else
			{
				for (Entry next = header.Next; next != header; next = next.Next)
				{
					if (value.Equals(next.Value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static Entry CreateSentinel()
		{
			Entry entry = new Entry(default(TKey), default(TValue));
			entry.Prev = entry;
			entry.Next = entry;
			return entry;
		}

		private static void RemoveEntry(Entry entry)
		{
			entry.Next.Prev = entry.Prev;
			entry.Prev.Next = entry.Next;
		}

		private void InsertEntry(Entry entry)
		{
			entry.Next = header;
			entry.Prev = header.Prev;
			header.Prev.Next = entry;
			header.Prev = entry;
		}

		private bool RemoveImpl(TKey key)
		{
			bool result = false;
			if (entries.ContainsKey(key))
			{
				Entry entry = entries[key];
				result = entries.Remove(key);
				version++;
				RemoveEntry(entry);
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			for (Entry next = header.Next; next != header; next = next.Next)
			{
				stringBuilder.Append(next.Key);
				stringBuilder.Append('=');
				stringBuilder.Append(next.Value);
				if (next.Next != header)
				{
					stringBuilder.Append(',');
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			Add(key, value);
		}

		bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
		{
			return ContainsKey(key);
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			return Remove(key);
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			return TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
