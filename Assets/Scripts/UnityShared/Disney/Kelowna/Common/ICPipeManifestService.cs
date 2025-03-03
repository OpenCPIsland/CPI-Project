using System.Collections;

namespace Disney.Kelowna.Common
{
	public interface ICPipeManifestService
	{
		int CPipeLatestManifestVersion
		{
			get;
		}

		IEnumerator LookupAssetUrl(CPipeManifestResponse cpipeManifestResponse, string assetName);
	}
}
