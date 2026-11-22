#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanRawJson
	{
		public readonly string Text;

		public LanRawJson(string text)
		{
			Text = text;
		}
	}

	public static class LanJson
	{
		public static object Parse(string text)
		{
			if (text == null)
			{
				return null;
			}
			int pos = 0;
			object value = ParseValue(text, ref pos, 0);
			SkipWhitespace(text, ref pos);
			if (pos != text.Length)
			{
				throw new FormatException("trailing data");
			}
			return value;
		}

		public static bool TryParse(string text, out object value)
		{
			try
			{
				value = Parse(text);
				return true;
			}
			catch (Exception)
			{
				value = null;
				return false;
			}
		}

		private static void SkipWhitespace(string s, ref int pos)
		{
			while (pos < s.Length && (s[pos] == ' ' || s[pos] == '\t' || s[pos] == '\r' || s[pos] == '\n'))
			{
				pos++;
			}
		}

		private static object ParseValue(string s, ref int pos, int depth)
		{
			if (depth > 64)
			{
				throw new FormatException("too deep");
			}
			SkipWhitespace(s, ref pos);
			if (pos >= s.Length)
			{
				throw new FormatException("unexpected end");
			}
			char c = s[pos];
			if (c == '{')
			{
				pos++;
				Dictionary<string, object> obj = new Dictionary<string, object>();
				SkipWhitespace(s, ref pos);
				if (pos < s.Length && s[pos] == '}')
				{
					pos++;
					return obj;
				}
				while (true)
				{
					SkipWhitespace(s, ref pos);
					string key = ParseString(s, ref pos);
					SkipWhitespace(s, ref pos);
					Expect(s, ref pos, ':');
					obj[key] = ParseValue(s, ref pos, depth + 1);
					SkipWhitespace(s, ref pos);
					if (pos < s.Length && s[pos] == ',')
					{
						pos++;
						continue;
					}
					Expect(s, ref pos, '}');
					return obj;
				}
			}
			if (c == '[')
			{
				pos++;
				List<object> list = new List<object>();
				SkipWhitespace(s, ref pos);
				if (pos < s.Length && s[pos] == ']')
				{
					pos++;
					return list;
				}
				while (true)
				{
					list.Add(ParseValue(s, ref pos, depth + 1));
					SkipWhitespace(s, ref pos);
					if (pos < s.Length && s[pos] == ',')
					{
						pos++;
						continue;
					}
					Expect(s, ref pos, ']');
					return list;
				}
			}
			if (c == '"')
			{
				return ParseString(s, ref pos);
			}
			if (Match(s, ref pos, "true"))
			{
				return true;
			}
			if (Match(s, ref pos, "false"))
			{
				return false;
			}
			if (Match(s, ref pos, "null"))
			{
				return null;
			}
			return ParseNumber(s, ref pos);
		}

		private static bool Match(string s, ref int pos, string word)
		{
			if (string.CompareOrdinal(s, pos, word, 0, word.Length) == 0)
			{
				pos += word.Length;
				return true;
			}
			return false;
		}

		private static void Expect(string s, ref int pos, char c)
		{
			if (pos >= s.Length || s[pos] != c)
			{
				throw new FormatException("expected " + c);
			}
			pos++;
		}

		private static object ParseNumber(string s, ref int pos)
		{
			int start = pos;
			if (pos < s.Length && (s[pos] == '-' || s[pos] == '+'))
			{
				pos++;
			}
			bool isFloat = false;
			while (pos < s.Length)
			{
				char c = s[pos];
				if (c >= '0' && c <= '9')
				{
					pos++;
				}
				else if (c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-')
				{
					isFloat = true;
					pos++;
				}
				else
				{
					break;
				}
			}
			string text = s.Substring(start, pos - start);
			if (text.Length == 0)
			{
				throw new FormatException("bad value");
			}
			if (!isFloat)
			{
				long l;
				if (long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out l))
				{
					return l;
				}
			}
			double d;
			if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
			{
				return d;
			}
			throw new FormatException("bad number");
		}

		private static string ParseString(string s, ref int pos)
		{
			Expect(s, ref pos, '"');
			StringBuilder sb = new StringBuilder();
			while (pos < s.Length)
			{
				char c = s[pos++];
				if (c == '"')
				{
					return sb.ToString();
				}
				if (c != '\\')
				{
					sb.Append(c);
					continue;
				}
				if (pos >= s.Length)
				{
					break;
				}
				char e = s[pos++];
				switch (e)
				{
				case '"':
					sb.Append('"');
					break;
				case '\\':
					sb.Append('\\');
					break;
				case '/':
					sb.Append('/');
					break;
				case 'b':
					sb.Append('\b');
					break;
				case 'f':
					sb.Append('\f');
					break;
				case 'n':
					sb.Append('\n');
					break;
				case 'r':
					sb.Append('\r');
					break;
				case 't':
					sb.Append('\t');
					break;
				case 'u':
					if (pos + 4 > s.Length)
					{
						throw new FormatException("bad escape");
					}
					sb.Append((char)int.Parse(s.Substring(pos, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
					pos += 4;
					break;
				default:
					throw new FormatException("bad escape");
				}
			}
			throw new FormatException("unterminated string");
		}

		public static string Serialize(object value)
		{
			StringBuilder sb = new StringBuilder();
			Write(sb, value);
			return sb.ToString();
		}

		private static void Write(StringBuilder sb, object value)
		{
			if (value == null)
			{
				sb.Append("null");
			}
			else if (value is LanRawJson)
			{
				sb.Append(((LanRawJson)value).Text);
			}
			else if (value is string)
			{
				WriteString(sb, (string)value);
			}
			else if (value is bool)
			{
				sb.Append(((bool)value) ? "true" : "false");
			}
			else if (value is int || value is long || value is short || value is byte)
			{
				sb.Append(Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture));
			}
			else if (value is double || value is float)
			{
				double d = Convert.ToDouble(value);
				if (double.IsNaN(d) || double.IsInfinity(d))
				{
					sb.Append("0");
				}
				else if (d == Math.Floor(d) && Math.Abs(d) < 1e15)
				{
					sb.Append(((long)d).ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
				}
			}
			else if (value is IDictionary<string, object>)
			{
				sb.Append('{');
				bool first = true;
				foreach (KeyValuePair<string, object> kv in (IDictionary<string, object>)value)
				{
					if (!first)
					{
						sb.Append(',');
					}
					first = false;
					WriteString(sb, kv.Key);
					sb.Append(':');
					Write(sb, kv.Value);
				}
				sb.Append('}');
			}
			else if (value is System.Collections.IEnumerable)
			{
				sb.Append('[');
				bool first = true;
				foreach (object item in (System.Collections.IEnumerable)value)
				{
					if (!first)
					{
						sb.Append(',');
					}
					first = false;
					Write(sb, item);
				}
				sb.Append(']');
			}
			else
			{
				WriteString(sb, value.ToString());
			}
		}

		public static void WriteString(StringBuilder sb, string s)
		{
			sb.Append('"');
			foreach (char c in s)
			{
				switch (c)
				{
				case '"':
					sb.Append("\\\"");
					break;
				case '\\':
					sb.Append("\\\\");
					break;
				case '\n':
					sb.Append("\\n");
					break;
				case '\r':
					sb.Append("\\r");
					break;
				case '\t':
					sb.Append("\\t");
					break;
				case '\b':
					sb.Append("\\b");
					break;
				case '\f':
					sb.Append("\\f");
					break;
				default:
					if (c < 0x20)
					{
						sb.Append("\\u").Append(((int)c).ToString("x4"));
					}
					else
					{
						sb.Append(c);
					}
					break;
				}
			}
			sb.Append('"');
		}

		public static string Quote(string s)
		{
			StringBuilder sb = new StringBuilder();
			WriteString(sb, s);
			return sb.ToString();
		}

		public static object Get(object obj, string key)
		{
			Dictionary<string, object> d = obj as Dictionary<string, object>;
			object v;
			if (d != null && d.TryGetValue(key, out v))
			{
				return v;
			}
			return null;
		}

		public static object Path(object obj, params string[] keys)
		{
			object cur = obj;
			foreach (string k in keys)
			{
				cur = Get(cur, k);
				if (cur == null)
				{
					return null;
				}
			}
			return cur;
		}

		public static bool TryNumber(object v, out double d)
		{
			if (v is long)
			{
				d = (long)v;
				return true;
			}
			if (v is double)
			{
				d = (double)v;
				return !double.IsNaN(d) && !double.IsInfinity(d);
			}
			if (v is int)
			{
				d = (int)v;
				return true;
			}
			d = 0;
			return false;
		}

		public static bool TryLong(object v, out long l)
		{
			if (v is long)
			{
				l = (long)v;
				return true;
			}
			if (v is int)
			{
				l = (int)v;
				return true;
			}
			if (v is double)
			{
				double d = (double)v;
				if (d == Math.Floor(d) && Math.Abs(d) < 9.2e18)
				{
					l = (long)d;
					return true;
				}
			}
			l = 0;
			return false;
		}

		public static bool TryInt(object v, out int i)
		{
			long l;
			if (TryLong(v, out l) && l >= int.MinValue && l <= int.MaxValue)
			{
				i = (int)l;
				return true;
			}
			i = 0;
			return false;
		}

		public static double Number(object v, double fallback)
		{
			double d;
			return TryNumber(v, out d) ? d : fallback;
		}

		public static string ExtractRaw(string text, string key)
		{
			string marker = "\"" + key + "\"";
			int idx = text.IndexOf(marker, StringComparison.Ordinal);
			if (idx < 0)
			{
				return null;
			}
			int pos = idx + marker.Length;
			while (pos < text.Length && (text[pos] == ' ' || text[pos] == ':'))
			{
				pos++;
			}
			if (pos >= text.Length)
			{
				return null;
			}
			char first = text[pos];
			if (first != '{' && first != '[')
			{
				int e = pos;
				while (e < text.Length && ",}]".IndexOf(text[e]) < 0)
				{
					e++;
				}
				return text.Substring(pos, e - pos).Trim();
			}
			char close = (first == '{') ? '}' : ']';
			int depth = 0;
			int end = pos;
			for (; end < text.Length; end++)
			{
				if (text[end] == first)
				{
					depth++;
				}
				else if (text[end] == close)
				{
					depth--;
					if (depth == 0)
					{
						end++;
						break;
					}
				}
			}
			return text.Substring(pos, end - pos);
		}
	}
}
#endif
