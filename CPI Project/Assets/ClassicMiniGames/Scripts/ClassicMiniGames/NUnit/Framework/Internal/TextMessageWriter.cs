using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Globalization;

namespace NUnit.Framework.Internal
{
	public class TextMessageWriter : MessageWriter
	{
		private static readonly int DEFAULT_LINE_LENGTH = 78;

		public static readonly string Pfx_Expected = "  Expected: ";

		public static readonly string Pfx_Actual = "  But was:  ";

		public static readonly int PrefixLength = Pfx_Expected.Length;

		private static readonly string Fmt_Connector = " {0} ";

		private static readonly string Fmt_Predicate = "{0} ";

		private static readonly string Fmt_Modifier = ", {0}";

		private static readonly string Fmt_Null = "null";

		private static readonly string Fmt_EmptyString = "<string.Empty>";

		private static readonly string Fmt_EmptyCollection = "<empty>";

		private static readonly string Fmt_String = "\"{0}\"";

		private static readonly string Fmt_Char = "'{0}'";

		private static readonly string Fmt_DateTime = "yyyy-MM-dd HH:mm:ss.fff";

		private static readonly string Fmt_ValueType = "{0}";

		private static readonly string Fmt_Default = "<{0}>";

		private int maxLineLength = DEFAULT_LINE_LENGTH;

		public override int MaxLineLength
		{
			get
			{
				return maxLineLength;
			}
			set
			{
				maxLineLength = value;
			}
		}

		public TextMessageWriter()
		{
		}

		public TextMessageWriter(string userMessage, params object[] args)
		{
			if (userMessage != null && userMessage != string.Empty)
			{
				WriteMessageLine(userMessage, args);
			}
		}

		public override void WriteMessageLine(int level, string message, params object[] args)
		{
			if (message != null)
			{
				while (level-- >= 0)
				{
					Write("  ");
				}
				if (args != null && args.Length > 0)
				{
					message = string.Format(message, args);
				}
				WriteLine(message);
			}
		}

		public override void DisplayDifferences(Constraint constraint)
		{
			WriteExpectedLine(constraint);
			WriteActualLine(constraint);
		}

		public override void DisplayDifferences(object expected, object actual)
		{
			WriteExpectedLine(expected);
			WriteActualLine(actual);
		}

		public override void DisplayDifferences(object expected, object actual, Tolerance tolerance)
		{
			WriteExpectedLine(expected, tolerance);
			WriteActualLine(actual);
		}

		public override void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping)
		{
			int maxDisplayLength = MaxLineLength - PrefixLength - 2;
			if (clipping)
			{
				MsgUtils.ClipExpectedAndActual(ref expected, ref actual, maxDisplayLength, mismatch);
			}
			expected = MsgUtils.EscapeControlChars(expected);
			actual = MsgUtils.EscapeControlChars(actual);
			mismatch = MsgUtils.FindMismatchPosition(expected, actual, 0, ignoreCase);
			Write(Pfx_Expected);
			WriteExpectedValue(expected);
			if (ignoreCase)
			{
				WriteModifier("ignoring case");
			}
			WriteLine();
			WriteActualLine(actual);
			if (mismatch >= 0)
			{
				WriteCaretLine(mismatch);
			}
		}

		public override void WriteConnector(string connector)
		{
			Write(Fmt_Connector, connector);
		}

		public override void WritePredicate(string predicate)
		{
			Write(Fmt_Predicate, predicate);
		}

		public override void WriteModifier(string modifier)
		{
			Write(Fmt_Modifier, modifier);
		}

		public override void WriteExpectedValue(object expected)
		{
			WriteValue(expected);
		}

		public override void WriteActualValue(object actual)
		{
			WriteValue(actual);
		}

		public override void WriteValue(object val)
		{
			if (val == null)
			{
				Write(Fmt_Null);
			}
			else if (val.GetType().IsArray)
			{
				WriteArray((Array)val);
			}
			else if (val is string)
			{
				WriteString((string)val);
			}
			else if (val is IEnumerable)
			{
				WriteCollectionElements((IEnumerable)val, 0, 10);
			}
			else if (val is char)
			{
				WriteChar((char)val);
			}
			else if (val is double)
			{
				WriteDouble((double)val);
			}
			else if (val is float)
			{
				WriteFloat((float)val);
			}
			else if (val is decimal)
			{
				WriteDecimal((decimal)val);
			}
			else if (val is DateTime)
			{
				WriteDateTime((DateTime)val);
			}
			else if (val.GetType().IsValueType)
			{
				Write(Fmt_ValueType, val);
			}
			else
			{
				Write(Fmt_Default, val);
			}
		}

		public override void WriteCollectionElements(IEnumerable collection, int start, int max)
		{
			int num = 0;
			int num2 = 0;
			foreach (object item in collection)
			{
				if (num2++ >= start)
				{
					if (++num > max)
					{
						break;
					}
					Write((num == 1) ? "< " : ", ");
					WriteValue(item);
				}
			}
			if (num == 0)
			{
				Write(Fmt_EmptyCollection);
				return;
			}
			if (num > max)
			{
				Write("...");
			}
			Write(" >");
		}

		private void WriteArray(Array array)
		{
			if (array.Length == 0)
			{
				Write(Fmt_EmptyCollection);
				return;
			}
			int rank = array.Rank;
			int[] array2 = new int[rank];
			int num = 1;
			int num2 = rank;
			while (--num2 >= 0)
			{
				num = (array2[num2] = num * array.GetLength(num2));
			}
			int num3 = 0;
			foreach (object item in array)
			{
				if (num3 > 0)
				{
					Write(", ");
				}
				bool flag = false;
				for (num2 = 0; num2 < rank; num2++)
				{
					flag = (flag || num3 % array2[num2] == 0);
					if (flag)
					{
						Write("< ");
					}
				}
				WriteValue(item);
				num3++;
				bool flag2 = false;
				for (num2 = 0; num2 < rank; num2++)
				{
					flag2 = (flag2 || num3 % array2[num2] == 0);
					if (flag2)
					{
						Write(" >");
					}
				}
			}
		}

		private void WriteString(string s)
		{
			if (s == string.Empty)
			{
				Write(Fmt_EmptyString);
			}
			else
			{
				Write(Fmt_String, s);
			}
		}

		private void WriteChar(char c)
		{
			Write(Fmt_Char, c);
		}

		private void WriteDouble(double d)
		{
			if (double.IsNaN(d) || double.IsInfinity(d))
			{
				Write(d);
				return;
			}
			string text = d.ToString("G17", CultureInfo.InvariantCulture);
			if (text.IndexOf('.') > 0)
			{
				Write(text + "d");
			}
			else
			{
				Write(text + ".0d");
			}
		}

		private void WriteFloat(float f)
		{
			if (float.IsNaN(f) || float.IsInfinity(f))
			{
				Write(f);
				return;
			}
			string text = f.ToString("G9", CultureInfo.InvariantCulture);
			if (text.IndexOf('.') > 0)
			{
				Write(text + "f");
			}
			else
			{
				Write(text + ".0f");
			}
		}

		private void WriteDecimal(decimal d)
		{
			Write(d.ToString("G29", CultureInfo.InvariantCulture) + "m");
		}

		private void WriteDateTime(DateTime dt)
		{
			Write(dt.ToString(Fmt_DateTime, CultureInfo.InvariantCulture));
		}

		private void WriteExpectedLine(Constraint constraint)
		{
			Write(Pfx_Expected);
			constraint.WriteDescriptionTo(this);
			WriteLine();
		}

		private void WriteExpectedLine(object expected)
		{
			WriteExpectedLine(expected, null);
		}

		private void WriteExpectedLine(object expected, Tolerance tolerance)
		{
			Write(Pfx_Expected);
			WriteExpectedValue(expected);
			if (tolerance != null && !tolerance.IsEmpty)
			{
				WriteConnector("+/-");
				WriteExpectedValue(tolerance.Value);
				if (tolerance.Mode != ToleranceMode.Linear)
				{
					Write(" {0}", tolerance.Mode);
				}
			}
			WriteLine();
		}

		private void WriteActualLine(Constraint constraint)
		{
			Write(Pfx_Actual);
			constraint.WriteActualValueTo(this);
			WriteLine();
		}

		private void WriteActualLine(object actual)
		{
			Write(Pfx_Actual);
			WriteActualValue(actual);
			WriteLine();
		}

		private void WriteCaretLine(int mismatch)
		{
			WriteLine("  {0}^", new string('-', PrefixLength + mismatch - 2 + 1));
		}
	}
}
