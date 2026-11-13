using ClubPenguin;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Collectibles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ServerDataGenerator
{
    private const string OutputDirectory = "Assets/Generated/Export/ServerData/";
    private static readonly string[] ReserializeAssetPaths =
    {
        "Assets/Game/Resources/Definitions/ScheduledEventDates",
        "Assets/Game/World/Resources/Definitions/Scene"
    };

    [MenuItem("Project/Server Data/Generate JSONs")]
    private static void GenerateFromMenu()
    {
        Generate(OutputDirectory);
    }

    private static void Generate(string selectedOutputDirectory)
    {
        if (string.IsNullOrWhiteSpace(selectedOutputDirectory))
        {
            EditorUtility.DisplayDialog("Server Data JSON", "Choose an output directory.", "OK");
            return;
        }

        string absoluteOutputDirectory = GetAbsolutePath(selectedOutputDirectory);
        if (Directory.Exists(absoluteOutputDirectory))
        {
            Directory.Delete(absoluteOutputDirectory, true);
        }
        Directory.CreateDirectory(absoluteOutputDirectory);
        List<StaticGameDataDefinitionConfig> configs = GetExportableConfigs();
        List<string> results = new List<string>();
        try
        {
            ForceReserializeServerDataAssets();

            for (int i = 0; i < configs.Count; i++)
            {
                StaticGameDataDefinitionConfig config = configs[i];
                EditorUtility.DisplayProgressBar("Generating Server Data JSON", config.ExportPath, (float)i / configs.Count);
                Type definitionType = GetDefinitionType(config);
                if (definitionType == null)
                {
                    Debug.LogWarning("Skipped " + config.name + ": definition type could not be resolved.");
                    continue;
                }

                List<StaticGameDataDefinition> definitions = GetDefinitions(config.DefinitionPath, definitionType);
                string json = SerializeDefinitions(definitions);
                string outputPath = Path.Combine(absoluteOutputDirectory, config.ExportPath.Replace('/', Path.DirectorySeparatorChar) + ".json");
                string outputParentDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputParentDirectory))
                {
                    Directory.CreateDirectory(outputParentDirectory);
                }
                File.WriteAllText(outputPath, json);
                results.Add(config.ExportPath + ".json (" + definitions.Count + ")");
            }

            EditorUtility.DisplayProgressBar("Generating Server Data JSON", "pickupables", 1f);
            SceneCollectibleExport collectibleExport = GetSceneCollectibleExport();
            WriteJsonFile(absoluteOutputDirectory, "pickupables", collectibleExport.Pickupables);
            WriteJsonFile(absoluteOutputDirectory, "pickupablegroups", collectibleExport.PickupableGroups);
            results.Add("pickupables.json (" + collectibleExport.PickupableCount + ")");
            results.Add("pickupablegroups.json (" + collectibleExport.GroupCount + ")");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Server Data JSON", "Generation failed. See the Console for details.", "OK");
            return;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        if (IsProjectPath(absoluteOutputDirectory))
        {
            AssetDatabase.Refresh();
        }
        EditorUtility.DisplayDialog("Server Data JSON", "Generated " + results.Count + " JSON files in:\n" + absoluteOutputDirectory, "OK");
    }

    private static void ForceReserializeServerDataAssets()
    {
        string[] assetPaths = AssetDatabase.FindAssets("", ReserializeAssetPaths)
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => !AssetDatabase.IsValidFolder(path))
            .ToArray();

        if (assetPaths.Length == 0)
        {
            return;
        }

        AssetDatabase.ForceReserializeAssets(assetPaths, ForceReserializeAssetsOptions.ReserializeAssets);
        AssetDatabase.SaveAssets();
    }

    private static void WriteJsonFile(string absoluteOutputDirectory, string exportPath, JObject json)
    {
        string outputPath = Path.Combine(absoluteOutputDirectory, "Assets", "Generated", "Export", exportPath + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, json.ToString(Formatting.None));
    }

    private static SceneCollectibleExport GetSceneCollectibleExport()
    {
        JObject pickupables = new JObject();
        JObject pickupableGroups = new JObject();
        JsonSerializer serializer = JsonSerializer.CreateDefault();
        List<string> scenePaths = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(IsEditableProjectScene)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
        int pickupableCount = 0;
        int groupCount = 0;

        foreach (string scenePath in scenePaths)
        {
            Scene scene = default(Scene);
            bool closeScene = false;
            try
            {
                scene = SceneManager.GetSceneByPath(scenePath);
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    closeScene = true;
                }

                JObject scenePickupables = new JObject();
                Dictionary<string, JArray> sceneGroups = new Dictionary<string, JArray>(StringComparer.Ordinal);
                foreach (GameObject rootObject in scene.GetRootGameObjects())
                {
                    foreach (Collectible collectible in rootObject.GetComponentsInChildren<Collectible>(true))
                    {
                        JObject record;
                        try
                        {
                            record = SerializeCollectible(collectible, serializer);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogWarning("Skipped collectible " + collectible.gameObject.GetPath() + " in " + scenePath + ": " + exception.Message);
                            continue;
                        }
                        string path = collectible.gameObject.GetPath();
                        scenePickupables[path] = record;
                        pickupableCount++;

                        string groupPath = GetValueGroupPath(collectible.transform);
                        if (!string.IsNullOrEmpty(groupPath))
                        {
                            if (!sceneGroups.TryGetValue(groupPath, out JArray group))
                            {
                                group = new JArray();
                                sceneGroups.Add(groupPath, group);
                            }
                            group.Add(record);
                        }
                    }
                }

                if (scenePickupables.Count > 0)
                {
                    pickupables[Path.GetFileNameWithoutExtension(scenePath)] = scenePickupables;
                }
                if (sceneGroups.Count > 0)
                {
                    JArray groups = new JArray();
                    foreach (KeyValuePair<string, JArray> sceneGroup in sceneGroups.OrderBy(pair => pair.Key, StringComparer.Ordinal))
                    {
                        groups.Add(new JObject
                        {
                            ["group"] = sceneGroup.Value,
                            ["path"] = sceneGroup.Key
                        });
                    }
                    pickupableGroups[Path.GetFileNameWithoutExtension(scenePath)] = groups;
                    groupCount += groups.Count;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Skipped scene " + scenePath + ": " + exception.Message);
            }
            finally
            {
                if (closeScene)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        return new SceneCollectibleExport(pickupables, pickupableGroups, pickupableCount, groupCount);
    }

    private static bool IsEditableProjectScene(string scenePath)
    {
        if (!scenePath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) || !AssetDatabase.IsOpenForEdit(scenePath))
        {
            return false;
        }
        return true;
    }

    private static JObject SerializeCollectible(Collectible collectible, JsonSerializer serializer)
    {
        Bounds bounds = GetCollectibleBounds(collectible.gameObject);
        return new JObject
        {
            ["maxBounds"] = SerializeVector3(bounds.max),
            ["minBounds"] = SerializeVector3(bounds.min),
            ["path"] = collectible.gameObject.GetPath(),
            ["reward"] = collectible.RewardDef == null ? JValue.CreateNull() : JToken.FromObject(collectible.RewardDef.ToReward(), serializer),
            ["tag"] = collectible.gameObject.tag,
            ["type"] = 0
        };
    }

    private static JObject SerializeVector3(Vector3 value)
    {
        return new JObject
        {
            ["x"] = value.x,
            ["y"] = value.y,
            ["z"] = value.z
        };
    }

    private static Bounds GetCollectibleBounds(GameObject collectible)
    {
        Collider collider = collectible.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds;
        }
        Renderer renderer = collectible.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        return new Bounds(collectible.transform.position, Vector3.zero);
    }

    private static string GetValueGroupPath(Transform collectibleTransform)
    {
        Transform current = collectibleTransform.parent;
        while (current != null)
        {
            if (current.name == "Value Groups Container" && current.parent != null)
            {
                return GetImmediateChildPath(current, collectibleTransform);
            }
            current = current.parent;
        }
        return string.Empty;
    }

    private static string GetImmediateChildPath(Transform groupContainer, Transform collectibleTransform)
    {
        Transform current = collectibleTransform;
        while (current.parent != groupContainer && current.parent != null)
        {
            current = current.parent;
        }
        return current.GetPath();
    }

    private sealed class SceneCollectibleExport
    {
        public readonly JObject Pickupables;
        public readonly JObject PickupableGroups;
        public readonly int PickupableCount;
        public readonly int GroupCount;

        public SceneCollectibleExport(JObject pickupables, JObject pickupableGroups, int pickupableCount, int groupCount)
        {
            Pickupables = pickupables;
            PickupableGroups = pickupableGroups;
            PickupableCount = pickupableCount;
            GroupCount = groupCount;
        }
    }

    private static List<StaticGameDataDefinitionConfig> GetExportableConfigs()
    {
        return AssetDatabase.FindAssets("t:StaticGameDataDefinitionConfig")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<StaticGameDataDefinitionConfig>)
            .Where(config => config != null && config.IsExportable && !string.IsNullOrEmpty(config.DefinitionPath) && !string.IsNullOrEmpty(config.ExportPath))
            .OrderBy(config => config.ExportPath, StringComparer.Ordinal)
            .ToList();
    }

    private static Type GetDefinitionType(StaticGameDataDefinitionConfig config)
    {
        string typeName = config.name.EndsWith("Config", StringComparison.Ordinal) ? config.name.Substring(0, config.name.Length - "Config".Length) : config.name;
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetTypes)
            .FirstOrDefault(type => type.Name == typeName && typeof(StaticGameDataDefinition).IsAssignableFrom(type));
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type != null);
        }
    }

    private static List<StaticGameDataDefinition> GetDefinitions(string definitionPath, Type definitionType)
    {
        if (!AssetDatabase.IsValidFolder(definitionPath))
        {
            Debug.LogWarning("Definition path does not exist: " + definitionPath);
            return new List<StaticGameDataDefinition>();
        }

        FieldInfo idField = StaticGameDataDefinitionIdAttribute.GetAttributedField(definitionType);
        return AssetDatabase.FindAssets("", new[] { definitionPath })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(path => AssetDatabase.LoadAssetAtPath(path, definitionType) as StaticGameDataDefinition)
            .Where(definition => definition != null && definitionType.IsAssignableFrom(definition.GetType()))
            .OrderBy(definition => GetSortValue(definition, idField), StringComparer.Ordinal)
            .ThenBy(definition => AssetDatabase.GetAssetPath(definition), StringComparer.Ordinal)
            .ToList();
    }

    private static string GetSortValue(StaticGameDataDefinition definition, FieldInfo idField)
    {
        if (idField == null)
        {
            return AssetDatabase.GetAssetPath(definition);
        }
        object value = idField.GetValue(definition);
        if (value is IFormattable)
        {
            return ((IFormattable)value).ToString(null, System.Globalization.CultureInfo.InvariantCulture).PadLeft(20, '0');
        }
        return value == null ? string.Empty : value.ToString();
    }

    private static string SerializeDefinitions(IEnumerable<StaticGameDataDefinition> definitions)
    {
        JsonSerializer serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.None,
            ContractResolver = new FieldOnlyContractResolver()
        };
        serializer.Converters.Add(new StaticDefinitionReferenceConverter());
        JArray output = new JArray();
        foreach (StaticGameDataDefinition definition in definitions)
        {
            output.Add(SerializeDefinition(definition, serializer));
        }
        return output.ToString(Formatting.None);
    }

    private static JObject SerializeDefinition(StaticGameDataDefinition definition, JsonSerializer serializer)
    {
        JObject output = new JObject();
        foreach (FieldInfo field in GetSerializableFields(definition.GetType()).OrderBy(GetJsonPropertyName, StringComparer.Ordinal))
        {
            string propertyName = GetJsonPropertyName(field);
            object value = field.GetValue(definition);
            output[propertyName] = value == null ? JValue.CreateNull() : SerializeFieldValue(definition, field, value, serializer);
        }
        return output;
    }

    private static JToken SerializeFieldValue(StaticGameDataDefinition definition, FieldInfo field, object value, JsonSerializer serializer)
    {
        if (definition.GetType().Name == "GlobalChatPhrasesDefinition" && field.Name == "ChatPhraseDefinitions")
        {
            JArray phrases = new JArray();
            foreach (StaticGameDataDefinition phrase in (System.Collections.IEnumerable)value)
            {
                phrases.Add(phrase == null ? JValue.CreateNull() : SerializeDefinition(phrase, serializer));
            }
            return phrases;
        }
        return JToken.FromObject(value, serializer);
    }

    private static IEnumerable<FieldInfo> GetSerializableFields(Type type)
    {
        for (Type currentType = type; currentType != null && currentType != typeof(ScriptableObject); currentType = currentType.BaseType)
        {
            foreach (FieldInfo field in currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                if (!field.IsStatic && (field.IsPublic || field.GetCustomAttributes(typeof(JsonPropertyAttribute), true).Length > 0))
                {
                    yield return field;
                }
            }
        }
    }

    private static string GetJsonPropertyName(FieldInfo field)
    {
        JsonPropertyAttribute property = field.GetCustomAttributes(typeof(JsonPropertyAttribute), true).OfType<JsonPropertyAttribute>().FirstOrDefault();
        return property != null && !string.IsNullOrEmpty(property.PropertyName) ? property.PropertyName : field.Name;
    }

    private static string GetAbsolutePath(string path)
    {
        return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(Directory.GetParent(Application.dataPath).FullName, path));
    }

    private static bool IsProjectPath(string path)
    {
        string projectPath = AppendDirectorySeparator(Directory.GetParent(Application.dataPath).FullName);
        return AppendDirectorySeparator(path).StartsWith(projectPath, StringComparison.OrdinalIgnoreCase);
    }

    private static string AppendDirectorySeparator(string path)
    {
        return path.EndsWith(Path.DirectorySeparatorChar.ToString()) ? path : path + Path.DirectorySeparatorChar;
    }

    private class FieldOnlyContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, MemberSerialization.Fields)
                .Where(IsSerializableField)
                .OrderBy(property => property.PropertyName, StringComparer.Ordinal)
                .ToList();
        }

        private static bool IsSerializableField(Newtonsoft.Json.Serialization.JsonProperty property)
        {
            FieldInfo field = property.DeclaringType.GetField(property.UnderlyingName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field != null && (field.IsPublic || field.GetCustomAttributes(typeof(JsonPropertyAttribute), true).Length > 0);
        }
    }

    private class StaticDefinitionReferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(StaticGameDataDefinition).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            FieldInfo idField = StaticGameDataDefinitionIdAttribute.GetAttributedField(value.GetType());
            if (idField == null)
            {
                writer.WriteNull();
                return;
            }
            serializer.Serialize(writer, idField.GetValue(value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}
