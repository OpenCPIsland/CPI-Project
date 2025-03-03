using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Disney.Kelowna.Common.Manifest
{
	public class InitializeManifestDefinitionCommand
	{
		public delegate void ManifestInitializationComplete(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest = null, bool requiresAppUgrade = false, bool appUgradeAvailable = false);

		private const int LOAD_MANIFEST_TIMEOUT_SECS = 10;

		private ManifestInitializationComplete callback;

		private IManifestService manifestService;

		public ContentManifestDirectory manifestDirectory;

		public ContentManifest embeddedManifest;

		public ContentManifest contentManifest;

		private ContentManifest downloadedManifest;

		private ScenePrereqContentBundlesManifest scenePrereqBundlesManifest;

		private bool appUpgradeAvailable;

		private readonly CoroutineGroup timeoutCoroutines = new CoroutineGroup();

		public readonly CoroutineGroup manifestCoroutines = new CoroutineGroup();

		private ICoroutine timeoutCheck;

		public InitializeManifestDefinitionCommand(IManifestService service, ManifestInitializationComplete callback)
		{
			manifestService = service;
			this.callback = callback;
		}

		private void onInitializeComplete(ContentManifest manifest, bool requiresAppUgrade = false)
		{
			if (timeoutCheck != null && !timeoutCheck.Completed && !timeoutCheck.Cancelled)
			{
				timeoutCheck.Cancel();
			}
			manifest.UpdateEntryMaps();
			callback(manifest, scenePrereqBundlesManifest, requiresAppUgrade, appUpgradeAvailable);
		}

		public void Execute()
		{
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (commonGameSettings.OfflineMode && string.IsNullOrEmpty(commonGameSettings.CDN))
			{
				onInitializeComplete(contentManifest_loadEmbedded());
			}
			else
			{
				try
				{
					timeoutCheck = CoroutineRunner.Start(timeout_loadManifestSequence(), this, "timeoutLoadPatchManifest");
					timeoutCoroutines.Add(timeoutCheck);
					manifestCoroutines.StartAndAddPersistent(loadContentManifest(), this, "loadContentManifest");
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
					handleFailure();
				}
			}
		}

		public IEnumerator loadContentManifest()
		{
			Stopwatch timer = new Stopwatch();
			timer.Start();
			yield return manifestDirectory_download();
			timer.Stop();
			if (manifestDirectory == null)
			{
				Log.LogError(this, "The ContentManifestDirectory downloaded is null! The request to download the directory either was not made or failed to download and deserialize. Reverting to the embedded ContentManifest as no ContentManifestDirectoryEntry is available to reference.");
				handleFailure();
				yield break;
			}
			embeddedManifest = manifestService.LoadEmbeddedManifest();
			DateTimeOffset contentDate = manifestService.GetContentDate();
			System.Version clientVersion = ClientInfo.ParseClientVersion(ClientInfo.Instance.ClientVersion);
            System.Version clientApiVersion = ClientInfo.ParseClientVersion(manifestService.GetClientApiVersionStr());
			string platform = manifestService.GetClientPlatform();
			string environment = manifestService.GetServerEnvironment().ToString().ToLower();
			ContentManifestDirectoryEntry clientDirectoryEntry = manifestDirectory.FindEntry(contentDate, clientVersion, platform, environment);
			appUpgradeAvailable = manifestDirectory.DoesNewerVersionExist(contentDate, clientVersion, platform, environment);
			if (clientDirectoryEntry == null)
			{
				Log.LogErrorFormatted(this, "No client directory entry available for clientVersion={0}, platform={1}, environment={2}. \nTriggering force upgrade.", clientVersion.ToString(), platform, environment);
				handleFailure(true);
				yield break;
			}
			ContentManifestDirectoryEntry contentDirectoryEntry = clientDirectoryEntry;
			if (!clientVersion.Equals(clientApiVersion))
			{
				contentDirectoryEntry = manifestDirectory.FindEntry(contentDate, clientApiVersion, platform, environment);
				if (contentDirectoryEntry == null)
				{
					Log.LogErrorFormatted(this, "Missing Client API Version: No content available for clientApiVersion={0}, platform={1}, environment={2}. Falling back to embedded content version", clientApiVersion.ToString(), platform, environment);
					handleFailure();
					yield break;
				}
			}
			if (contentDirectoryEntry.IsEmbeddedContent())
			{
				contentManifest = embeddedManifest;
				checkContentVersionAndManifestHashInManifest(contentManifest, contentDirectoryEntry);
			}
			else
			{
				yield return contentManifest_download(contentDirectoryEntry);
				contentManifest = ContentManifestUtility.Merge(embeddedManifest, downloadedManifest);
			}
			onInitializeComplete(contentManifest);
		}

		private void checkContentVersionAndManifestHashInManifest(ContentManifest manifest, ContentManifestDirectoryEntry directoryEntry)
		{
			if (manifest == null || directoryEntry == null)
			{
				Log.LogErrorFormatted(this, "Can't check on manifest since one of the parameters is null manifest={0}, directoryEntry={1}\t", manifest, directoryEntry);
				return;
			}
			if (string.IsNullOrEmpty(manifest.ContentManifestHash) && !string.IsNullOrEmpty(directoryEntry.url))
			{
				string text = manifest.ContentManifestHash = Path.GetFileNameWithoutExtension(directoryEntry.url);
			}
			if (string.IsNullOrEmpty(manifest.ContentVersion))
			{
				manifest.ContentVersion = directoryEntry.contentVersion;
			}
			else if (manifest.ContentVersion != directoryEntry.contentVersion)
			{
				manifest.ContentVersion = directoryEntry.contentVersion;
			}
		}

		public IEnumerator manifestDirectory_download()
		{
			ICPipeManifestService cpipeManifestService = Service.Get<ICPipeManifestService>();
			CPipeManifestResponse cpipeManifestResponse = new CPipeManifestResponse();
			yield return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, "ClientManifestDirectory.json");
			bool forceDownload = false;
			Uri uri = new Uri(cpipeManifestResponse.FullAssetUrl);
			string manifestDirectory_cachedPath = PathUtil.Combine(Application.persistentDataPath, uri.LocalPath);
			if (!forceDownload && File.Exists(manifestDirectory_cachedPath))
			{
				string stringToDeserialize = File.ReadAllText(manifestDirectory_cachedPath);
				manifestDirectory = Service.Get<JsonService>().Deserialize<ContentManifestDirectory>(stringToDeserialize);
				yield break;
			}
			UnityWebRequest www = UnityWebRequest.Get(cpipeManifestResponse.FullAssetUrl);
			Service.Get<LoadingController>().RegisterDownload(www);
			yield return www.SendWebRequest();
			Service.Get<LoadingController>().UnRegisterDownload(www);
			try
			{
				if (www.isNetworkError || string.IsNullOrEmpty(www.downloadHandler.text))
				{
					throw new Exception("Failed to download manifest directory from CDN. Reverting to the embedded directory.");
				}
				manifestDirectory = Service.Get<JsonService>().Deserialize<ContentManifestDirectory>(www.downloadHandler.text);
				string parentPath = PathUtil.GetParentPath(manifestDirectory_cachedPath);
				if (!Directory.Exists(parentPath))
				{
					Directory.CreateDirectory(parentPath);
				}
				File.WriteAllText(manifestDirectory_cachedPath, www.downloadHandler.text);
			}
			catch (Exception)
			{
				manifestDirectory = manifestDirectory_loadEmbedded();
				onInitializeComplete(manifestService.LoadEmbeddedManifest());
			}
		}

		private ContentManifestDirectory manifestDirectory_loadEmbedded()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Configuration/embedded_manifest_directory.json");
			return Service.Get<JsonService>().Deserialize<ContentManifestDirectory>(textAsset.text);
		}

		private IEnumerator contentManifest_download(ContentManifestDirectoryEntry directoryEntry)
		{
			if (directoryEntry == null)
			{
				handleFailure();
				yield break;
			}
			if (string.IsNullOrEmpty(directoryEntry.url))
			{
				handleFailure();
				yield break;
			}
			string contentManifestUrl = directoryEntry.url;
			ICPipeManifestService cpipeManifestService = Service.Get<ICPipeManifestService>();
			CPipeManifestResponse cpipeManifestResponse = new CPipeManifestResponse();
			yield return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, contentManifestUrl);
			UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(cpipeManifestResponse.FullAssetUrl, 1u, 0u);
			Service.Get<LoadingController>().RegisterDownload(www);
			yield return www.SendWebRequest();
			Service.Get<LoadingController>().UnRegisterDownload(www);
			AssetBundle contentBundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
			if (www.isNetworkError || contentBundle == null)
			{
				Log.LogErrorFormatted(this, "Failed to download content bundle from CDN: {0}", www.error);
				handleFailure();
				yield break;
			}
			downloadedManifest = ContentHelper.GetContentManifestFromAssetBundle(contentBundle);
			if (downloadedManifest != null)
			{
				checkContentVersionAndManifestHashInManifest(downloadedManifest, directoryEntry);
				contentManifest_cacheDownloadedToDisk(downloadedManifest);
			}
			string[] assetNames = contentBundle.GetAllAssetNames();
			for (int i = 0; i < assetNames.Length; i++)
			{
				string fileName = Path.GetFileName(assetNames[i]);
				if (string.Equals(fileName, "ScenesPrereqBundlesManifest.json", StringComparison.OrdinalIgnoreCase))
				{
					TextAsset textAsset = contentBundle.LoadAsset<TextAsset>(assetNames[i]);
					scenePrereqBundlesManifest = Service.Get<JsonService>().Deserialize<ScenePrereqContentBundlesManifest>(textAsset.text);
					scenePrereqBundlesManifest_cacheDownloadedToDisk(scenePrereqBundlesManifest);
				}
			}
			contentBundle.Unload(false);
			if (downloadedManifest == null)
			{
				Log.LogError(this, "Content manifest not found in downloaded bundle! Reverting to the embedded manifest");
				handleFailure();
			}
		}

		private void contentManifest_cacheDownloadedToDisk(ContentManifest manifestToBeCached)
		{
			string path = Path.Combine(Application.persistentDataPath, "ContentManifest.txt");
			manifestToBeCached.WriteToFile(path);
		}

		private ContentManifest contentManifest_loadFromCache()
		{
			string path = Path.Combine(Application.persistentDataPath, "ContentManifest.txt");
			if (File.Exists(path))
			{
				ContentManifest contentManifest = new ContentManifest();
				contentManifest.ReadFromFile(path);
				return contentManifest;
			}
			return null;
		}

		private ContentManifest contentManifest_loadEmbedded()
		{
			return manifestService.LoadEmbeddedManifest();
		}

		public static void contentManifest_clearCachedManifest()
		{
			string path = Path.Combine(Application.persistentDataPath, "ContentManifest.txt");
			File.Delete(path);
		}

		private ScenePrereqContentBundlesManifest loadCachedScenePrereqBundlesManifest()
		{
			string path = Path.Combine(Application.persistentDataPath, "ScenesPrereqBundlesManifest.json");
			string stringToDeserialize = File.ReadAllText(path);
			return Service.Get<JsonService>().Deserialize<ScenePrereqContentBundlesManifest>(stringToDeserialize);
		}

		private void scenePrereqBundlesManifest_cacheDownloadedToDisk(ScenePrereqContentBundlesManifest scenePrereqBundlesManifestToCache)
		{
			string path = Path.Combine(Application.persistentDataPath, "ScenesPrereqBundlesManifest.json");
			string contents = Service.Get<JsonService>().Serialize(scenePrereqBundlesManifestToCache);
			File.WriteAllText(path, contents);
		}

		private IEnumerator timeout_loadManifestSequence()
		{
			yield return new WaitForSeconds(10f);
			onInitializeComplete(contentManifest_loadEmbedded());
		}

		private void handleFailure(bool requiresAppUgrade = false)
		{
			timeoutCoroutines.StopAll();
			manifestCoroutines.StopAll();
			onInitializeComplete(contentManifest_loadEmbedded(), requiresAppUgrade);
		}
	}
}
