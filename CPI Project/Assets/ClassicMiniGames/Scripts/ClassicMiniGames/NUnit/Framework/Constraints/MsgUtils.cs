using System;
using System.Collections;
using System.Text;

namespace NUnit.Framework.Constraints
{
	public class MsgUtils
	{
		private const string ELLIPSIS = "...";

		public static string GetTypeRepresentation(object obj)
		{
			Array array = obj as Array;
			if (array == null)
			{
				return string.Format("<{0}>", obj.GetType());
			}
			StringBuilder stringBuilder = new StringBuilder();
			Type type = array.GetType();
			int num = 0;
			while (type.IsArray)
			{
				type = type.GetElementType();
				num++;
			}
			stringBuilder.Append(type.ToString());
			stringBuilder.Append('[');
			for (int i = 0; i < array.Rank; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(array.GetLength(i));
			}
			stringBuilder.Append(']');
			while (--num > 0)
			{
				stringBuilder.Append("[]");
			}
			return string.Format("<{0}>", stringBuilder.ToString());
		}

		public static string EscapeControlChars(string s)
		{
			if (s != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = s;
				foreach (char c in text)
				{
					switch (c)
					{
					case '\\':
						stringBuilder.Append("\\\\");
						break;
					case '\0':
						stringBuilder.Append("\\0");
						break;
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\u0085':
					case '\u2028':
					case '\u2029':
						stringBuilder.Append(string.Format("\\x{0:X4}", (int)c));
						break;
					default:
						stringBuilder.Append(c);
						break;
					}
				}
				s = stringBuilder.ToString();
			}
			return s;
		}

		public static string GetArrayIndicesAsString(int[] indices)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(indices[i].ToString());
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public static int[] GetArrayIndicesFromCollectionIndex(IEnumerable collection, int index)
		{
			Array array = collection as Array;
			int num = (array == null) ? 1 : array.Rank;
			int[] array2 = new int[num];
			int num2 = num;
			while (--num2 > 0)
			{
				int length = array.GetLength(num2);
				array2[num2] = index % length;
				index /= length;
			}
			array2[0] = index;
			return array2;
		}

		public static string ClipString(string s, int maxStringLength, int clipStart)
		{
			int num = maxStringLength;
			StringBuilder stringBuilder = new StringBuilder();
			if (clipStart > 0)
			{
				num -= "...".Length;
				stringBuilder.Append("...");
			}
			if (s.Length - clipStart > num)
			{
				num -= "...".Length;
				stringBuilder.Append(s.Substring(clipStart, num));
				stringBuilder.Append("...");
			}
			else if (clipStart > 0)
			{
				stringBuilder.Append(s.Substring(clipStart));
			}
			else
			{
				stringBuilder.Append(s);
			}
			return stringBuilder.ToString();
		}

		public static void ClipExpectedAndActual(ref string expected, ref string actual, int maxDisplayLength, int mismatch)
		{
			int num = Math.Max(expected.Length, actual.Length);
			if (num > maxDisplayLength)
			{
				int num2 = maxDisplayLength - "...".Length;
				int num3 = num - num2;
				if (num3 > mismatch)
				{
					num3 = Math.Max(0, mismatch - num2 / 2);
				}
				expected = ClipString(expected, maxDisplayLength, num3);
				actual = ClipString(actual, maxDisplayLength, num3);
			}
		}

		public static int FindMismatchPosition(string expected, string actual, int istart, bool ignoreCase)
		{
			int num = Math.Min(expected.Length, actual.Length);
			string text = ignoreCase ? expected.ToLower() : expected;
			string text2 = ignoreCase ? actual.ToLower() : actual;
			for (int i = istart; i < num; i++)
			{
				if (text[i] != text2[i])
				{
					return i;
				}
			}
			if (expected.Length != actual.Length)
			{
				return num;
			}
			return -1;
		}
	}
}
