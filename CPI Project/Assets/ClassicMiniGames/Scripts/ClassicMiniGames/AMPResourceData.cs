using DisneyMobile.CoreUnitySystems;
using System;
using System.Globalization;

[Serializable]
public class AMPResourceData
{
	public string asset = "";

	public string assetType = "";

	public string status = "";

	public string assetVersion = "";

	public string fileName = "";

	public int fileSize = 0;

	public string url = "";

	public int GetAssetVersion()
	{
		int result = -1;
		try
		{
			result = (int)float.Parse(assetVersion, CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Logger.LogFatal(this, "Asset version number parse error [" + assetVersion + "] exception " + ex.Message);
		}
		return result;
	}

	public AppResource CreateCatalogData(int catalogVersion)
	{
		AppResource appResource = new AppResource();
		appResource.assetName = asset;
		appResource.url = url;
		appResource.assetFileName = fileName;
		appResource.assetVersion = catalogVersion.ToString();
		appResource.serverCatalogVersion = GetAssetVersion();
		appResource.isRequired = false;
		appResource.isAmps = true;
		appResource.isRequired = true;
		return appResource;
	}
}
