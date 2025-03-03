using UnityEngine;

namespace Disney.Kelowna.Common
{
	public abstract class AbstractAsyncAssetRequest<TAsset, TRequest> : AssetRequest<TAsset> where TAsset : class where TRequest : AsyncOperation
	{
		public TRequest Request
		{
			get
			{
				return (TRequest)((MutableAsyncOperationReturn)coroutineReturns[0]).MutableOperation;
			}
			set
			{
				((MutableAsyncOperationReturn)coroutineReturns[0]).MutableOperation = value;
			}
		}

		protected AbstractAsyncAssetRequest(string key, TRequest operation)
			: base(key, new CoroutineReturn[1]
			{
				new MutableAsyncOperationReturn(operation)
			})
		{
		}
	}
}
