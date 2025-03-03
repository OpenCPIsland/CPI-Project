namespace Disney.Kelowna.Common
{
	public class IndexedAssetRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		private readonly TAsset asset;

		public override TAsset Asset
		{
			get
			{
				return asset;
			}
		}

		public override bool Finished
		{
			get
			{
				return true;
			}
		}

		public IndexedAssetRequest(string key, TAsset asset)
			: base(key, new CoroutineReturn[0])
		{
			this.asset = asset;
		}
	}
}
