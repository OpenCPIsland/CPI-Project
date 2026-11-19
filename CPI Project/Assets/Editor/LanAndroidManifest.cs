#if UNITY_ANDROID
using System.IO;
using UnityEditor.Android;

public class LanAndroidManifest : IPostGenerateGradleAndroidProject
{
	private static readonly string[] Permissions = new string[2]
	{
		"android.permission.INTERNET",
		"android.permission.CHANGE_WIFI_MULTICAST_STATE"
	};

	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	public void OnPostGenerateGradleAndroidProject(string path)
	{
		string manifest = Path.Combine(Path.Combine(Path.Combine(path, "src"), "main"), "AndroidManifest.xml");
		if (!File.Exists(manifest))
		{
			return;
		}
		string text = File.ReadAllText(manifest);
		string original = text;
		foreach (string permission in Permissions)
		{
			if (text.Contains("\"" + permission + "\""))
			{
				continue;
			}
			int at = text.IndexOf("<application");
			if (at < 0)
			{
				at = text.LastIndexOf("</manifest>");
			}
			if (at < 0)
			{
				return;
			}
			text = text.Insert(at, "<uses-permission android:name=\"" + permission + "\" />\n  ");
		}
		if (text != original)
		{
			File.WriteAllText(manifest, text);
		}
	}
}
#endif
