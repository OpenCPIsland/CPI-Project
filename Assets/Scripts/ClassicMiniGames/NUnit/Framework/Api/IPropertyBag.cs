using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Api
{
	public interface IPropertyBag : IXmlNodeBuilder, IEnumerable
	{
		int Count
		{
			get;
		}

		IList this[string key]
		{
			get;
			set;
		}

		ICollection<string> Keys
		{
			get;
		}

		void Add(string key, object value);

		void Set(string key, object value);

		object Get(string key);

		string GetSetting(string key, string defaultValue);

		int GetSetting(string key, int defaultValue);

		bool GetSetting(string key, bool defaultValue);

		Enum GetSetting(string key, Enum defaultValue);

		void Remove(string key);

		void Remove(string key, object value);

		void Remove(PropertyEntry entry);

		bool ContainsKey(string key);

		bool Contains(string key, object value);

		bool Contains(PropertyEntry entry);
	}
}
