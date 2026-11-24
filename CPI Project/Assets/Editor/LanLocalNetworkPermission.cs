using System.IO;
using UnityEditor.Android;

public class LanLocalNetworkPermission : IPostGenerateGradleAndroidProject
{
	private const string Line = "<uses-permission android:name=\"android.permission.ACCESS_LOCAL_NETWORK\" />";

	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	public void OnPostGenerateGradleAndroidProject(string path)
	{
		string manifest = Path.Combine(path, "src", "main", "AndroidManifest.xml");
		if (!File.Exists(manifest))
		{
			return;
		}
		string text = File.ReadAllText(manifest);
		if (text.Contains("android.permission.ACCESS_LOCAL_NETWORK"))
		{
			return;
		}
		int end = text.LastIndexOf("</manifest>");
		if (end < 0)
		{
			return;
		}
		File.WriteAllText(manifest, text.Insert(end, "  " + Line + "\n"));
	}
}
