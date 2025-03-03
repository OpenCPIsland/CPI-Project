using System;
using System.Text;

namespace Disney.Kelowna.Common
{
	public class MessageBuilder
	{
		private readonly StringBuilder str = new StringBuilder();

		private int indentLevel;

		public int IndentLevel
		{
			get
			{
				return indentLevel;
			}
		}

		public MessageBuilder PushIndent(uint indentCount = 1u)
		{
			indentLevel += (int)indentCount;
			return this;
		}

		public MessageBuilder PopIndent(uint indentCount = 1u)
		{
			indentLevel = Math.Max(0, indentLevel - (int)indentCount);
			return this;
		}

		public MessageBuilder Indent()
		{
			return Append("".PadRight(indentLevel, '\t'));
		}

		public MessageBuilder Append(string format, params string[] args)
		{
			return Append(string.Format(format, args));
		}

		public MessageBuilder Append(string msg)
		{
			str.Append(msg);
			return this;
		}

		public MessageBuilder AppendLine(string format, params string[] args)
		{
			return AppendLine(string.Format(format, args));
		}

		public MessageBuilder AppendLine(string msg)
		{
			Indent();
			str.AppendLine(msg);
			return this;
		}

		public TableBuilder AppendTable()
		{
			return new TableBuilder(this);
		}

		public string GetMessage()
		{
			return str.ToString();
		}
	}
}
