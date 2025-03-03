using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class FastList<Key, Data>
	{
		[SerializeField]
		public List<Key> _keys = new List<Key>();

		[SerializeField]
		public List<Data> _data = new List<Data>();

		private Dictionary<Key, Data> _fastList = new Dictionary<Key, Data>();

		public void BuildFast()
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				if (!_fastList.ContainsKey(_keys[i]))
				{
					_fastList.Add(_keys[i], _data[i]);
				}
			}
		}

		public void Clear()
		{
			_keys.Clear();
			_data.Clear();
			_fastList.Clear();
		}

		public Key[] Keys()
		{
			return _keys.ToArray();
		}

		public int GetIndexByData(Data data)
		{
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Equals(data))
				{
					return i;
				}
			}
			return -1;
		}

		public int GetIndexByKey(Key key)
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i].Equals(key))
				{
					return i;
				}
			}
			return -1;
		}

		public bool UpdateKey(Key oldKey, Key newKey)
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i].Equals(oldKey) && !_keys.Contains(newKey))
				{
					_keys[i] = newKey;
					return true;
				}
			}
			return false;
		}

		public Data FindItem(Key key)
		{
			if (_fastList.ContainsKey(key))
			{
				return _fastList[key];
			}
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i].Equals(key))
				{
					return _data[i];
				}
			}
			return default(Data);
		}

		public int GetCount()
		{
			return _keys.Count;
		}

		public Data FindItemByIndex(int index)
		{
			if (index < _keys.Count)
			{
				return _data[index];
			}
			return default(Data);
		}

		public void Remove(Key name)
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i].Equals(name))
				{
					_keys.RemoveAt(i);
					_data.RemoveAt(i);
					break;
				}
			}
			if (_fastList.ContainsKey(name))
			{
				_fastList.Remove(name);
			}
		}

		public void Add(Key name, Data item)
		{
			if (!Contains(name))
			{
				_keys.Add(name);
				_data.Add(item);
			}
		}

		public bool Contains(Key name)
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i].Equals(name))
				{
					return true;
				}
			}
			return false;
		}
	}
}
