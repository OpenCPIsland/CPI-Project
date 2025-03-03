namespace Disney.Kelowna.Common
{
	public abstract class Device
	{
		public readonly DeviceManager DeviceManager;

		public abstract string DeviceType
		{
			get;
		}

		protected Device(DeviceManager deviceManager)
		{
			DeviceManager = deviceManager;
		}

		public abstract AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null) where TAsset : class;

		public abstract TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry) where TAsset : class;
	}
}
