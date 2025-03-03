using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
// Using an alias for LitJson.JsonMapper to avoid conflicts
using LitJsonMapper = LitJson.JsonMapper;
using AssemblyCSharpEditor;

public class CreateAssetBundles : MonoBehaviour
{
    static List<BuildTarget> targets = new List<BuildTarget> {
        BuildTarget.Android,
        BuildTarget.iOS,
        BuildTarget.StandaloneWindows,
        BuildTarget.StandaloneOSX,
        BuildTarget.StandaloneLinux64,
      //  BuildTarget.Switch,
    };

    public static long Todaysdate1;
    public static long Todaysdate2;

    static string generateHexString() {
        System.Random random = new System.Random();
        var bytes = new Byte[16];
        random.NextBytes(bytes);

        var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
        var hexStr = String.Concat(hexArray);
        return hexStr.ToLower();
    }

    public static long UnixTimeNow()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }

    public static string CreateMD5(string input)
    {
        string unixTime = UnixTimeNow().ToString();
        input += unixTime;

        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }

    public static string CalculateCRC32(string filePath)
    {
        using (FileStream fs = File.Open(filePath, FileMode.Open))
        {
            // Create a CRC32 object
            var crc32 = new Crc32();
            byte[] hashBytes = crc32.ComputeHash(fs);

            // Convert hash bytes to a hex string
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }

    static void BuildAllAssetBundles(BuildTarget target, ref Dictionary<string, string> bindings)
    {
        string assetBundleDirectory = "";
        if ((int)target == 24)
        {
            assetBundleDirectory = "Export/ClientBundles/standalonelinux";
        }
        else
        {
            assetBundleDirectory = string.Format("Export/ClientBundles/{0}", target.ToString().ToLower());
        }
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        List<AssetBundleBuild> assetsToBuild = new List<AssetBundleBuild>();
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (string name in names)
        {
            if (name == "contentmanifest")
                continue;
            var bindingName = CreateMD5(name);

            Debug.Log("Building " + name + "With binding name - " + bindingName);
            var paths = AssetDatabase.GetAssetPathsFromAssetBundle(name);
            foreach (var path in paths)
            {
                Debug.Log("Bundle " + name + " File: " + path);
            }
            var asset = new AssetBundleBuild();
            asset.assetBundleName = string.Format("{0}.unity3d", bindingName);
            asset.assetNames = paths;
            assetsToBuild.Add(asset);
            bindings.Add(name, bindingName);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, assetsToBuild.ToArray(), BuildAssetBundleOptions.None, target);
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 1: Build ClientBundles and Generate ContentManifest.txt")]
    static void GetNames()
{

        if (Directory.Exists("Export"))
        {
            Directory.Delete("Export", true);
        }

        var targetPath2 = string.Format("Assets/assetpipeline/generated/internal/test event/sceneprereqcontentprocessor");
        if (!Directory.Exists(targetPath2))
        {
            Directory.CreateDirectory(targetPath2);
        }
        File.Delete(string.Format("{0}/ScenesPrereqBundlesManifest.json", targetPath2));
        StreamWriter writer2 = new StreamWriter(string.Format("{0}/ScenesPrereqBundlesManifest.json", targetPath2), true);
        writer2.WriteLine("");
        writer2.WriteLine("{");
        writer2.WriteLine("}");
        writer2.Close();

        foreach (BuildTarget target in targets)
        {

            var targetPath = ""; //string.Format("Assets/assetpipeline/generated/output/unbundledassets/first test translations/{0}", target.ToString().ToLower());
            if ((int)target == 24)
            {
                targetPath = string.Format("Assets/assetpipeline/generated/output/unbundledassets/test event/standalonelinux");
            }
            else
            {
                targetPath = string.Format("Assets/assetpipeline/generated/output/unbundledassets/test event/{0}", target.ToString().ToLower());
            }
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            File.Delete(string.Format("{0}/contentmanifest.txt", targetPath));
            StreamWriter writer = new StreamWriter(string.Format("{0}/contentmanifest.txt", targetPath), true);

            string assetBundleDirectory = "";
            if ((int)target == 24)
            {
                assetBundleDirectory = "Export/ClientBundles/standalonelinux";
            }
            else
            {
                assetBundleDirectory = string.Format("Export/ClientBundles/{0}", target.ToString().ToLower());
            }
            var contentManifest = "";
            var names = AssetDatabase.GetAllAssetBundleNames();
            Dictionary<string, string> bindings = new Dictionary<string, string>();
            Dictionary<string, string> assetNames = new Dictionary<string, string>();
            BuildAllAssetBundles(target, ref bindings);
            foreach (string name in names)
            {
                if (name == "contentmanifest")
                    continue;

                string bindingName = name;
                bindings.TryGetValue(name, out bindingName);
                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(name);
                Debug.Log("Asset Bundle: " + name);
                foreach (string path in paths)
                {
                    var assetFileName = path.ToLower().Split(new string[] { "/" }, System.StringSplitOptions.None);

                    var resourcesIndex = 0;
                    for (var i = 1; i < assetFileName.Length - 1; i++)
                    {
                        resourcesIndex++;
                        if (path.ToLower().Contains("resources/scenes") || path.ToLower().Contains("resources/additivscenes"))
                        {
                        }
                        else if (assetFileName[i] == "resources" || assetFileName[i] == "resources_landscape") break;
                    }
                    var basepatharr = assetFileName.Skip(1).Take(resourcesIndex);
                    var assetBasePath = basepatharr.Aggregate((total, next) => total + "/" + next);

                    var baseassetarr = assetFileName.Skip(resourcesIndex + 1).Take(assetFileName.Length - resourcesIndex - 1);
                    var assetBase = baseassetarr.Aggregate((total, next) => total + "/" + next);

                    var assetNameParts = assetBase.Split('.');
                    var assetName = assetNameParts[0];

                    var assetEntry = string.Format("asset:{0}?dl=bundle:www-bundle&x={1}&b={2}.unity3d&p={3}", assetName, assetNameParts[1], bindingName, assetBasePath);
                    if (assetNames.ContainsKey(assetName))
                    {
                        assetNames[assetName] = (int.Parse(assetNames[assetName]) + 1).ToString();
                        assetEntry = string.Format("asset:{0}_{1}?dl=bundle:www-bundle&x={2}&b={3}.unity3d&p={4}", assetName, assetNames[assetName], assetNameParts[1], bindingName, assetBasePath);
                    }
                    else
                    {
                        assetNames.Add(assetName, "0");
                    }

                    writer.WriteLine(assetEntry);
                    contentManifest += assetEntry;
                }
                uint crc = 0;
                BuildPipeline.GetCRCForAssetBundle(string.Format("{0}/{1}.unity3d", assetBundleDirectory, bindingName), out crc);
                writer.WriteLine($"bundle:{bindingName}.unity3d?d=&p=0&crc={crc}");
            }
            if ((int)target == 24)
            {
                writer.WriteLine("baseuri:ClientBundles/standalonelinux");
            }
            else
            {
                writer.WriteLine("baseuri:ClientBundles/{0}", target.ToString().ToLower());
            }
            writer.WriteLine("contentversion:");
            writer.WriteLine("contentmanifesthash:");
            writer.Close();
        }
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 2: Build ContentMainfest.txt and Generate ClientManifestDirectory.json")]
    static void GenerateManifest()
    {
        var clientManifestDirectory = new List<ClientManifest>();
        Debug.Log("Generating Manifests...");
        File.Delete("Export/ClientManifestDirectory.json");
        foreach (BuildTarget target in targets)
        {
            var targetPath2 = string.Format("Assets/assetpipeline/generated/internal/test event/sceneprereqcontentprocessor");
            var targetPath = string.Format("Assets/assetpipeline/generated/output/unbundledassets/test event/{0}", target.ToString().ToLower());
            string assetBundleDirectory = "";// string.Format("Export/ClientBundles/{0}", target.ToString().ToLower());
            if ((int)target == 24)
            {
                assetBundleDirectory = "Export/ClientBundles/standalonelinux";
            }
            else
            {
                assetBundleDirectory = string.Format("Export/ClientBundles/{0}", target.ToString().ToLower());
            }
            var cmanifestName = generateHexString();

            if ((int)target == 24)
            {
                clientManifestDirectory.Add(new ClientManifest("1.13.0", target.ToString().ToLower(), "production", "Client 1.13.0_2018_11_05", false, string.Format("ClientBundles/standalonelinux/{1}.unity3d", target.ToString().ToLower(), cmanifestName), "", generateHexString(), "Client 1.13.0", "2024-12-25 00:00:00 -08:00"));
            }
            else
            {
                clientManifestDirectory.Add(new ClientManifest("1.13.0", target.ToString().ToLower(), "production", "Client 1.13.0_2018_11_05", false, string.Format("ClientBundles/{0}/{1}.unity3d", target.ToString().ToLower(), cmanifestName), "", generateHexString(), "Client 1.13.0", "2024-12-25 00:00:00 -08:00"));
            }


            var toBuild = new AssetBundleBuild[1];
            toBuild[0].assetBundleName = string.Format("{0}.unity3d", cmanifestName);
            toBuild[0].assetNames = new string[2];
            toBuild[0].assetNames[0] = string.Format("{0}/contentmanifest.txt", targetPath);
            toBuild[0].assetNames[1] = string.Format("{0}/ScenesPrereqBundlesManifest.json", targetPath2);

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, toBuild, BuildAssetBundleOptions.None, target);
			Debug.Log("Generated... " + target.ToString());
        }
        var clientmd = LitJsonMapper.ToJson(new ClientManifestDirectory(clientManifestDirectory));

        var jsonwriter = new StreamWriter("Export/ClientManifestDirectory.json", true);
        jsonwriter.WriteLine(clientmd);
        jsonwriter.Close();
    }


    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 3: Generate __manifest cpremix prod")]
    static void GenerateManifest2()
    {
        ClientManifests clientManifestDirectory = new ClientManifests();
        string Todaysdate = Convert.ToString((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        Todaysdate1 = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        Todaysdate2 = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        clientManifestDirectory.version = "test-"+ Todaysdate;
        clientManifestDirectory.unique = Todaysdate;
        clientManifestDirectory.cdnRoot = "http://localhost/";
        clientManifestDirectory.url = "http://localhost/manifests/__manifest_cpremix_prod.0000.json";
        clientManifestDirectory.paths = new Dictionary<string, ClientPaths> {};

        string directoryPath = "Export";

        var matchingFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                             .Where(file => file.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                                            file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
                                            file.EndsWith(".unity3d", StringComparison.OrdinalIgnoreCase));

        foreach (string file in matchingFiles)
        {
            FileInfo fileInfo = new FileInfo(file);
            long fileSize = fileInfo.Length;
            string crc32 = CalculateCRC32(file);

            var clientPath = new ClientPaths
            {
                crc = crc32,
                v = Todaysdate1
            };

            string relativePath = file.Substring(directoryPath.Length + 1).Replace("Export\\", "").Replace("\\", "/");
            if(file.Contains("ClientManifestDirectory.json"))
            {
                var clientPath2 = new ClientPaths
                {
                    crc = crc32,
                    v = Todaysdate2
                };
                clientManifestDirectory.paths[relativePath] = clientPath2;
            }
            else
            {
                clientManifestDirectory.paths[relativePath] = clientPath;
            }
        }

        Debug.Log("Generating __manifest_cpremix_prod...");
        File.Delete("Export/__manifest_cpremix_prod.0000.json");

        var clientmd = LitJsonMapper.ToJson(clientManifestDirectory);

        var jsonwriter = new StreamWriter("Export/__manifest_cpremix_prod.0000.json", true);
        jsonwriter.WriteLine(clientmd);
        jsonwriter.Close();
    }

    [MenuItem("Project/AssetBundles/Generated/Generate CDN AssetBundles/Step 4: Pepare for your CDN")]
    static void GenerateManifest3()
    {
        Debug.Log("Peparing for WWW ");

        var targetPath2 = string.Format("Export/www/manifests");
        if (!Directory.Exists(targetPath2))
        {
            Directory.CreateDirectory(targetPath2);
        }

        var targetPath3 = string.Format("Export/www/" + Todaysdate1);
        if (!Directory.Exists(targetPath3))
        {
            Directory.CreateDirectory(targetPath3);
        }


        var targetPath4 = string.Format("Export/www/" + Todaysdate2);
        if (!Directory.Exists(targetPath4))
        {
            Directory.CreateDirectory(targetPath4);
        }

        Directory.Move("Export/ClientBundles/", targetPath3 + "/ClientBundles/");

        string[] files = Directory.GetFiles(targetPath3 + "/ClientBundles/", "*.txt", System.IO.SearchOption.AllDirectories);
        foreach (string file in files)
            try
            {
                File.Delete(file);
            }
            catch { }

        string[] files2 = Directory.GetFiles(targetPath3 + "/ClientBundles/", "*.manifest", System.IO.SearchOption.AllDirectories);
        foreach (string file in files2)
            try
            {
                File.Delete(file);
            }
            catch { }

        File.Delete(targetPath3 + "/ClientBundles/android/android");
        File.Delete(targetPath3 + "/ClientBundles/ios/ios");
        File.Delete(targetPath3 + "/ClientBundles/standaloneosx/standaloneosx");
        File.Delete(targetPath3 + "/ClientBundles/standalonewindows/standalonewindows");
        File.Delete(targetPath3 + "/ClientBundles/standalonelinux64/standalonelinux64");

        File.Move("Export/ClientManifestDirectory.json", targetPath4 + "/ClientManifestDirectory.json");
        File.Move("Export/__manifest_cpremix_prod.0000.json", targetPath2 + "/__manifest_cpremix_prod.0000.json");
        Debug.Log("The WWW is done, now you can put it in your CDN server.");
    }
}
