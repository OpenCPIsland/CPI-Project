using DisneyMobile.CoreUnitySystems;
using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour, IConfigurable
{
	private const string CatalogFileName = "AppResourceCatalog.txt";

	public string mAMPSAppID = "TODO";

	public string mAMPSServer = "TODO";

	public string mNetworkMode = "TODO";

	public int mMaxConcurrentDownload = 3;

	public string mResourceVersion = "1";

	public bool mEnableAMPS = false;

	public DelegateAllRequiredResourceReadyCallback RequiredReadyCallback = null;

	private static ResourceManager m_instance = null;

	private static bool WifiOnly = false;

	public bool ResourceVersioncheckInProgress = false;

	public AMPVersionResultData AMPSAssetList;

	private bool m_queuePaused = false;

	public Dictionary<string, AppResource> ResourceCatalog = null;

	public List<AppResource> TestCatalog = null;

	protected static string platformLowerCase = "unknown";

	protected static string deviceModel = "unknown";

	public static ResourceManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public static event Action<bool> CatalogUpdatedEvent;

	public void Configure(IDictionary<string, object> dictionary)
	{
		AutoConfigurable.AutoConfigureObject(this, dictionary);
		WifiOnly = (mNetworkMode == "WifiOnly");
	}

	public void Reconfigure(IDictionary<string, object> dictionary)
	{
		Configure(dictionary);
	}

	private void Awake()
	{
		m_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Initialize(DelegateAllRequiredResourceReadyCallback readyCallback)
	{
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Resource manager initialized");
		RequiredReadyCallback = readyCallback;
		LoadResourceCatalog();
		if (mEnableAMPS)
		{
			DownloadAssetCatalogFromServer();
			return;
		}
		RequiredReadyCallback(true, "NOAMPS");
		RequiredReadyCallback = null;
	}

	public void DownloadAssetCatalogFromServer()
	{
		StartCoroutine(StartResourceVersionCheck());
	}

	public object LoadAssetFromBundle(string bundleName, string assetname, string tag, ResourceEventHandler callback, ResourceAssetType intype)
	{
		AppResource appResource = FindAppResource(bundleName);
		if (appResource != null)
		{
			object loadedAsset = appResource.GetLoadedAsset(assetname);
			if (loadedAsset != null)
			{
				callback(true, tag, loadedAsset);
				return loadedAsset;
			}
			appResource.AddAsset(assetname, callback, intype, tag);
			StartDownload(appResource);
		}
		else if (Utilities.IsURL(bundleName))
		{
			LoadResourceAsset(bundleName, 999999, null, ResourceAssetType.ASSET_BUNDLE, tag);
			appResource = FindAppResource(bundleName);
			if (appResource != null)
			{
				appResource.AddAsset(assetname, callback, intype, tag);
			}
		}
		else
		{
			UnityEngine.Object @object = Resources.Load(bundleName + "/" + assetname);
			if (@object != null)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Found in resource" + bundleName + "/" + assetname);
				callback(true, tag, @object);
				return @object;
			}
			DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Asset not found under Resource " + bundleName + "/" + assetname);
		}
		return null;
	}

	public object LoadResourceAsset(string resourcename, int cachehours, ResourceEventHandler callback, ResourceAssetType inType, string tag)
	{
		AppResource appResource = FindAppResource(resourcename);
		if (appResource != null)
		{
			if (!appResource.Expired())
			{
				object loadedAsset = appResource.GetLoadedAsset();
				if (loadedAsset != null)
				{
					return loadedAsset;
				}
			}
			if (appResource.ResourceRequest == null)
			{
				appResource.ResourceRequest = new Asset(resourcename, callback, inType, tag);
			}
			else
			{
				appResource.ResourceRequest.AddCallback(callback);
			}
			StartDownload(appResource);
			return null;
		}
		appResource = new AppResource(resourcename, cachehours, callback, inType, tag);
		ResourceCatalog.Add(resourcename, appResource);
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "New resource added to download list, " + resourcename);
		if (cachehours > 0)
		{
			SaveResourceCatalog();
		}
		StartDownload(appResource);
		return null;
	}

	private void LoadResourceCatalog()
	{
		string text = FileManager.LoadTextFile(AppResource.ResourceSaveLocation, "AppResourceCatalog.txt");
		if (string.IsNullOrEmpty(text))
		{
			ResourceCatalog = new Dictionary<string, AppResource>();
			DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Resource catalog is empty ");
			return;
		}
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "=============== Resource catalog = " + text);
		List<AppResource> list = TestCatalog = JSONManager.getTemplatedTypeForJson<List<AppResource>>(text);
		ResourceCatalog = new Dictionary<string, AppResource>();
		foreach (AppResource item in list)
		{
			if (item.isRequired && item.HasNewer())
			{
				item.inDownloadQueue = true;
			}
			ResourceCatalog.Add(item.assetName, item);
		}
	}

	public AppResource FindAppResource(string resourcename)
	{
		if (ResourceCatalog.ContainsKey(resourcename))
		{
			DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "found " + resourcename);
			return ResourceCatalog[resourcename];
		}
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "NOT found " + resourcename);
		return null;
	}

	public bool ServerHasNewerVersionForResourceNamed(string resourceName)
	{
		bool result = false;
		AppResource appResource = FindAppResource(resourceName);
		if (appResource != null)
		{
			result = appResource.HasNewer();
		}
		return result;
	}

	public int LatestVersionForAssetNamed(string resourceName)
	{
		int result = -1;
		AppResource appResource = FindAppResource(resourceName);
		if (appResource != null)
		{
			result = appResource.serverCatalogVersion;
		}
		return result;
	}

	public void SuspendDownloadQueue()
	{
		m_queuePaused = true;
	}

	public void ResumeDownloadQueue()
	{
		m_queuePaused = false;
		DownloadNextInQueue();
	}

	public void StopAllDownloads()
	{
		SuspendDownloadQueue();
		foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
		{
			if (item.Value.IsDownloading())
			{
				item.Value.StopDownload();
			}
		}
	}

	private string GetFullAssetBundleName(string assetBundleName)
	{
		return assetBundleName + "_" + platformLowerCase + "_" + mResourceVersion + ".assetbundle";
	}

	private string GetAMPSAssetListInfoCallURL()
	{
		string text = "";
		return string.Format("{0}/amps/api/listassets/{1}?deviceModel={2}&pageSize=100000", mAMPSServer, mAMPSAppID, deviceModel);
	}

	private bool AllRequiredDownloaded()
	{
		foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
		{
			if (item.Value.HasNewer() && item.Value.isRequired)
			{
				return false;
			}
		}
		return true;
	}

	private int CountNumberDownloads()
	{
		int num = 0;
		foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
		{
			if (item.Value.IsDownloading())
			{
				num++;
			}
		}
		return num;
	}

	private int PendingDownloads()
	{
		int num = 0;
		foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
		{
			if (item.Value.inDownloadQueue)
			{
				num++;
			}
		}
		return num;
	}

	private void DownloadNextInQueue()
	{
		if (!m_queuePaused && (!WifiOnly || (WifiOnly && Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)))
		{
			int num = CountNumberDownloads();
			if (num < mMaxConcurrentDownload)
			{
				foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
				{
					if (!item.Value.IsDownloading() && item.Value.inDownloadQueue)
					{
						item.Value.StartDownload();
						num++;
						DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Cur download = " + num);
						if (num >= mMaxConcurrentDownload)
						{
							break;
						}
					}
				}
			}
		}
	}

	private void StartDownload(AppResource ar)
	{
		if (!ar.IsDownloading() && !ar.inDownloadQueue)
		{
			if (CountNumberDownloads() >= mMaxConcurrentDownload)
			{
				ar.wasInDownloadQueue = true;
				ar.inDownloadQueue = true;
			}
			else
			{
				ar.StartDownload();
			}
		}
	}

	private void OnCatalogDownloaded(Dictionary<string, AppResource> newcatalog)
	{
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Catalog loaded from server");
		bool flag = false;
		if (newcatalog != null)
		{
			Dictionary<string, AppResource> dictionary = new Dictionary<string, AppResource>();
			foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
			{
				AppResource value = item.Value;
				if (newcatalog.ContainsKey(value.assetName))
				{
					dictionary.Add(value.assetName, value);
					flag |= value.UpdateWith(newcatalog[value.assetName]);
					if (value.HasNewer() && value.isRequired)
					{
						DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Auto download needed for " + value.assetName);
						value.inDownloadQueue = true;
					}
					newcatalog.Remove(value.assetName);
				}
				else if (value.createLocalCache)
				{
					dictionary.Add(value.assetName, value);
					if (value.Expired())
					{
						DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Expired resource " + value.assetName);
						value.inDownloadQueue = true;
					}
				}
				else
				{
					flag = true;
					if (value.IsDownloading())
					{
						value.StopDownload();
					}
					value.ReleaseResource();
					DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Resource removed from catalog " + value.Serialize());
				}
			}
			foreach (KeyValuePair<string, AppResource> item2 in newcatalog)
			{
				AppResource value = item2.Value;
				flag = true;
				if (value.isRequired)
				{
					value.inDownloadQueue = true;
				}
				dictionary.Add(value.assetName, value);
				DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "New Resource added to catalog " + item2.Value.Serialize());
			}
			if (ResourceManager.CatalogUpdatedEvent != null)
			{
				ResourceManager.CatalogUpdatedEvent(flag);
			}
			ResourceCatalog = dictionary;
		}
		else
		{
			DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Catalog download failed");
		}
		DownloadNextInQueue();
		if (AllRequiredDownloaded() && RequiredReadyCallback != null)
		{
			RequiredReadyCallback(true, "NoNeed");
			RequiredReadyCallback = null;
		}
		if (flag)
		{
			SaveResourceCatalog();
		}
		else
		{
			DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "No catalog change");
		}
	}

	public void SaveResourceCatalog()
	{
		List<AppResource> list = TestCatalog = new List<AppResource>();
		foreach (KeyValuePair<string, AppResource> item in ResourceCatalog)
		{
			if (item.Value.ShouldSaveToCatalog())
			{
				list.Add(item.Value);
			}
		}
		string text = JSONManager.serializeToJson(list);
		DisneyMobile.CoreUnitySystems.Logger.LogInfo(this, "Resource Catalog Saved : " + text);
		FileManager.SaveTextFile(AppResource.ResourceSaveLocation, "AppResourceCatalog.txt", text);
	}

	public void OnDownloadCompleted(bool successful, AppResource ar, string msg)
	{
		if (ar.isRequired)
		{
			if (!successful)
			{
				if (RequiredReadyCallback != null)
				{
					RequiredReadyCallback(successful, msg);
					RequiredReadyCallback = null;
				}
			}
			else if (AllRequiredDownloaded() && RequiredReadyCallback != null)
			{
				RequiredReadyCallback(successful, msg);
				RequiredReadyCallback = null;
			}
		}
		DownloadNextInQueue();
	}

	private IEnumerator StartResourceVersionCheck()
	{
		if (ResourceVersioncheckInProgress)
		{
		}
		ResourceVersioncheckInProgress = true;
		string url = GetAMPSAssetListInfoCallURL();
		WWW www = new WWW(url);
		yield return www;
		bool noErrors = www.isDone && www.error == null && www.text != null;
		if (www.error != null)
		{
			DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "DoResourceVersionCheck  www.error=" + www.error);
			ShowAMPSDownloadError("Msg_DownloadFailLong", "UIManagerOz::StartResourceVersionCheck error=" + www.error);
			noErrors = false;
		}
		if (noErrors)
		{
			try
			{
				AMPSAssetList = JSONManager.getTemplatedTypeForJson<AMPVersionResultData>(www.text);
				if (AMPSAssetList == null)
				{
					DisneyMobile.CoreUnitySystems.Logger.LogFatal(this, "Deserialization AMPS asset list data failed " + url);
					noErrors = false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("deserialization exception " + url + " exception=" + ex);
				ShowAMPSDownloadError("Msg_DownloadFailLong", "UIManagerOz::StartResourceVersionCheck: deserialization exception. url=" + url + " exception=" + ex);
				noErrors = false;
			}
		}
		if (noErrors)
		{
			OnCatalogDownloaded(AMPSAssetList.GetResourceCatalog());
		}
		else
		{
			OnCatalogDownloaded(null);
		}
		ResourceVersioncheckInProgress = false;
	}

	public void ShowAMPSDownloadError(string localizedMessageKey, string debugMessage)
	{
	}

	private void resource1()
	{
		ResourceCatalog["resource1"].Finished(true, "successful");
	}

	private void resource2()
	{
		ResourceCatalog["resource2"].Finished(true, "successful");
	}

	private void TestCatalogDownload()
	{
		Dictionary<string, AppResource> dictionary = new Dictionary<string, AppResource>();
		dictionary.Add("resource1", new AppResource("resource1", true, "1.0.0", "Test Robot Bundle1", 2));
		dictionary.Add("resource2", new AppResource("resource2", true, "1.0.0", "Test Robot Bundle2", 2));
		OnCatalogDownloaded(dictionary);
	}
}
