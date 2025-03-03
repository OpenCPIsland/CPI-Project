using System;
using System.Reflection;
using System.Threading;

namespace Disney.Manimal.Common.Diagnostics
{
	public class LogManager : ILogManager
	{
		private static ILoggerFactory _factory;

		private static readonly object _sync;

		public static ILoggerFactory Factory
		{
			get
			{
				if (_factory == null)
				{
					lock (_sync)
					{
						if (_factory == null)
						{
							TraceLoggerFactory factory = new TraceLoggerFactory();
							Thread.MemoryBarrier();
							_factory = factory;
						}
					}
				}
				return _factory;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				lock (_sync)
				{
					Thread.MemoryBarrier();
					_factory = value;
				}
			}
		}

		ILoggerFactory ILogManager.Factory
		{
			get
			{
				return Factory;
			}
			set
			{
				Factory = value;
			}
		}

		static LogManager()
		{
			_sync = new object();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				assembly.GetCustomAttributes(typeof(LogConfigurationAttribute), true);
			}
		}

		public static ILogger GetLogger<T>()
		{
			return Factory.GetLogger(typeof(T));
		}

		public static ILogger GetLogger(Type type)
		{
			return Factory.GetLogger(type);
		}

		public static ILogger GetLogger(string key)
		{
			return Factory.GetLogger(key);
		}

		ILogger ILogManager.GetLogger<T>()
		{
			return GetLogger<T>();
		}

		ILogger ILogManager.GetLogger(Type type)
		{
			return GetLogger(type);
		}

		ILogger ILogManager.GetLogger(string key)
		{
			return GetLogger(key);
		}
	}
}
