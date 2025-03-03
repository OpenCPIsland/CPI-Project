using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems
{
	public static class DictionaryExtension
	{
		public static IDictionary<string, object> GetOrAddDic(this IDictionary<string, object> thisDictionary, string key)
		{
			object value;
			thisDictionary.TryGetValue(key, out value);
			if (value == null)
			{
				value = new Dictionary<string, object>();
				thisDictionary.Add(key, value);
			}
			return value as IDictionary<string, object>;
		}
	}
}
