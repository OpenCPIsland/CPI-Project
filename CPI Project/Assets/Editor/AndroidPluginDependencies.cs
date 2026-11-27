#if UNITY_ANDROID
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Android;

public class AndroidPluginDependencies : IPostGenerateGradleAndroidProject
{
	private const string Dependency = "implementation 'org.bouncycastle:bcprov-jdk18on:1.87'";

	private const string LocalNetworkPermission = "android.permission.ACCESS_LOCAL_NETWORK";

	private const string ProguardRules = "-keep class org.bouncycastle.** { *; }\n-dontwarn org.bouncycastle.**\n-keep class com.disney.** { *; }\n-keep class com.cpisland.** { *; }\n";

	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	public void OnPostGenerateGradleAndroidProject(string path)
	{
		string gradle = Path.Combine(path, "build.gradle");
		if (File.Exists(gradle))
		{
			string text = File.ReadAllText(gradle);
			if (!text.Contains("org.bouncycastle:bcprov-jdk18on"))
			{
				int index = text.IndexOf("dependencies {");
				if (index >= 0)
				{
					index += "dependencies {".Length;
					text = text.Insert(index, "\n    " + Dependency);
					File.WriteAllText(gradle, text);
				}
			}
		}
		if (TargetSdk(path) >= 37)
		{
			AddPermission(Path.Combine(path, Path.Combine("src", Path.Combine("main", "AndroidManifest.xml"))), LocalNetworkPermission);
		}
		string proguard = Path.Combine(path, "proguard-unity.txt");
		if (File.Exists(proguard))
		{
			string rules = File.ReadAllText(proguard);
			if (!rules.Contains("org.bouncycastle"))
			{
				File.AppendAllText(proguard, "\n" + ProguardRules);
			}
		}
	}

	private static int TargetSdk(string path)
	{
		int configured = (int)PlayerSettings.Android.targetSdkVersion;
		if (configured > 0)
		{
			return configured;
		}
		string[] files = new string[2]
		{
			Path.Combine(path, "build.gradle"),
			Path.Combine(Path.Combine(Path.GetDirectoryName(path), "launcher"), "build.gradle")
		};
		foreach (string file in files)
		{
			if (!File.Exists(file))
			{
				continue;
			}
			Match match = Regex.Match(File.ReadAllText(file), "targetSdk(?:Version)?\\s*=?\\s*(\\d+)");
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value);
			}
		}
		return 0;
	}

	private static void AddPermission(string manifest, string permission)
	{
		if (!File.Exists(manifest))
		{
			return;
		}
		string text = File.ReadAllText(manifest);
		if (text.Contains("\"" + permission + "\""))
		{
			return;
		}
		int index = text.IndexOf("<application");
		if (index < 0)
		{
			index = text.LastIndexOf("</manifest>");
		}
		if (index < 0)
		{
			return;
		}
		text = text.Insert(index, "<uses-permission android:name=\"" + permission + "\" />\n  ");
		File.WriteAllText(manifest, text);
	}
}
#endif
