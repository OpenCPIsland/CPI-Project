using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Disney.Kelowna.Common
{
	public sealed class AddressableAssetRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		private readonly AsyncOperationHandle<TAsset> handle;

		public override TAsset Asset
		{
			get
			{
				return handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
			}
		}

		public AddressableAssetRequest(string key, AsyncOperationHandle<TAsset> handle)
			: base(key, new AddressableOperationReturn(handle))
		{
			this.handle = handle;
		}
	}

	public sealed class AddressableDependencyRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		public override TAsset Asset
		{
			get
			{
				return null;
			}
		}

		public AddressableDependencyRequest(string key, AsyncOperationHandle handle)
			: base(key, new AddressableOperationReturn(handle))
		{
		}
	}

	internal sealed class AddressableOperationReturn : CoroutineReturn
	{
		private readonly AsyncOperationHandle handle;

		public override bool Finished
		{
			get
			{
				return handle.IsDone;
			}
		}

		public AddressableOperationReturn(AsyncOperationHandle handle)
		{
			this.handle = handle;
		}
	}

	public class AsyncAssetBundleRequest<TAsset> : AbstractAsyncAssetRequest<TAsset, AssetBundleRequest> where TAsset : class
	{
		public override TAsset Asset
		{
			get
			{
				return (base.Request != null) ? ((TAsset)(object)base.Request.asset) : null;
			}
		}

		public AsyncAssetBundleRequest(string key, AssetBundleRequest request)
			: base(key, request)
		{
		}
	}
}
