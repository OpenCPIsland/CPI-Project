namespace Disney.Kelowna.Common
{
	public class AssetRequestWrapper<TAsset> : CoroutineReturn where TAsset : class
	{
		public AssetRequest<TAsset> MutableRequest
		{
			get;
			set;
		}

		public override bool Finished
		{
			get
			{
				return MutableRequest != null && MutableRequest.Finished;
			}
		}

		public override bool Cancelled
		{
			get
			{
				return MutableRequest != null && MutableRequest.Cancelled;
			}
		}

		public AssetRequestWrapper(AssetRequest<TAsset> request)
		{
			MutableRequest = request;
		}
	}
}
