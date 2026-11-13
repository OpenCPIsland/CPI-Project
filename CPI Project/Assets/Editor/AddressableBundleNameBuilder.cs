using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.Build.Pipeline;

public sealed class AddressableBundleNameBuilder : BuildScriptPackedMode
{
    public override string Name
    {
        get { return nameof(AddressableBundleNameBuilder); }
    }

    protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup, BundledAssetGroupSchema schema, BundleDetails info, string assetBundleName)
    {
        if (assetGroup != null)
        {
            string bundleName = GenerateAddressables.GetAddressableBundleName(assetGroup);
            if (!string.IsNullOrEmpty(bundleName))
            {
                return bundleName;
            }
        }

        return base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
    }
}
