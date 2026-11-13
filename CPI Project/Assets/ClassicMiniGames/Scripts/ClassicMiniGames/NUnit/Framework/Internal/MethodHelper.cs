using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class MethodHelper
	{
		public static string GetDisplayName(MethodInfo method, object[] arglist)
		{
			StringBuilder stringBuilder = new StringBuilder(method.Name);
			if (method.IsGenericMethod)
			{
				stringBuilder.Append("<");
				int num = 0;
				Type[] genericArguments = method.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					if (num++ > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(type.Name);
				}
				stringBuilder.Append(">");
			}
			if (arglist != null)
			{
				stringBuilder.Append("(");
				for (int j = 0; j < arglist.Length; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(GetDisplayString(arglist[j]));
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		private static string GetDisplayString(object arg)
		{
			string text = (arg == null) ? "null" : Convert.ToString(arg, CultureInfo.InvariantCulture);
			if (arg is double)
			{
				double num = (double)arg;
				if (double.IsNaN(num))
				{
					text = "double.NaN";
				}
				else if (double.IsPositiveInfinity(num))
				{
					text = "double.PositiveInfinity";
				}
				else if (double.IsNegativeInfinity(num))
				{
					text = "double.NegativeInfinity";
				}
				else if (num == double.MaxValue)
				{
					text = "double.MaxValue";
				}
				else if (num == double.MinValue)
				{
					text = "double.MinValue";
				}
				else
				{
					if (text.IndexOf('.') == -1)
					{
						text += ".0";
					}
					text += "d";
				}
			}
			else if (arg is float)
			{
				float num2 = (float)arg;
				if (float.IsNaN(num2))
				{
					text = "float.NaN";
				}
				else if (float.IsPositiveInfinity(num2))
				{
					text = "float.PositiveInfinity";
				}
				else if (float.IsNegativeInfinity(num2))
				{
					text = "float.NegativeInfinity";
				}
				else if (num2 == float.MaxValue)
				{
					text = "float.MaxValue";
				}
				else if (num2 == float.MinValue)
				{
					text = "float.MinValue";
				}
				else
				{
					if (text.IndexOf('.') == -1)
					{
						text += ".0";
					}
					text += "f";
				}
			}
			else if (arg is decimal)
			{
				decimal d = (decimal)arg;
				text = ((d == decimal.MinValue) ? "decimal.MinValue" : ((!(d == decimal.MaxValue)) ? (text + "m") : "decimal.MaxValue"));
			}
			else if (arg is long)
			{
				long num3 = (long)arg;
				text = ((num3 == long.MinValue) ? "long.MinValue" : ((num3 != long.MinValue) ? (text + "L") : "long.MaxValue"));
			}
			else if (arg is ulong)
			{
				ulong num4 = (ulong)arg;
				text = ((num4 == 0) ? "ulong.MinValue" : ((num4 != 0) ? (text + "UL") : "ulong.MaxValue"));
			}
			else if (arg is string)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("\"");
				string text2 = (string)arg;
				foreach (char c in text2)
				{
					stringBuilder.Append(EscapeControlChar(c));
				}
				stringBuilder.Append("\"");
				text = stringBuilder.ToString();
			}
			else if (arg is char)
			{
				text = "'" + EscapeControlChar((char)arg) + "'";
			}
			else if (arg is int)
			{
				switch ((int)arg)
				{
				case int.MaxValue:
					text = "int.MaxValue";
					break;
				case int.MinValue:
					text = "int.MinValue";
					break;
				}
			}
			return text;
		}

		private static string EscapeControlChar(char c)
		{
			switch (c)
			{
			case '\'':
				return "\\'";
			case '"':
				return "\\\"";
			case '\\':
				return "\\\\";
			case '\0':
				return "\\0";
			case '\a':
				return "\\a";
			case '\b':
				return "\\b";
			case '\f':
				return "\\f";
			case '\n':
				return "\\n";
			case '\r':
				return "\\r";
			case '\t':
				return "\\t";
			case '\v':
				return "\\v";
			case '\u0085':
			case '\u2028':
			case '\u2029':
				return string.Format("\\x{0:X4}", (int)c);
			default:
				return c.ToString();
			}
		}
	}
}
