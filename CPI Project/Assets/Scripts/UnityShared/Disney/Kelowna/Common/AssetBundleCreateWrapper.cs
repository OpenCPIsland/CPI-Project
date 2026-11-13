using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class AssetBundleCreateWrapper : MutableAsyncOperationReturn
	{
		public AssetBundle AssetBundle
		{
			get
			{
				return ((AssetBundleCreateRequest)MutableOperation).assetBundle;
			}
		}

		public AssetBundleCreateRequest Request
		{
			get
			{
				return (AssetBundleCreateRequest)MutableOperation;
			}
		}

		public AssetBundleCreateWrapper(AssetBundleCreateRequest request)
			: base(request)
		{
		}
	}
}
