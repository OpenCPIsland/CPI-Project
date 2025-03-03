using Disney.MobileNetwork;
using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public abstract class BaseContentIntegrationTest : BaseIntegrationTest
	{
		protected ContentManifest manifest;

		protected virtual ContentManifest generateManifest()
		{
			return ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest");
		}

		protected override IEnumerator setup()
		{
			manifest = generateManifest();
			IGcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			Service.Set(gcsAccessTokenService);
			ICPipeManifestService instance = new CPipeManifestService(ContentHelper.GetCdnUrl(), ContentHelper.GetCpipeMappingFilename(), gcsAccessTokenService);
			Service.Set(instance);
			Content instance2 = new Content(manifest);
			Service.Set(instance2);
			Content.BundleManager.UnmountFrameCount = 4;
			Content.BundleManager.MonitorFrameFrequency = 1;
			yield break;
		}

		protected override void tearDown()
		{
			Content content = Service.Get<Content>();
			if (content != null)
			{
				content.Dispose();
			}
		}
	}
}
