using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using LitJsonMapper = LitJson.JsonMapper;
using AssemblyCSharpEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.Build.Pipeline;

public class CreateAssetBundles : MonoBehaviour
{
    private const string CdnGroupPrefix = "CDN-";
    private const string CdnManifestGroupPrefix = "CDN-Manifest-";
    private static List<BuildTarget> targets
    {
        get { return CdnTargetSettings.Get(); }
    }
    private static readonly Dictionary<string, string> outputBundleNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    // CRCs of the bundles Addressables built, by file name. The Addressables pipeline writes no .manifest
    // files, so BuildPipeline.GetCRCForAssetBundle can't be used.
    private static readonly Dictionary<string, uint> bundleCrcs = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);

    public static long Todaysdate1;
    public static long Todaysdate2;

    private static string generateHexString()
    {
        System.Random random = new System.Random();
        var bytes = new Byte[16];
        random.NextBytes(bytes);
        return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))).ToLowerInvariant();
    }

    public static long UnixTimeNow()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static string CreateMD5(string input)
    {
        input += UnixTimeNow().ToString();
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                result.Append(hashBytes[i].ToString("X2"));
            }
            return result.ToString().ToLowerInvariant();
        }
    }

    public static string CalculateCRC32(string filePath)
    {
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var crc32 = new Crc32();
            byte[] hashBytes = crc32.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }

    public static bool TryGetOutputBundleName(string groupName, out string bundleName)
    {
        return outputBundleNames.TryGetValue(groupName, out bundleName);
    }

    private static List<AddressableAssetGroup> GetCdnGroups(AddressableAssetSettings settings)
    {
        List<AddressableAssetGroup> groups = new List<AddressableAssetGroup>();
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group != null &&
                group.Name.StartsWith(CdnGroupPrefix, StringComparison.OrdinalIgnoreCase) &&
                !group.Name.StartsWith(CdnManifestGroupPrefix, StringComparison.OrdinalIgnoreCase) &&
                group.entries.Count > 0)
            {
                groups.Add(group);
            }
        }

        if (groups.Count == 0)
        {
            throw new InvalidOperationException("No CDN Addressables groups contain assets.");
        }
        return groups;
    }

    private static string GetCdnBundleName(AddressableAssetGroup group)
    {
        return "cdn/" + group.Name.Substring(CdnGroupPrefix.Length).ToLowerInvariant();
    }

    private static string GetPlatformName(BuildTarget target)
    {
        return (int)target == 24 ? "standalonelinux" : target.ToString().ToLowerInvariant();
    }

    private static string GetContentManifestDirectory(BuildTarget target)
    {
        return "Assets/assetpipeline/generated/output/unbundledassets/test event/" + target.ToString().ToLowerInvariant();
    }

    private static string GetClientBundleDirectory(BuildTarget target)
    {
        return "Export/ClientBundles/" + GetPlatformName(target);
    }

    private static CdnAddressableBundleBuilder GetCdnBuilder(AddressableAssetSettings settings)
    {
        for (int i = 0; i < settings.DataBuilders.Count; i++)
        {
            CdnAddressableBundleBuilder builder = settings.DataBuilders[i] as CdnAddressableBundleBuilder;
            if (builder != null)
            {
                return builder;
            }
        }

        CdnAddressableBundleBuilder newBuilder = ScriptableObject.CreateInstance<CdnAddressableBundleBuilder>();
        newBuilder.name = nameof(CdnAddressableBundleBuilder);
        AssetDatabase.AddObjectToAsset(newBuilder, settings);
        if (!settings.AddDataBuilder(newBuilder, false))
        {
            throw new InvalidOperationException("Failed to add the CDN Addressables builder.");
        }
        AssetDatabase.SaveAssets();
        return newBuilder;
    }

    private static void BuildAddressables(BuildTarget target, List<AddressableAssetGroup> includedGroups, string outputDirectory)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        CdnAddressableBundleBuilder builder = GetCdnBuilder(settings);
        Dictionary<AddressableAssetGroup, bool> previousBuildStates = new Dictionary<AddressableAssetGroup, bool>();
        string previousBuildPath = settings.profileSettings.GetValueByName(settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath);
        string buildPath = Path.GetFullPath(outputDirectory);
        try
        {
            foreach (AddressableAssetGroup group in settings.groups)
            {
                if (group == null)
                {
                    continue;
                }
                BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null)
                {
                    continue;
                }
                previousBuildStates.Add(group, schema.IncludeInBuild);
                schema.IncludeInBuild = includedGroups.Contains(group);
                if (schema.IncludeInBuild)
                {
                    schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
                    schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
                    schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
                    schema.Compression = BundledAssetGroupSchema.BundleCompressionMode.Uncompressed;
                    schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
                }
            }

            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, buildPath);
            Directory.CreateDirectory(outputDirectory);
            AddressablesDataBuilderInput input = new AddressablesDataBuilderInput(settings, new BuildPlayerOptions { target = target });
            AddressablesPlayerBuildResult result = builder.BuildData<AddressablesPlayerBuildResult>(input);
            if (result == null || !string.IsNullOrEmpty(result.Error))
            {
                throw new InvalidOperationException(result == null ? "Addressables returned no CDN build result." : result.Error);
            }
            foreach (AddressablesPlayerBuildResult.BundleBuildResult built in result.AssetBundleBuildResults)
            {
                bundleCrcs[Path.GetFileName(built.FilePath)] = built.Crc;
            }
            foreach (string file in Directory.GetFiles(outputDirectory, "*", SearchOption.AllDirectories))
            {
                if (!file.EndsWith(".unity3d", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(file);
                }
            }
        }
        finally
        {
            foreach (KeyValuePair<AddressableAssetGroup, bool> state in previousBuildStates)
            {
                BundledAssetGroupSchema schema = state.Key.GetSchema<BundledAssetGroupSchema>();
                if (schema != null)
                {
                    schema.IncludeInBuild = state.Value;
                }
            }
            settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, previousBuildPath);
            AssetDatabase.SaveAssets();
        }
    }

    private static Dictionary<string, string> CreateBundleBindings(List<AddressableAssetGroup> groups)
    {
        Dictionary<string, string> bindings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        outputBundleNames.Clear();
        foreach (AddressableAssetGroup group in groups)
        {
            string bundleName = GetCdnBundleName(group);
            string bindingName = CreateMD5(bundleName);
            bindings.Add(bundleName, bindingName);
            outputBundleNames.Add(group.Name, bindingName);
        }
        return bindings;
    }

    private static void BuildCdnAddressables(BuildTarget target, List<AddressableAssetGroup> groups)
    {
        BuildAddressables(target, groups, GetClientBundleDirectory(target));
    }

    private static void WriteContentManifest(BuildTarget target, List<AddressableAssetGroup> groups, Dictionary<string, string> bindings)
    {
        string targetDirectory = GetContentManifestDirectory(target);
        string manifestPath = targetDirectory + "/contentmanifest.txt";
        string platform = GetPlatformName(target);
        string outputDirectory = GetClientBundleDirectory(target);
        Dictionary<string, string> assetNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        Directory.CreateDirectory(targetDirectory);
        using (StreamWriter writer = new StreamWriter(manifestPath, false))
        {
            foreach (AddressableAssetGroup group in groups)
            {
                string bundleName = GetCdnBundleName(group);
                string bindingName = bindings[bundleName];
                HashSet<string> groupBindings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    string entryBindingName = entry.IsScene ? bindingName + "_scenes" : bindingName;
                    string path = entry.AssetPath;
                    string[] assetFileName = path.ToLowerInvariant().Split('/');
                    int resourcesIndex = 0;
                    bool markerFound = false;
                    // Scenes keep their bare name as the key, so they have no marker folder to find.
                    string lowerAssetPath = path.ToLowerInvariant();
                    bool isSpecialScenePath = lowerAssetPath.Contains("resources/scenes") || lowerAssetPath.Contains("resources/additivscenes");
                    for (int i = 1; i < assetFileName.Length - 1; i++)
                    {
                        resourcesIndex++;
                        string lowerPath = path.ToLowerInvariant();
                        if (lowerPath.Contains("resources/scenes") || lowerPath.Contains("resources/additivscenes"))
                        {
                            continue;
                        }
                        // Content keys are the path below a Resources or AssetBundles folder, as in the
                        // embedded manifest (p=game/gui/resources, p=modules/igloo/assetbundles).
                        if (assetFileName[i] == "resources" || assetFileName[i] == "resources_landscape" || assetFileName[i] == "assetbundles")
                        {
                            markerFound = true;
                            break;
                        }
                    }
                    if (!markerFound && !isSpecialScenePath)
                    {
                        throw new InvalidOperationException("CDN Addressable asset is not under a Resources or AssetBundles folder: " + path);
                    }

                    string assetBasePath = string.Join("/", assetFileName.Skip(1).Take(resourcesIndex));
                    string assetBase = string.Join("/", assetFileName.Skip(resourcesIndex + 1).Take(assetFileName.Length - resourcesIndex - 1));
                    string[] assetNameParts = assetBase.Split('.');
                    if (assetNameParts.Length < 2)
                    {
                        throw new InvalidOperationException("CDN Addressable asset has no file extension: " + path);
                    }

                    string assetName = assetNameParts[0];
                    string assetEntry;
                    if (assetNames.TryGetValue(assetName, out string duplicateCount))
                    {
                        int nextDuplicateCount = int.Parse(duplicateCount) + 1;
                        assetNames[assetName] = nextDuplicateCount.ToString();
                        assetEntry = string.Format("asset:{0}_{1}?dl=bundle:www-bundle&x={2}&b={3}.unity3d&p={4}", assetName, nextDuplicateCount, assetNameParts[1], entryBindingName, assetBasePath);
                    }
                    else
                    {
                        assetNames.Add(assetName, "0");
                        assetEntry = string.Format("asset:{0}?dl=bundle:www-bundle&x={1}&b={2}.unity3d&p={3}", assetName, assetNameParts[1], entryBindingName, assetBasePath);
                    }
                    writer.WriteLine(assetEntry);
                    groupBindings.Add(entryBindingName);
                }

                foreach (string entryBindingName in groupBindings)
                {
                    string bundleFile = entryBindingName + ".unity3d";
                    uint crc;
                    if (!bundleCrcs.TryGetValue(bundleFile, out crc))
                    {
                        throw new InvalidOperationException("Addressables reported no CRC for CDN bundle: " + Path.Combine(outputDirectory, bundleFile));
                    }
                    writer.WriteLine(string.Format("bundle:{0}.unity3d?d=&p=0&crc={1}", entryBindingName, crc));
                }
            }

            writer.WriteLine("baseuri:ClientBundles/" + platform);
            writer.WriteLine("contentversion:");
            writer.WriteLine("contentmanifesthash:");
        }
    }

    private static string BuildAddressableManifestBundle(BuildTarget target, string bundleName)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        string targetDirectory = GetContentManifestDirectory(target);
        string manifestPath = targetDirectory + "/contentmanifest.txt";
        string prerequisiteManifestPath = "Assets/assetpipeline/generated/internal/test event/sceneprereqcontentprocessor/ScenesPrereqBundlesManifest.json";
        string groupName = CdnManifestGroupPrefix + bundleName;

        if (!File.Exists(manifestPath) || !File.Exists(prerequisiteManifestPath))
        {
            throw new FileNotFoundException("The CDN manifest source files are missing.");
        }

        AddressableAssetGroup group = settings.CreateGroup(groupName, false, false, false, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
        outputBundleNames.Clear();
        outputBundleNames.Add(group.Name, bundleName);
        try
        {
            AddManifestEntry(settings, group, manifestPath);
            AddManifestEntry(settings, group, prerequisiteManifestPath);
            AssetDatabase.SaveAssets();
            BuildAddressables(target, new List<AddressableAssetGroup> { group }, GetClientBundleDirectory(target));
        }
        finally
        {
            settings.RemoveGroup(group);
            outputBundleNames.Clear();
            AssetDatabase.SaveAssets();
        }
        return Path.Combine(GetClientBundleDirectory(target), bundleName + ".unity3d");
    }

    private static void AddManifestEntry(AddressableAssetSettings settings, AddressableAssetGroup group, string assetPath)
    {
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid))
        {
            throw new InvalidOperationException("Unable to find the CDN manifest asset: " + assetPath);
        }
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
        entry.address = assetPath.ToLowerInvariant();
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 1: Build ClientBundles and Generate ContentManifest.txt")]
    private static void GetNames()
    {
        if (Directory.Exists("Export"))
        {
            Directory.Delete("Export", true);
        }

        string prerequisiteDirectory = "Assets/assetpipeline/generated/internal/test event/sceneprereqcontentprocessor";
        Directory.CreateDirectory(prerequisiteDirectory);
        File.WriteAllText(prerequisiteDirectory + "/ScenesPrereqBundlesManifest.json", Environment.NewLine + "{" + Environment.NewLine + "}" + Environment.NewLine);

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        List<AddressableAssetGroup> groups = GetCdnGroups(settings);
        foreach (BuildTarget target in targets)
        {
            Dictionary<string, string> bindings = CreateBundleBindings(groups);
            BuildCdnAddressables(target, groups);
            WriteContentManifest(target, groups, bindings);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 2: Build ContentMainfest.txt and Generate ClientManifestDirectory.json")]
    private static void GenerateManifest()
    {
        List<ClientManifest> clientManifestDirectory = new List<ClientManifest>();
        string clientVersion = PlayerSettings.bundleVersion;
        if (File.Exists("Export/ClientManifestDirectory.json"))
        {
            File.Delete("Export/ClientManifestDirectory.json");
        }

        foreach (BuildTarget target in targets)
        {
            string cmanifestName = generateHexString();
            string manifestBundlePath = BuildAddressableManifestBundle(target, cmanifestName);
            if (!File.Exists(manifestBundlePath))
            {
                throw new FileNotFoundException("The CDN Addressables manifest bundle was not generated.", manifestBundlePath);
            }

            string platform = target.ToString().ToLowerInvariant();
            string urlPlatform = GetPlatformName(target);
            string url = string.Format("ClientBundles/{0}/{1}.unity3d", urlPlatform, cmanifestName);
            clientManifestDirectory.Add(new ClientManifest(clientVersion, platform, "production", "Client " + clientVersion, false, url, "", generateHexString(), "Client " + clientVersion, DateTime.UtcNow.ToString("yyyy-M-d HH:mm:ss") + " +00:00"));
        }

        string clientManifestJson = LitJsonMapper.ToJson(new ClientManifestDirectory(clientManifestDirectory));
        Directory.CreateDirectory("Export");
        File.WriteAllText("Export/ClientManifestDirectory.json", clientManifestJson + Environment.NewLine);
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Set CDN Build Targets...")]
    private static void SetCdnTargets()
    {
        ScriptableWizard.DisplayWizard<CdnTargetsWizard>("CDN Build Targets", "Save");
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Set CDN Root URL...")]
    private static void SetCdnRoot()
    {
        ScriptableWizard.DisplayWizard<CdnRootWizard>("CDN Root URL", "Save");
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 3: Generate __manifest cpremix prod")]
    private static void GenerateManifest2()
    {
        ClientManifests clientManifestDirectory = new ClientManifests();
        string todaysDate = UnixTimeNow().ToString();
        Todaysdate1 = UnixTimeNow();
        Todaysdate2 = UnixTimeNow();
        clientManifestDirectory.version = "test-" + todaysDate;
        clientManifestDirectory.unique = todaysDate;
        string cdnRoot = CdnRootSettings.Get();
        clientManifestDirectory.cdnRoot = cdnRoot;
        clientManifestDirectory.url = cdnRoot + "manifests/__manifest_cpremix_prod.0000.json";
        clientManifestDirectory.paths = new Dictionary<string, ClientPaths>();

        string directoryPath = "Export";
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(directoryPath);
        }

        IEnumerable<string> matchingFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Where(file => file.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".unity3d", StringComparison.OrdinalIgnoreCase));

        foreach (string file in matchingFiles)
        {
            string crc32 = CalculateCRC32(file);
            string relativePath = file.Substring(directoryPath.Length + 1).Replace("Export\\", "").Replace("\\", "/");
            ClientPaths clientPath = new ClientPaths { crc = crc32, v = Todaysdate1 };
            if (file.Contains("ClientManifestDirectory.json"))
            {
                clientPath = new ClientPaths { crc = crc32, v = Todaysdate2 };
                clientManifestDirectory.paths[relativePath] = clientPath;
            }
            else
            {
                clientManifestDirectory.paths[relativePath] = clientPath;
            }
        }

        if (File.Exists("Export/__manifest_cpremix_prod.0000.json"))
        {
            File.Delete("Export/__manifest_cpremix_prod.0000.json");
        }
        File.WriteAllText("Export/__manifest_cpremix_prod.0000.json", LitJsonMapper.ToJson(clientManifestDirectory) + Environment.NewLine);
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 4: Pepare for your CDN")]
    private static void GenerateManifest3()
    {
        string manifestsDirectory = "Export/www/manifests";
        string contentDirectory = "Export/www/" + Todaysdate1;
        string clientManifestDirectory = "Export/www/" + Todaysdate2;
        Directory.CreateDirectory(manifestsDirectory);
        Directory.CreateDirectory(contentDirectory);
        Directory.CreateDirectory(clientManifestDirectory);

        string clientBundlesDirectory = contentDirectory + "/ClientBundles";
        Directory.Move("Export/ClientBundles", clientBundlesDirectory);
        foreach (string file in Directory.GetFiles(clientBundlesDirectory, "*", SearchOption.AllDirectories))
        {
            if (!file.EndsWith(".unity3d", StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(file);
            }
        }

        File.Move("Export/ClientManifestDirectory.json", clientManifestDirectory + "/ClientManifestDirectory.json");
        File.Move("Export/__manifest_cpremix_prod.0000.json", manifestsDirectory + "/__manifest_cpremix_prod.0000.json");
    }
}

public sealed class CdnAddressableBundleBuilder : BuildScriptPackedMode
{
    public override string Name
    {
        get { return nameof(CdnAddressableBundleBuilder); }
    }

    protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup, BundledAssetGroupSchema schema, BundleDetails info, string assetBundleName)
    {
        if (assetGroup != null)
        {
            string outputName;
            if (CreateAssetBundles.TryGetOutputBundleName(assetGroup.Name, out outputName))
            {
                return outputName + (assetBundleName.StartsWith("scenes_", StringComparison.OrdinalIgnoreCase) ? "_scenes" : string.Empty) + ".unity3d";
            }
            if (assetGroup.Name.StartsWith("CDN-Manifest-", StringComparison.OrdinalIgnoreCase))
            {
                return assetGroup.Name.Substring("CDN-Manifest-".Length) + ".unity3d";
            }
        }
        return base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
    }
}

public static class CdnRootSettings
{
    private const string PrefKey = "CPI.CdnRoot";
    public const string DefaultRoot = "http://localhost/";

    public static string Normalize(string root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            return DefaultRoot;
        }
        root = root.Trim();
        return root.EndsWith("/") ? root : root + "/";
    }

    public static string Get()
    {
        return Normalize(EditorPrefs.GetString(PrefKey, DefaultRoot));
    }

    public static void Set(string root)
    {
        EditorPrefs.SetString(PrefKey, Normalize(root));
    }
}

public sealed class CdnRootWizard : ScriptableWizard
{
    public string cdnRoot;

    private void OnEnable()
    {
        cdnRoot = CdnRootSettings.Get();
    }

    private void OnWizardCreate()
    {
        CdnRootSettings.Set(cdnRoot);
    }
}


public static class CdnTargetSettings
{
    private const string PrefKey = "CPI.CdnTargets";
    private const string EnvKey = "CPI_CDN_TARGETS";
    public static readonly BuildTarget[] All =
    {
        BuildTarget.Android,
        BuildTarget.iOS,
        BuildTarget.StandaloneWindows64,
        BuildTarget.StandaloneOSX,
        BuildTarget.StandaloneLinux64
    };

    // A comma-separated list of BuildTarget names. Unknown names are ignored; an empty or
    // unusable list means every target. Order always follows All.
    public static List<BuildTarget> Parse(string list)
    {
        List<BuildTarget> result = new List<BuildTarget>();
        if (!string.IsNullOrWhiteSpace(list))
        {
            HashSet<string> names = new HashSet<string>(list.Split(',').Select(n => n.Trim()), StringComparer.OrdinalIgnoreCase);
            foreach (BuildTarget target in All)
            {
                if (names.Contains(target.ToString()))
                {
                    result.Add(target);
                }
            }
        }
        return result.Count > 0 ? result : new List<BuildTarget>(All);
    }

    // The environment variable (for headless builds) wins over the saved choice.
    public static List<BuildTarget> Get()
    {
        string fromEnv = Environment.GetEnvironmentVariable(EnvKey);
        return Parse(string.IsNullOrWhiteSpace(fromEnv) ? EditorPrefs.GetString(PrefKey, "") : fromEnv);
    }

    public static void Set(IEnumerable<BuildTarget> chosen)
    {
        EditorPrefs.SetString(PrefKey, string.Join(",", chosen.Select(t => t.ToString()).ToArray()));
    }
}

public sealed class CdnTargetsWizard : ScriptableWizard
{
    public bool android = true;
    public bool iOS = true;
    public bool windows64 = true;
    public bool macOS = true;
    public bool linux64 = true;

    private void OnEnable()
    {
        List<BuildTarget> current = CdnTargetSettings.Get();
        android = current.Contains(BuildTarget.Android);
        iOS = current.Contains(BuildTarget.iOS);
        windows64 = current.Contains(BuildTarget.StandaloneWindows64);
        macOS = current.Contains(BuildTarget.StandaloneOSX);
        linux64 = current.Contains(BuildTarget.StandaloneLinux64);
    }

    private void OnWizardCreate()
    {
        List<BuildTarget> chosen = new List<BuildTarget>();
        if (android) chosen.Add(BuildTarget.Android);
        if (iOS) chosen.Add(BuildTarget.iOS);
        if (windows64) chosen.Add(BuildTarget.StandaloneWindows64);
        if (macOS) chosen.Add(BuildTarget.StandaloneOSX);
        if (linux64) chosen.Add(BuildTarget.StandaloneLinux64);
        CdnTargetSettings.Set(chosen);
    }
}
