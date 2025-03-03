namespace Disney.Kelowna.Common
{
	public class CPipeManifestResponse
	{
		public string FullAssetUrl
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public void Clear()
		{
			FullAssetUrl = "";
			AssetName = "";
		}
	}
}
