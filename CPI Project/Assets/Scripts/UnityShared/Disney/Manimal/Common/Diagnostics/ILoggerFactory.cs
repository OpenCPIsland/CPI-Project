using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public interface ILoggerFactory
	{
		ILogger GetLogger(Type type);

		ILogger GetLogger(string key);
	}
}
