namespace DI.Threading
{
	public static class ObjectExtension
	{
		public static Task RunAsync(this object that, string methodName, params object[] args)
		{
			return that.RunAsync<object>(methodName, null, args);
		}

		public static Task RunAsync(this object that, string methodName, TaskDistributor target, params object[] args)
		{
			return that.RunAsync<object>(methodName, target, args);
		}

		public static Task<T> RunAsync<T>(this object that, string methodName, params object[] args)
		{
			return that.RunAsync<T>(methodName, null, args);
		}

		public static Task<T> RunAsync<T>(this object that, string methodName, TaskDistributor target, params object[] args)
		{
			return Task.Create<T>(that, methodName, args).Run(target);
		}
	}
}
