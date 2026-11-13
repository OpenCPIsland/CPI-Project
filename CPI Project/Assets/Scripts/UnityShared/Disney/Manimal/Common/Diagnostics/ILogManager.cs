using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public interface ILogManager
	{
		ILoggerFactory Factory
		{
			get;
			set;
		}

		ILogger GetLogger<T>();

		ILogger GetLogger(Type type);

		ILogger GetLogger(string key);
	}
}
