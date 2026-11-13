using Disney.Manimal.Common.Diagnostics;
using System;
using System.Text;

namespace Disney.LaunchPadFramework
{
	public class LPFMUTLogEntryFormatter : ILogEntryFormatter
	{
		private static readonly ILogEntryFormatter _instance = new LPFMUTLogEntryFormatter();

		public static ILogEntryFormatter Instance
		{
			get
			{
				return _instance;
			}
		}

		private LPFMUTLogEntryFormatter()
		{
		}

		public string FormatLogEntry(LogEntry entry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[" + entry.LoggerName + "]").Append(" - ");
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
