using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public class MockCPipeManifestServices : ICPipeManifestService
	{
		private CPipeManifestService cpipeManifestService;

		public int CPipeLatestManifestVersion
		{
			get;
			set;
		}

		public MockCPipeManifestServices(IGcsAccessTokenService gcsAccessTokenService)
		{
			cpipeManifestService = new CPipeManifestService(ContentHelper.GetCdnUrl(), ContentHelper.GetCpipeMappingFilename(), gcsAccessTokenService);
		}

		public IEnumerator LookupAssetUrl(CPipeManifestResponse cpipeManifestResponse, string assetName)
		{
			if (assetName == "ClientManifestDirectory.json")
			{
				string assetName2 = "IntegrationTests/ClientVersion-1.1.1/" + assetName;
				return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, assetName2);
			}
			return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, assetName);
		}
	}
}
