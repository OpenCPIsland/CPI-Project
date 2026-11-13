using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> keys = new List<TKey>();

		[SerializeField]
		private List<TValue> values = new List<TValue>();

		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					keys.Add(current.Key);
					values.Add(current.Value);
				}
			}
		}

		public void OnAfterDeserialize()
		{
			Clear();
			if (keys.Count != values.Count)
			{
				throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
			}
			for (int i = 0; i < keys.Count; i++)
			{
				Add(keys[i], values[i]);
			}
		}

		public void RemoveByValue(TValue someValue)
		{
			List<TKey> list = new List<TKey>();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					if (current.Value.Equals(someValue))
					{
						list.Add(current.Key);
					}
				}
			}
			foreach (TKey item in list)
			{
				Remove(item);
			}
		}
	}
}
