using System.Collections;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class WwwBundleDevice_Load_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "test/test_cube?dl=www-bundle&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			GcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			CPipeManifestService cpipeManifestService = new CPipeManifestService("https://storage.googleapis.com/ad37ed76-3a24-91ba-def6-2ff16973c49d.disney.io/", "__mapping_cpremix_dev.json", gcsAccessTokenService);
			deviceManager.Mount(new WwwBundleDevice(deviceManager, gcsAccessTokenService, cpipeManifestService));
			AssetRequest<AssetBundle> request = deviceManager.LoadAsync<AssetBundle>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(request == null);
			IntegrationTestEx.FailIf(request.Finished);
			IntegrationTestEx.FailIf(request.Cancelled);
			yield return request;
			IntegrationTest.Assert(request.Finished);
			IntegrationTest.Assert(!request.Cancelled);
			IntegrationTest.Assert(request.Asset != null);
			IntegrationTest.Assert(request.Asset.GetAllAssetNames().Length == 1);
			string[] assetNames = request.Asset.GetAllAssetNames();
			IntegrationTest.Assert(assetNames.Contains("assets/bundleassets/test/test_cube.prefab"));
			request.Asset.Unload(false);
		}
	}
}
