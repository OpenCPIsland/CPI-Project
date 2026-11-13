using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Disney.Manimal.Http
{
	public class HttpQueryParamCollection : Collection<HttpParameter>
	{
		public ICollection<string> Names
		{
			get
			{
				List<string> list = new List<string>();
				using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						HttpParameter current = enumerator.Current;
						if (!list.Contains(current.Name))
						{
							list.Add(current.Name);
						}
					}
				}
				return list;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				List<string> list = new List<string>();
				using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						HttpParameter current = enumerator.Current;
						list.Add(current.Value);
					}
				}
				return list;
			}
		}

		public HttpQueryParamCollection()
		{
		}

		public HttpQueryParamCollection(string query)
		{
			if (!string.IsNullOrEmpty(query))
			{
				ParseQueryString(query);
			}
		}

		public void Add(string name, string value)
		{
			CleanNameAndValue(ref name, ref value);
			Add(new HttpParameter
			{
				Name = name,
				Value = value
			});
		}

		public void Remove(string name)
		{
			Remove(name, false);
		}

		public void Remove(string name, bool ignoreCase)
		{
			StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			List<HttpParameter> list = null;
			using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HttpParameter current = enumerator.Current;
					if (string.Equals(current.Name, name, comparisonType))
					{
						if (list == null)
						{
							list = new List<HttpParameter>();
						}
						list.Add(current);
					}
				}
			}
			if (list != null)
			{
				foreach (HttpParameter item in list)
				{
					Remove(item);
				}
			}
		}

		public bool ContainsName(string name)
		{
			return ContainsName(name, false);
		}

		public bool ContainsName(string name, bool ignoreCase)
		{
			StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HttpParameter current = enumerator.Current;
					if (string.Equals(current.Name, name, comparisonType))
					{
						return true;
					}
				}
			}
			return false;
		}

		public string[] GetValues(string name)
		{
			return GetValues(name, false);
		}

		public string[] GetValues(string name, bool ignoreCase)
		{
			StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			List<string> list = new List<string>();
			using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HttpParameter current = enumerator.Current;
					if (string.Equals(current.Name, name, comparisonType))
					{
						list.Add(current.Value);
					}
				}
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public virtual string ToString(ICollection<string> excludeNames)
		{
			if (base.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<HttpParameter> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HttpParameter current = enumerator.Current;
					string name = current.Name;
					string value = current.Value;
					bool flag = !string.IsNullOrEmpty(name);
					bool flag2 = !string.IsNullOrEmpty(value);
					if ((flag || flag2) && (!flag || excludeNames == null || !excludeNames.Contains(name)))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append('&');
						}
						if (flag)
						{
							stringBuilder.Append(Uri.EscapeDataString(name)).Append("=");
						}
						if (flag2)
						{
							value = Uri.EscapeDataString(value);
							stringBuilder.Append(value);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		protected override void InsertItem(int index, HttpParameter item)
		{
			if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.Value))
			{
				throw new InvalidOperationException("Can't add a name/value pair where both the name and value are null or empty.");
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, HttpParameter item)
		{
			if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.Value))
			{
				throw new InvalidOperationException("Can't add a name/value pair where both the name and value are null or empty.");
			}
			base.SetItem(index, item);
		}

		private void ParseQueryString(string query)
		{
			string[] array = query.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				string name;
				string value;
				if (array3.Length == 2)
				{
					name = Uri.UnescapeDataString(array3[0]);
					value = Uri.UnescapeDataString(array3[1]);
				}
				else
				{
					name = string.Empty;
					value = Uri.UnescapeDataString(array3[0]);
				}
				CleanNameAndValue(ref name, ref value);
				if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(value))
				{
					Add(name, value);
				}
			}
		}

		private static void CleanNameAndValue(ref string name, ref string value)
		{
			name = (string.IsNullOrEmpty(name) ? string.Empty : name);
			value = (string.IsNullOrEmpty(value) ? string.Empty : value);
		}
	}
}
