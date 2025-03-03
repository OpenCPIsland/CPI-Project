using System;
using System.Threading;

namespace Disney.Manimal.Async
{
	public static class ThreadUtils
	{
		public static AsyncCallback SyncContextCallback(SynchronizationContext syncContext, AsyncCallback callback)
		{
			if (syncContext == null)
			{
				return callback;
			}
			return delegate(IAsyncResult asyncResult)
			{
				syncContext.Post(delegate(object result)
				{
					callback((IAsyncResult)result);
				}, asyncResult);
			};
		}

		public static Action<T> SyncContextCallback<T>(SynchronizationContext syncContext, Action<T> callback)
		{
			if (syncContext == null)
			{
				return callback;
			}
			return delegate(T asyncResult)
			{
				syncContext.Post(delegate(object result)
				{
					callback((T)result);
				}, asyncResult);
			};
		}
	}
}
