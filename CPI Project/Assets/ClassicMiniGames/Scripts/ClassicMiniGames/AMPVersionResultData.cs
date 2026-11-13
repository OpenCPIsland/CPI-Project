using DisneyMobile.CoreUnitySystems;
using System;
using System.Collections.Generic;

[Serializable]
public class AMPVersionResultData
{
	public AMPVersionResponse response;

	public AMPVersionResult result;

	public bool IsReady()
	{
		return result.data != null;
	}

	public Dictionary<string, AppResource> GetResourceCatalog()
	{
		Dictionary<string, AppResource> dictionary = new Dictionary<string, AppResource>();
		foreach (AMPResourceData datum in result.data)
		{
			if (datum.status == "liv")
			{
				Logger.LogInfo(this, "Found resource in downloaded AMPS catalog " + datum.fileName);
				dictionary.Add(datum.asset, datum.CreateCatalogData(result.meta.catalogVersion));
			}
		}
		return dictionary;
	}

	public int GetAssetVersion(string bname)
	{
		if (result.data == null)
		{
			return -1;
		}
		int num = -1;
		foreach (AMPResourceData datum in result.data)
		{
			if (datum != null && datum.asset == bname)
			{
				int assetVersion = datum.GetAssetVersion();
				if (assetVersion > num)
				{
					num = assetVersion;
				}
			}
		}
		return num;
	}
}
