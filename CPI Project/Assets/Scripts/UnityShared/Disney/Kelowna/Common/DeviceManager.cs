using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class DeviceManager : IDisposable
	{
		private readonly Dictionary<string, Device> deviceMap = new Dictionary<string, Device>();

		public void Dispose()
		{
			deviceMap.Clear();
		}

		public void Mount(Device device)
		{
			deviceMap.Add(device.DeviceType, device);
		}

		public void Unmount(Device device)
		{
			deviceMap.Remove(device.DeviceType);
		}

		public bool IsMounted(string deviceType)
		{
			return deviceMap.ContainsKey(deviceType);
		}

		public bool IsMounted(Device device)
		{
			return deviceMap.ContainsKey(device.DeviceType);
		}

		public Device GetDevice(string deviceType)
		{
			return deviceMap[deviceType];
		}

		public AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null) where TAsset : class
		{
			Device device;
			prepareDevice(ref deviceList, out device);
			return device.LoadAsync(deviceList, ref entry, handler);
		}

		public TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry) where TAsset : class
		{
			Device device;
			prepareDevice(ref deviceList, out device);
			return device.LoadImmediate<TAsset>(deviceList, ref entry);
		}

		private void prepareDevice(ref string deviceList, out Device device)
		{
			int num = deviceList.IndexOf(':');
			string text;
			if (num != -1)
			{
				text = deviceList.Substring(0, num);
				deviceList = deviceList.Substring(num + 1);
			}
			else
			{
				text = deviceList;
				deviceList = "";
			}
			if (!deviceMap.TryGetValue(text, out device))
			{
				throw new InvalidOperationException("The requested device is not mounted: " + text);
			}
		}
	}
}
