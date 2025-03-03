using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ClubPenguin.Kelowna.Common.ImageCache
{
	public class ImageCache
	{
		private const string DEFAULT_LOCATION = "ImageCache";

		private const string MANIFEST_NAME = "cacheManifest.dat";

		private const int DEFAULT_MAX_SIZE = 51200000;

		private const string PLAYERPREFS_CLEARED_KEY = "cp.ImageCache.ClearedVersion";

		private const string DEFAULT_CLEARED_VALUE = "1.0";

		private List<DImageCache> manifest;

		private Dictionary<string, Texture2D> sessionCachedImages;

		private string savedImagesLocation;

		private int cacheSizeByteLimit;

		private int currentCacheSize;

		private CacheableType<string> lastClearedVersion;

		public ImageCache()
		{
			manifest = new List<DImageCache>();
			sessionCachedImages = new Dictionary<string, Texture2D>();
		}

		public void Init(int cacheSizeByteLimit = 51200000)
		{
			this.cacheSizeByteLimit = cacheSizeByteLimit;
			try
			{
				loadManifest();
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
				Log.LogErrorFormatted(this, "An error occured trying to load the manifest. Message: {0}", ex.Message);
			}
		}

		public void CheckAndClearCache(string currentVersion)
		{
			lastClearedVersion = new CacheableType<string>("cp.ImageCache.ClearedVersion", "1.0");
			if (!currentVersion.Equals(lastClearedVersion.Value))
			{
				ClearImageCache();
				lastClearedVersion.Value = currentVersion;
			}
		}

		public void ClearSessionCachedImages()
		{
			foreach (KeyValuePair<string, Texture2D> sessionCachedImage in sessionCachedImages)
			{
				UnityEngine.Object.Destroy(sessionCachedImage.Value);
			}
			sessionCachedImages.Clear();
		}

		public bool ContainsImage(string hashName)
		{
			if (string.IsNullOrEmpty(hashName))
			{
				return false;
			}
			if (!Directory.Exists(GetSavedImagesLocation()))
			{
				return false;
			}
			if (sessionCachedImages.ContainsKey(hashName))
			{
				return true;
			}
			return manifest.Exists((DImageCache data) => data.Hashname.Equals(hashName)) && File.Exists(PathUtil.Combine(GetSavedImagesLocation(), hashName));
		}

		public Texture2D GetTextureFromCache(string hashName)
		{
			if (sessionCachedImages.ContainsKey(hashName))
			{
				return sessionCachedImages[hashName];
			}
			if (ContainsImage(hashName))
			{
				string fullPath = PathUtil.Combine(GetSavedImagesLocation(), hashName);
				byte[] data = readFromDisk(fullPath);
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(data);
				sessionCachedImages.Add(hashName, texture2D);
				return texture2D;
			}
			throw new Exception("File does not exist. ContainsImage should be called before calling LoadTextureFromDisk()");
		}

		public void RemoveImage(string hashName)
		{
			if (ContainsImage(hashName))
			{
				DImageCache cacheItem = manifest.Find((DImageCache item) => item.Hashname.Equals(hashName));
				removeImageFromCache(cacheItem);
			}
		}

		public void SaveTextureToCache(string hashName, Texture2D texture)
		{
			byte[] array = texture.EncodeToPNG();
			if (array.Length >= cacheSizeByteLimit)
			{
				throw new InsufficientMemoryException("Attempting to save a texture that is larger than the cache size limit.Size of limit (KB): " + cacheSizeByteLimit + ". Size of texture (Bytes): " + array.Length);
			}
			if (ContainsImage(hashName))
			{
				DImageCache dImageCache = manifest.Find((DImageCache x) => x.Hashname.Equals(hashName));
				currentCacheSize -= dImageCache.Filesize;
				dImageCache.Filesize = array.Length;
				currentCacheSize += dImageCache.Filesize;
				checkCacheSize();
				dImageCache.Timestamp = DateTime.Now.GetTimeInMilliseconds();
				sessionCachedImages[hashName] = texture;
			}
			else
			{
				DImageCache dImageCache = default(DImageCache);
				dImageCache.Hashname = hashName;
				dImageCache.Filesize = array.Length;
				dImageCache.Timestamp = DateTime.Now.GetTimeInMilliseconds();
				manifest.Add(dImageCache);
				currentCacheSize += dImageCache.Filesize;
				checkCacheSize();
				if (!sessionCachedImages.ContainsKey(hashName))
				{
					sessionCachedImages.Add(hashName, texture);
				}
			}
			try
			{
				writeToDisk(GetSavedImagesLocation(), hashName, array);
				saveManifest();
			}
			catch (Exception ex)
			{
				Log.LogError(this, "Unable to save item to disk. Exception Message: " + ex.Message);
				Log.LogException(this, ex);
			}
		}

		public void ClearImageCache()
		{
			for (int num = manifest.Count - 1; num >= 0; num--)
			{
				removeImageFromCache(manifest[num]);
			}
			saveManifest();
		}

		private void loadManifest()
		{
			string path = PathUtil.Combine(GetSavedImagesLocation(), "cacheManifest.dat");
			if (File.Exists(path))
			{
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					using (FileStream serializationStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
					{
						manifest = (List<DImageCache>)binaryFormatter.Deserialize(serializationStream);
					}
				}
				catch (Exception ex)
				{
					manifest.Clear();
					Log.LogError(this, "Unable to load image cache manifest. Clearing cached manifest. Message: " + ex.Message);
					File.Delete(path);
				}
			}
			currentCacheSize = 0;
			for (int i = 0; i < manifest.Count; i++)
			{
				currentCacheSize += manifest[i].Filesize;
			}
			checkCacheSize();
			cacheHealthCheck();
		}

		private void cacheHealthCheck()
		{
			string path = GetSavedImagesLocation();
			if (!Directory.Exists(path))
			{
				return;
			}
			string[] files = Directory.GetFiles(path);
			foreach (string filePath in files)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
				if (!string.IsNullOrEmpty(fileNameWithoutExtension) && !filePath.Contains("cacheManifest.dat") && !manifest.Exists((DImageCache entry) => entry.Hashname == Path.GetFileNameWithoutExtension(filePath)))
				{
					try
					{
						File.Delete(filePath);
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "Unable to delete file at path {0}. Message: {1}", filePath, ex.Message);
					}
				}
			}
			List<string> entriesToRemove = new List<string>();
			for (int j = 0; j < manifest.Count; j++)
			{
				if (!ContainsImage(manifest[j].Hashname))
				{
					entriesToRemove.Add(manifest[j].Hashname);
				}
			}
			if (entriesToRemove.Count > 0)
			{
				int i;
				for (i = 0; i < entriesToRemove.Count; i++)
				{
					removeImageFromCache(manifest.Find((DImageCache entry) => entry.Hashname == entriesToRemove[i]));
				}
				entriesToRemove.Clear();
			}
		}

		private void saveManifest()
		{
			string text = GetSavedImagesLocation();
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string path = PathUtil.Combine(text, "cacheManifest.dat");
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using (FileStream serializationStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
				{
					binaryFormatter.Serialize(serializationStream, manifest);
				}
			}
			catch (Exception ex)
			{
				Log.LogError(this, "Unable to save image cache manifest. Message: " + ex.Message);
				Log.LogException(this, ex);
			}
		}

		public string GetSavedImagesLocation()
		{
			if (string.IsNullOrEmpty(savedImagesLocation))
			{
				savedImagesLocation = PathUtil.Combine(Application.persistentDataPath, "ImageCache");
			}
			return savedImagesLocation;
		}

		private void writeToDisk(string path, string fileName, byte[] bytes)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string path2 = PathUtil.Combine(path, fileName);
			Stream stream = null;
			try
			{
				stream = new FileStream(path2, FileMode.OpenOrCreate, FileAccess.Write);
				using (BinaryWriter binaryWriter = new BinaryWriter(stream))
				{
					binaryWriter.Write(bytes);
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private byte[] readFromDisk(string fullPath)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					return binaryReader.ReadBytes((int)fileStream.Length);
				}
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Dispose();
				}
			}
		}

		private void checkCacheSize()
		{
			while (currentCacheSize > cacheSizeByteLimit)
			{
				try
				{
					removeOldestImageFromCache();
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		private void removeOldestImageFromCache()
		{
			if (manifest.Count > 0)
			{
				manifest.Sort((DImageCache a, DImageCache b) => a.Timestamp.CompareTo(b.Timestamp));
				removeImageFromCache(manifest[0]);
			}
			saveManifest();
		}

		private void removeImageFromCache(DImageCache cacheItem)
		{
			string path = PathUtil.Combine(GetSavedImagesLocation(), cacheItem.Hashname);
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Unable to delete " + cacheItem.Hashname + " from device. Message: " + ex.Message);
					Log.LogException(this, ex);
				}
			}
			if (sessionCachedImages.ContainsKey(cacheItem.Hashname))
			{
				UnityEngine.Object.Destroy(sessionCachedImages[cacheItem.Hashname]);
				sessionCachedImages.Remove(cacheItem.Hashname);
			}
			if (manifest.Count > 0 && manifest.Contains(cacheItem))
			{
				currentCacheSize -= cacheItem.Filesize;
				manifest.Remove(cacheItem);
			}
		}
	}
}
