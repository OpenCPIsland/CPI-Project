using DisneyMobile.CoreUnitySystems.Utility;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	[Serializable]
	public class AppResource
	{
		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public static FileLocation ResourceSaveLocation = FileLocation.CACHE;

		public string assetName = "";

		public string assetFileName = "";

		public string assetVersion;

		public bool isInstalled = false;

		public string description = "";

		public int localCatalogVersion = -1;

		public int serverCatalogVersion = -1;

		public string expirationDate = "";

		public bool isText = false;

		public bool isRequired;

		public bool isAmps = false;

		public bool createLocalCache = true;

		public int expirationTime = 0;

		public string url;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public WWW theWWW = null;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public bool inDownloadQueue = false;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public bool wasInDownloadQueue = false;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public bool mIsLoading = false;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public bool SaveCache = true;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		private List<Asset> mAssetList = new List<Asset>();

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public Asset ResourceRequest = null;

		public AppResource()
		{
		}

		public AppResource(string inassetName, bool isrequired, string inassetVersion, string indescription, int inserverCatalogVersion)
		{
			assetName = inassetName;
			assetFileName = GetNextCacheName() + ".bin";
			assetVersion = inassetVersion;
			description = indescription;
			serverCatalogVersion = inserverCatalogVersion;
		}

		public AppResource(string inassetName, int cachehours, ResourceEventHandler inCallback, ResourceAssetType inType, string tag)
		{
			isText = (inType == ResourceAssetType.STRING);
			isRequired = false;
			createLocalCache = (cachehours > 0);
			expirationTime = cachehours;
			ResetExpirationDate();
			serverCatalogVersion = 1;
			assetName = inassetName;
			assetFileName = GetNextCacheName() + "." + FileManager.GetExtension(assetName, ".bin");
			url = inassetName;
			ResourceRequest = new Asset(inassetName, inCallback, inType, tag);
		}

		private void ResetExpirationDate()
		{
			if (expirationTime > 0)
			{
				DateTime date = DateTime.UtcNow.AddHours(expirationTime);
				expirationDate = Utilities.DateTimeToString(date);
			}
		}

		public void AddAsset(string inName, ResourceEventHandler inCallback, ResourceAssetType intype, string tag)
		{
			Asset asset = FindAsset(inName);
			if (asset != null)
			{
				asset.AddCallback(inCallback);
			}
			else
			{
				mAssetList.Add(new Asset(inName, inCallback, intype, tag));
			}
		}

		public void ProcessAssetsInBundle(bool success, AssetBundle bundle)
		{
			foreach (Asset mAsset in mAssetList)
			{
				mAsset.ProcessResult(success, bundle);
			}
		}

		private string GetNextCacheName()
		{
			int @int = PlayerPrefs.GetInt("__CacheName__", 0);
			@int++;
			PlayerPrefs.SetInt("__CacheName__", @int);
			return "Cache_" + @int;
		}

		public bool HasNewer()
		{
			return serverCatalogVersion > localCatalogVersion;
		}

		public bool IsDownloading()
		{
			return mIsLoading;
		}

		public bool Expired()
		{
			bool result = false;
			if (!isAmps && expirationTime > 0)
			{
				if (!string.IsNullOrEmpty(expirationDate))
				{
					Logger.LogWarning(this, "expirationDate should have been set!!");
				}
				else
				{
					DateTime t = Utilities.StringToDateTime(expirationDate, true);
					result = (DateTime.UtcNow > t);
				}
			}
			return result;
		}

		public bool ShouldSaveToCatalog()
		{
			return isAmps || createLocalCache;
		}

		public float GetProgress()
		{
			if (theWWW != null)
			{
				return theWWW.progress;
			}
			if (localCatalogVersion == serverCatalogVersion)
			{
				return 1f;
			}
			return 0f;
		}

		public static ResourceAssetType GetResourceTypeByFileName(string fname)
		{
			string extension = FileManager.GetExtension(fname, "");
			ResourceAssetType result = ResourceAssetType.NONE;
			switch (extension)
			{
			case "assetbundle":
				result = ResourceAssetType.ASSET_BUNDLE;
				break;
			case "unity3d":
				result = ResourceAssetType.ASSET_BUNDLE;
				break;
			case "txt":
			case "json":
			case "xml":
				result = ResourceAssetType.STRING;
				break;
			case "jpg":
			case "png":
				result = ResourceAssetType.IMAGE;
				break;
			case "ogg":
				result = ResourceAssetType.AUDIO;
				break;
			}
			return result;
		}

		public bool ProcessLoadFromFile()
		{
			bool result = false;
			if (GetResourceTypeByFileName(assetFileName) == ResourceAssetType.ASSET_BUNDLE)
			{
				AssetBundle bundle = AssetBundle.LoadFromFile(FileManager.GetFullPath(ResourceSaveLocation, assetFileName));
				FinishWithBundle(bundle, "Load from file");
				result = true;
			}
			else if (ResourceRequest != null)
			{
				result = ResourceRequest.ProcessResult(FileManager.GetFullPath(ResourceSaveLocation, assetFileName));
			}
			return result;
		}

		public bool StartDownload()
		{
			wasInDownloadQueue = inDownloadQueue;
			inDownloadQueue = false;
			if (IsDownloading())
			{
				return false;
			}
			if (assetFileName != "" && FileManager.FileExists(ResourceSaveLocation, assetFileName) && !Expired() && !HasNewer())
			{
				if (ProcessLoadFromFile())
				{
					return true;
				}
				Logger.LogWarning(this, "Cannot load resource from file");
			}
			Logger.LogInfo(this, " Download start for " + Serialize());
			ResourceManager.Instance.StartCoroutine(StartLoadingResource(this));
			return true;
		}

		private IEnumerator StartLoadingResource(AppResource ar)
		{
			mIsLoading = true;
			ar.theWWW = new WWW(ar.url);
			yield return ar.theWWW;
			bool noErrors = ar.theWWW.isDone && ar.theWWW.error == null && ar.theWWW.bytes != null;
			mIsLoading = false;
			if (ar.theWWW.error != null)
			{
				Logger.LogWarning(this, "resource download error,  www.error=" + ar.theWWW.error);
				noErrors = false;
			}
			if (noErrors)
			{
				Logger.LogInfo(this, "Resource download completed successfully : " + ar.assetName);
				ResetExpirationDate();
				ar.SaveDownloadedData();
				if (ResourceRequest == null && mAssetList.Count == 0)
				{
					ar.Finished(true, "OK");
					yield break;
				}
				if (ResourceRequest != null && ResourceRequest.AssetType != ResourceAssetType.ASSET_BUNDLE)
				{
					ResourceRequest.ProcessResult(true, theWWW);
					ProcessAssetsInBundle(false, theWWW.assetBundle);
					ar.Finished(true, "OK");
					yield break;
				}
				AssetBundle bundle = theWWW.assetBundle;
				if (bundle != null)
				{
					ar.FinishWithBundle(bundle, "OK");
				}
				else if (theWWW.bytes != null)
				{
					AssetBundleCreateRequest acr = AssetBundle.LoadFromMemoryAsync(theWWW.bytes);
					yield return acr;
					if (acr.assetBundle == null)
					{
						Logger.LogWarning(this, "Loading asset bundle from memory failed");
					}
					ar.FinishWithBundle(acr.assetBundle, "OK");
				}
				else
				{
					ar.FinishWithBundle(null, "Bad Bundle Data");
				}
			}
			else
			{
				Logger.LogInfo(this, "Resource download failed : " + ar.assetName);
				ar.FinishWithBundle(null, "Download error");
			}
		}

		private bool SaveDownloadedData()
		{
			bool result = false;
			if (isAmps || createLocalCache)
			{
				localCatalogVersion = serverCatalogVersion;
				ResourceManager.Instance.SaveResourceCatalog();
				if (isText)
				{
					if (theWWW.text != null)
					{
						result = FileManager.SaveTextFile(ResourceSaveLocation, assetFileName, theWWW.text);
					}
					else
					{
						Logger.LogWarning(this, "No data downloaded for " + assetName);
					}
				}
				else if (theWWW.bytes != null)
				{
					result = FileManager.SaveBinaryFile(ResourceSaveLocation, assetFileName, theWWW.bytes);
				}
				else
				{
					Logger.LogWarning(this, "No data downloaded for " + assetName);
				}
			}
			return result;
		}

		private void FinishWithBundle(AssetBundle bundle, string msg)
		{
			if (ResourceRequest != null)
			{
				ResourceRequest.ProcessResult(true, bundle);
			}
			ProcessAssetsInBundle(true, bundle);
			Finished(true, msg);
		}

		public void Finished(bool success, string msg)
		{
			if (theWWW != null)
			{
				theWWW.Dispose();
				theWWW = null;
			}
			ResourceManager.Instance.OnDownloadCompleted(success, this, msg);
		}

		public void StopDownload()
		{
			if (IsDownloading())
			{
				mIsLoading = false;
				Logger.LogFatal(this, "Down paused for " + assetName);
				inDownloadQueue = wasInDownloadQueue;
				FinishWithBundle(null, "Canceled");
			}
		}

		public bool DownloadLatest()
		{
			if (HasNewer() || Expired())
			{
				StartDownload();
			}
			return false;
		}

		public void ReleaseResource()
		{
			Debug.LogError("Resource release");
			if (ResourceRequest != null)
			{
				ResourceRequest.ReleaseResource();
				ResourceRequest = null;
			}
			foreach (Asset mAsset in mAssetList)
			{
				mAsset.ReleaseResource();
			}
			mAssetList.Clear();
		}

		public bool UpdateWith(AppResource ar)
		{
			if (serverCatalogVersion != ar.serverCatalogVersion)
			{
				Logger.LogInfo(this, "diff " + serverCatalogVersion + " == " + ar.serverCatalogVersion);
				isRequired = ar.isRequired;
				assetVersion = ar.assetVersion;
				isInstalled = false;
				description = ar.description;
				serverCatalogVersion = ar.serverCatalogVersion;
				ReleaseResource();
				if (IsDownloading())
				{
					StopDownload();
					StartDownload();
				}
				Logger.LogInfo(this, "Resource Merged with change " + assetName + Serialize());
				return true;
			}
			Logger.LogInfo(this, "Resource Merged no change " + assetName);
			return false;
		}

		public string Serialize()
		{
			return JSONManager.serializeToJson(this, true);
		}

		public Asset FindAsset(string assetname)
		{
			foreach (Asset mAsset in mAssetList)
			{
				if (mAsset.AssetName == assetname)
				{
					return mAsset;
				}
			}
			return null;
		}

		public object GetLoadedAsset()
		{
			if (ResourceRequest != null)
			{
				return ResourceRequest.LoadedObject;
			}
			return null;
		}

		public object GetLoadedAsset(string assetname)
		{
			Asset asset = FindAsset(assetname);
			if (asset != null)
			{
				return asset.LoadedObject;
			}
			return null;
		}
	}
}
