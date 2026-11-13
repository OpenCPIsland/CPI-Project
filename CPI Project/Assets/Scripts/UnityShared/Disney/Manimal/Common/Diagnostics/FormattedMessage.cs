using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public class FormattedMessage
	{
		private readonly IFormatProvider _formatProvider;

		private readonly string _format;

		private readonly object[] _args;

		public IFormatProvider FormatProvider
		{
			get
			{
				return _formatProvider;
			}
		}

		public string Format
		{
			get
			{
				return _format;
			}
		}

		public object[] Args
		{
			get
			{
				return _args;
			}
		}

		public FormattedMessage(string format, params object[] args)
			: this(null, format, args)
		{
		}

		public FormattedMessage(IFormatProvider formatProvider, string format, params object[] args)
		{
			_formatProvider = formatProvider;
			_format = format;
			_args = args;
		}
	}
}
