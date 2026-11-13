using System;
using System.Collections;
using System.IO;

namespace Disney.Kelowna.Common
{
	public class FileBytesDevice : Device
	{
		public const string DEVICE_TYPE = "file-bytes";

		public override string DeviceType
		{
			get
			{
				return "file-bytes";
			}
		}

		public FileBytesDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForFileStreamAssetToLoad(asyncAssetRequest, entry.AssetPath, handler), this, "waitForFileStreamAssetToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForFileStreamAssetToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			FileStream fileStream = new FileStream(key, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true);
			byte[] bytes;
			try
			{
				bytes = new byte[fileStream.Length];
				IAsyncResult asyncResult = fileStream.BeginRead(bytes, 0, bytes.Length, null, null);
				while (!asyncResult.IsCompleted)
				{
					yield return null;
				}
				fileStream.EndRead(asyncResult);
			}
			finally
			{
				fileStream.Close();
			}
			assetRequest.Request = new IndexedAssetRequest<TAsset>(key, (TAsset)(object)bytes);
			yield return assetRequest;
			if (handler != null)
			{
				handler(key, (TAsset)(object)bytes);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			using (FileStream fileStream = new FileStream(entry.AssetPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, false))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					byte[] array = binaryReader.ReadBytes((int)fileStream.Length);
					return (TAsset)(object)array;
				}
			}
		}
	}
}
