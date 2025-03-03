using System;
using System.Globalization;
using System.Text;

namespace Disney.Manimal.Common.Diagnostics
{
	public class BasicLogEntryFormatter : ILogEntryFormatter
	{
		private static readonly ILogEntryFormatter _instance = new BasicLogEntryFormatter();

		public static ILogEntryFormatter Instance
		{
			get
			{
				return _instance;
			}
		}

		private BasicLogEntryFormatter()
		{
		}

		public string FormatLogEntry(LogEntry entry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = entry.Timestamp.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
			stringBuilder.Append(value).Append(" ");
			stringBuilder.Append(("[" + entry.Level.ToString().ToUpper() + "]").PadRight(8));
			stringBuilder.Append(entry.LoggerName).Append(" - ");
			if (entry.Message != null)
			{
				object message = entry.Message;
				stringBuilder.Append(message.ToString());
			}
			if (entry.FormattedMessage != null)
			{
				if (entry.Message != null)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				FormattedMessage formattedMessage = entry.FormattedMessage;
				stringBuilder.AppendFormat(formattedMessage.FormatProvider, formattedMessage.Format, formattedMessage.Args);
			}
			return stringBuilder.ToString();
		}
	}
}
