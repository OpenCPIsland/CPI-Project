using System;

namespace Disney.Kelowna.Common
{
	public abstract class AssetRequest<TAsset> : CompositeCoroutineReturn, IDisposable where TAsset : class
	{
		public readonly string Key;

		protected bool disposed = false;

		public abstract TAsset Asset
		{
			get;
		}

		protected AssetRequest(string key, params CoroutineReturn[] coroutineReturns)
			: base(coroutineReturns)
		{
			Key = key;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
