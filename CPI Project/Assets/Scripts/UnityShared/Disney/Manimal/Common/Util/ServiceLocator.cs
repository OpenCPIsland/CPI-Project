using System;
using System.Collections.Generic;

namespace Disney.Manimal.Common.Util
{
	public static class ServiceLocator
	{
		private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>(30);

		public static T Get<T>()
		{
			return (T)Get(typeof(T));
		}

		private static object Get(Type type)
		{
			object obj = _services.ContainsKey(type) ? _services[type] : null;
			if (obj == null)
			{
				throw new InvalidOperationException(string.Format("Instance of {0} may not be null", type));
			}
			return obj;
		}

		public static T GetOrReturnNull<T>()
		{
			return (T)GetOrReturnNull(typeof(T));
		}

		public static object GetOrReturnNull(Type type)
		{
			return _services.ContainsKey(type) ? _services[type] : null;
		}

		public static T Set<T>(T service)
		{
			Set(typeof(T), service);
			return service;
		}

		public static object Set(Type type, object service)
		{
			_services[type] = service;
			return service;
		}
	}
}
