namespace Disney.Kelowna.Common
{
	public class ResourceDevice : Device
	{
		public const string DEVICE_TYPE = "res";

		public override string DeviceType
		{
			get
			{
				return "res";
			}
		}

		public ResourceDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			return AsycnResourceLoader<TAsset>.Load(ref entry, handler);
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			return ResourceLoader<TAsset>.Load(ref entry);
		}
	}
}
