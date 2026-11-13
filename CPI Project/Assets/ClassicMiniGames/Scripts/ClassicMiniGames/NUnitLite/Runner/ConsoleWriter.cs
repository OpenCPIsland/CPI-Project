using System;
using System.IO;
using System.Text;

namespace NUnitLite.Runner
{
	public class ConsoleWriter : TextWriter
	{
		private static TextWriter writer;

		public static TextWriter Out
		{
			get
			{
				if (writer == null)
				{
					writer = new ConsoleWriter();
				}
				return writer;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		public override void Write(char value)
		{
			Console.Write(value);
		}

		public override void Write(string value)
		{
			Console.Write(value);
		}

		public override void WriteLine(string value)
		{
			Console.WriteLine(value);
		}
	}
}
