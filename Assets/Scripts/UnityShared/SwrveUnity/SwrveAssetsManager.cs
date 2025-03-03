using SwrveUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SwrveUnity
{
	public class SwrveAssetsManager : ISwrveAssetsManager
	{
		private MonoBehaviour Container;

		private string SwrveTemporaryPath;

		public string CdnImages
		{
			get;
			set;
		}

		public string CdnFonts
		{
			get;
			set;
		}

		public HashSet<string> AssetsOnDisk
		{
			get;
			set;
		}

		public SwrveAssetsManager(MonoBehaviour container, string swrveTemporaryPath)
		{
			Container = container;
			SwrveTemporaryPath = swrveTemporaryPath;
			AssetsOnDisk = new HashSet<string>();
		}

		public IEnumerator DownloadAssets(HashSet<SwrveAssetsQueueItem> assetsQueue, Action callBack)
		{
			yield return StartTask("SwrveAssetsManager.DownloadAssetQueue", DownloadAssetQueue(assetsQueue));
			if (callBack != null)
			{
				callBack();
			}
			TaskFinished("SwrveAssetsManager.DownloadAssets");
		}

		private IEnumerator DownloadAssetQueue(HashSet<SwrveAssetsQueueItem> assetsQueue)
		{
			IEnumerator<SwrveAssetsQueueItem> enumerator = assetsQueue.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SwrveAssetsQueueItem item = enumerator.Current;
				if (!CheckAsset(item.Name))
				{
					yield return StartTask("SwrveAssetsManager.DownloadAsset", DownloadAsset(item));
				}
				else
				{
					AssetsOnDisk.Add(item.Name);
				}
			}
			TaskFinished("SwrveAssetsManager.DownloadAssetQueue");
		}

		protected virtual IEnumerator DownloadAsset(SwrveAssetsQueueItem item)
		{
			string cdn = item.IsImage ? CdnImages : CdnFonts;
			string url = cdn + item.Name;
			SwrveLog.Log("Downloading asset: " + url);
			WWW www = new WWW(url);
			yield return www;
			WwwDeducedError err = UnityWwwHelper.DeduceWwwError(www);
			if (www != null && err == WwwDeducedError.NoError && www.isDone)
			{
				if (item.IsImage)
				{
					SaveImageAsset(item, www);
				}
				else
				{
					SaveBinaryAsset(item, www);
				}
			}
			TaskFinished("SwrveAssetsManager.DownloadAsset");
		}

		private bool CheckAsset(string fileName)
		{
			if (CrossPlatformFile.Exists(GetTemporaryPathFileName(fileName)))
			{
				return true;
			}
			return false;
		}

		private string GetTemporaryPathFileName(string fileName)
		{
			return Path.Combine(SwrveTemporaryPath, fileName);
		}

		protected virtual void SaveImageAsset(SwrveAssetsQueueItem item, WWW www)
		{
			Texture2D texture = www.texture;
			if (texture != null)
			{
				byte[] bytes = www.bytes;
				string text = SwrveHelper.sha1(bytes);
				if (text == item.Digest)
				{
					byte[] bytes2 = texture.EncodeToPNG();
					string temporaryPathFileName = GetTemporaryPathFileName(item.Name);
					SwrveLog.Log("Saving to " + temporaryPathFileName);
					CrossPlatformFile.SaveBytes(temporaryPathFileName, bytes2);
					bytes2 = null;
					UnityEngine.Object.Destroy(texture);
					AssetsOnDisk.Add(item.Name);
				}
				else
				{
					SwrveLog.Log("Error downloading image assetItem:" + item.Name + ". Did not match digest:" + text);
				}
			}
		}

		protected virtual void SaveBinaryAsset(SwrveAssetsQueueItem item, WWW www)
		{
			byte[] bytes = www.bytes;
			string text = SwrveHelper.sha1(bytes);
			if (text == item.Digest)
			{
				string temporaryPathFileName = GetTemporaryPathFileName(item.Name);
				SwrveLog.Log("Saving to " + temporaryPathFileName);
				CrossPlatformFile.SaveBytes(temporaryPathFileName, bytes);
				bytes = null;
				AssetsOnDisk.Add(item.Name);
			}
			else
			{
				SwrveLog.Log("Error downloading binary assetItem:" + item.Name + ". Did not match digest:" + text);
			}
		}

		protected virtual Coroutine StartTask(string tag, IEnumerator task)
		{
			return Container.StartCoroutine(task);
		}

		protected virtual void TaskFinished(string tag)
		{
		}
	}
}
