using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class WeakIndex<T> where T : class
	{
		private readonly Dictionary<string, WeakReference> index;

		public WeakIndex()
		{
			index = new Dictionary<string, WeakReference>();
		}

		public bool IsIndexed(string key)
		{
			CheckIsAlive(key);
			return index.ContainsKey(key);
		}

		public bool IsReserved(string key)
		{
			return index.ContainsKey(key) && index[key] == null;
		}

		public void Reserve(string key)
		{
			if (IsIndexed(key))
			{
				if (index[key] == null)
				{
					throw new InvalidOperationException("Cannot reserve key that is already reserved: " + key);
				}
				throw new InvalidOperationException("Cannot reserve key that is already indexed: " + key);
			}
			index.Add(key, null);
		}

		public void Add(string key, T obj)
		{
			if (IsReserved(key))
			{
				index[key] = new WeakReference(obj);
				return;
			}
			CheckIsAlive(key);
			if (index.ContainsKey(key))
			{
				throw new ArgumentException("Attempted to add an index key that was already added: " + key, "key");
			}
			index.Add(key, new WeakReference(obj));
		}

		public void Remove(string key)
		{
			index.Remove(key);
		}

		public void RemoveAll()
		{
			index.Clear();
		}

		public T Get(string key)
		{
			return CheckIsAlive(key);
		}

		public TChild Get<TChild>(string key) where TChild : class, T
		{
			T val = Get(key);
			if (val == null)
			{
				return null;
			}
			try
			{
				return (TChild)val;
			}
			catch (InvalidCastException innerException)
			{
				throw new InvalidOperationException("Asset for key '" + key + "' is of type " + val.GetType().FullName + " and cannot be casted to " + typeof(TChild).FullName, innerException);
			}
		}

		public IEnumerable<T> Search(Func<string, bool> predicate)
		{
			foreach (KeyValuePair<string, WeakReference> kvp in index)
			{
				KeyValuePair<string, WeakReference> keyValuePair = kvp;
				if (predicate(keyValuePair.Key))
				{
					keyValuePair = kvp;
					T obj = CheckIsAlive(keyValuePair.Key);
					if (obj != null)
					{
						yield return obj;
					}
				}
			}
		}

		private T CheckIsAlive(string key)
		{
			WeakReference value;
			if (index.TryGetValue(key, out value))
			{
				if (value == null)
				{
					return null;
				}
				if (!value.IsAlive || object.ReferenceEquals(value.Target, null))
				{
					Remove(key);
					return null;
				}
				return (T)value.Target;
			}
			return null;
		}
	}
}
