using NUnit.Framework.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Framework.Internal
{
	public class PropertyBag : IPropertyBag, IXmlNodeBuilder, IEnumerable
	{
		public class PropertyBagEnumerator : IEnumerator<PropertyEntry>, IDisposable, IEnumerator
		{
			private IEnumerator<KeyValuePair<string, IList>> innerEnum;

			private PropertyBag bag;

			private IEnumerator valueEnum;

			PropertyEntry IEnumerator<PropertyEntry>.Current
			{
				get
				{
					return GetCurrentEntry();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return GetCurrentEntry();
				}
			}

			public PropertyBagEnumerator(PropertyBag bag)
			{
				this.bag = bag;
				Initialize();
			}

			private void Initialize()
			{
				innerEnum = bag.inner.GetEnumerator();
				valueEnum = null;
				if (innerEnum.MoveNext())
				{
					valueEnum = innerEnum.Current.Value.GetEnumerator();
				}
			}

			private PropertyEntry GetCurrentEntry()
			{
				if (valueEnum == null)
				{
					throw new InvalidOperationException();
				}
				string key = innerEnum.Current.Key;
				object current = valueEnum.Current;
				return new PropertyEntry(key, current);
			}

			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				if (valueEnum == null)
				{
					return false;
				}
				while (!valueEnum.MoveNext())
				{
					if (!innerEnum.MoveNext())
					{
						valueEnum = null;
						return false;
					}
					valueEnum = innerEnum.Current.Value.GetEnumerator();
				}
				return true;
			}

			void IEnumerator.Reset()
			{
				Initialize();
			}
		}

		private Dictionary<string, IList> inner = new Dictionary<string, IList>();

		public int Count
		{
			get
			{
				int num = 0;
				foreach (string key in inner.Keys)
				{
					num += inner[key].Count;
				}
				return num;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return inner.Keys;
			}
		}

		public IList this[string key]
		{
			get
			{
				IList list;
				if (!TryGetValue(key, out list))
				{
					list = new ObjectList();
					inner.Add(key, list);
				}
				return list;
			}
			set
			{
				inner[key] = value;
			}
		}

		private bool TryGetValue(string key, out IList list)
		{
			return inner.TryGetValue(key, out list);
		}

		public void Add(string key, object value)
		{
			IList list;
			if (!TryGetValue(key, out list))
			{
				list = new ObjectList();
				inner.Add(key, list);
			}
			list.Add(value);
		}

		public void Set(string key, object value)
		{
			IList list = new ObjectList();
			list.Add(value);
			inner[key] = list;
		}

		public object Get(string key)
		{
			IList list;
			return (TryGetValue(key, out list) && list.Count > 0) ? list[0] : null;
		}

		public bool GetSetting(string key, bool defaultValue)
		{
			object obj = Get(key);
			return (obj == null) ? defaultValue : ((bool)obj);
		}

		public string GetSetting(string key, string defaultValue)
		{
			object obj = Get(key);
			return (obj == null) ? defaultValue : ((string)obj);
		}

		public int GetSetting(string key, int defaultValue)
		{
			object obj = Get(key);
			return (obj == null) ? defaultValue : ((int)obj);
		}

		public Enum GetSetting(string key, Enum defaultValue)
		{
			object obj = Get(key);
			return (obj == null) ? defaultValue : ((Enum)obj);
		}

		public void Clear()
		{
			inner.Clear();
		}

		public void Remove(string key)
		{
			inner.Remove(key);
		}

		public void Remove(string key, object value)
		{
			IList list;
			if (TryGetValue(key, out list))
			{
				list.Remove(value);
			}
		}

		public void Remove(PropertyEntry entry)
		{
			Remove(entry.Name, entry.Value);
		}

		public bool ContainsKey(string key)
		{
			return inner.ContainsKey(key);
		}

		public bool Contains(string key, object value)
		{
			IList list;
			return TryGetValue(key, out list) && list.Contains(value);
		}

		public bool Contains(PropertyEntry entry)
		{
			return Contains(entry.Name, entry.Value);
		}

		public IEnumerator GetEnumerator()
		{
			return new PropertyBagEnumerator(this);
		}

		public XmlNode ToXml(bool recursive)
		{
			XmlNode parentNode = XmlHelper.CreateTopLevelElement("dummy");
			return AddToXml(parentNode, recursive);
		}

		public XmlNode AddToXml(XmlNode parentNode, bool recursive)
		{
			XmlNode xmlNode = XmlHelper.AddElement(parentNode, "properties");
			foreach (string key in Keys)
			{
				foreach (object item in this[key])
				{
					XmlNode node = XmlHelper.AddElement(xmlNode, "property");
					XmlHelper.AddAttribute(node, "name", key.ToString());
					XmlHelper.AddAttribute(node, "value", item.ToString());
				}
			}
			return xmlNode;
		}
	}
}
