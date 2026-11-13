using System;
using System.Collections.Generic;

namespace Disney.MobileNetwork
{
	public static class Service
	{
		private static List<IServiceWrapper> serviceWrapperList;

		public static IEnumerable<object> AllServices
		{
			get
			{
				foreach (IServiceWrapper wrapper in serviceWrapperList)
				{
					yield return wrapper.Instance;
				}
			}
		}

		public static void Set<T>(T instance)
		{
			if (ServiceWrapper<T>.instance != null)
			{
				throw new InvalidOperationException("An instance of this service class has already been set!");
			}
			ServiceWrapper<T>.instance = instance;
			if (serviceWrapperList == null)
			{
				serviceWrapperList = new List<IServiceWrapper>();
			}
			serviceWrapperList.Add(new ServiceWrapper<T>());
		}

		public static T Get<T>()
		{
			if (!IsSet<T>())
			{
				throw new Exception(string.Format("Cannot find Service: {0}", typeof(T).FullName));
			}
			return ServiceWrapper<T>.instance;
		}

		public static bool IsSet<T>()
		{
			return ServiceWrapper<T>.instance != null;
		}

		public static void ResetAll()
		{
			if (serviceWrapperList != null)
			{
				for (int num = serviceWrapperList.Count - 1; num >= 0; num--)
				{
					serviceWrapperList[num].Unset();
				}
				serviceWrapperList.Clear();
			}
		}
	}
}
