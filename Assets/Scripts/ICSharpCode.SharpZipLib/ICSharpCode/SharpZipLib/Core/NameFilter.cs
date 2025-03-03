using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpZipLib.Core
{
	public class NameFilter : IScanFilter
	{
		private string filter_;

		private ArrayList inclusions_;

		private ArrayList exclusions_;

		public NameFilter(string filter)
		{
			filter_ = filter;
			inclusions_ = new ArrayList();
			exclusions_ = new ArrayList();
			Compile();
		}

		public static bool IsValidExpression(string expression)
		{
			bool result = true;
			try
			{
				new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				return result;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static bool IsValidFilterExpression(string toTest)
		{
			if (toTest == null)
			{
				throw new ArgumentNullException("toTest");
			}
			bool result = true;
			try
			{
				string[] array = SplitQuoted(toTest);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && array[i].Length > 0)
					{
						string pattern = (array[i][0] == '+') ? array[i].Substring(1, array[i].Length - 1) : ((array[i][0] != '-') ? array[i] : array[i].Substring(1, array[i].Length - 1));
						new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
					}
				}
				return result;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static string[] SplitQuoted(string original)
		{
			char c = '\\';
			char[] array = new char[1]
			{
				';'
			};
			ArrayList arrayList = new ArrayList();
			if (original != null && original.Length > 0)
			{
				int num = -1;
				StringBuilder stringBuilder = new StringBuilder();
				while (num < original.Length)
				{
					num++;
					if (num >= original.Length)
					{
						arrayList.Add(stringBuilder.ToString());
					}
					else if (original[num] == c)
					{
						num++;
						if (num >= original.Length)
						{
							throw new ArgumentException("Missing terminating escape character", "original");
						}
						if (Array.IndexOf(array, original[num]) < 0)
						{
							stringBuilder.Append(c);
						}
						stringBuilder.Append(original[num]);
					}
					else if (Array.IndexOf(array, original[num]) >= 0)
					{
						arrayList.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					else
					{
						stringBuilder.Append(original[num]);
					}
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		public override string ToString()
		{
			return filter_;
		}

		public bool IsIncluded(string name)
		{
			bool result = false;
			if (inclusions_.Count == 0)
			{
				return true;
			}
			foreach (Regex item in inclusions_)
			{
				if (item.IsMatch(name))
				{
					return true;
				}
			}
			return result;
		}

		public bool IsExcluded(string name)
		{
			bool result = false;
			foreach (Regex item in exclusions_)
			{
				if (item.IsMatch(name))
				{
					return true;
				}
			}
			return result;
		}

		public bool IsMatch(string name)
		{
			if (IsIncluded(name))
			{
				return !IsExcluded(name);
			}
			return false;
		}

		private void Compile()
		{
			if (filter_ == null)
			{
				return;
			}
			string[] array = SplitQuoted(filter_);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Length > 0)
				{
					bool flag = array[i][0] != '-';
					string pattern = (array[i][0] == '+') ? array[i].Substring(1, array[i].Length - 1) : ((array[i][0] != '-') ? array[i] : array[i].Substring(1, array[i].Length - 1));
					if (flag)
					{
						inclusions_.Add(new Regex(pattern, (RegexOptions)25));
					}
					else
					{
						exclusions_.Add(new Regex(pattern, (RegexOptions)25));
					}
				}
			}
		}
	}
}
