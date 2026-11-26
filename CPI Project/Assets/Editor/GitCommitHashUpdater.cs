using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GitCommitHashUpdater : IPreprocessBuildWithReport
{
	private const string Url = "https://api.github.com/repos/OpenCPIsland/CPI-Project/commits/develop";

	private const string AssetPath = "Assets/Generated/Resources/GitCommit.txt";

	private const int ShortLength = 7;

	private const string ProgressTitle = "Updating Git Commit Hash";

	private const float TimeoutSeconds = 15f;

	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	[MenuItem("Project/Update Git Commit Hash")]
	public static void UpdateFromMenu()
	{
		string hash = Refresh();
		if (hash != null)
		{
		}
	}

	public void OnPreprocessBuild(BuildReport report)
	{
		Refresh();
	}

	public static string Refresh()
	{
		try
		{
			string hash = Fetch();
			if (hash == null)
			{
				Debug.LogWarning("[GitCommit] Could not get the latest commit from " + Url + ". Keeping the saved hash.");
				return null;
			}
			EditorUtility.DisplayProgressBar(ProgressTitle, "Saving " + hash + " to " + AssetPath, 0.95f);
			string current = File.Exists(AssetPath) ? File.ReadAllText(AssetPath).Trim() : null;
			if (current != hash)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
				File.WriteAllText(AssetPath, hash);
				AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
			}
			EditorUtility.DisplayProgressBar(ProgressTitle, "Done", 1f);
			return hash;
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}
	}

	private static string Fetch()
	{
		try
		{
			using (HttpClient client = new HttpClient())
			using (CancellationTokenSource cancel = new CancellationTokenSource())
			{
				client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
				client.DefaultRequestHeaders.UserAgent.ParseAdd("CPI-Project-Editor");
				client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.sha");
				Task<HttpResponseMessage> request = client.GetAsync(Url, cancel.Token);
				DateTime started = DateTime.UtcNow;
				while (!request.IsCompleted)
				{
					float elapsed = (float)(DateTime.UtcNow - started).TotalSeconds;
					if (EditorUtility.DisplayCancelableProgressBar(ProgressTitle, "Getting the latest commit from OpenCPIsland/CPI-Project (develop)", Mathf.Clamp01(elapsed / TimeoutSeconds) * 0.9f))
					{
						cancel.Cancel();
						Debug.LogWarning("[GitCommit] Cancelled.");
						return null;
					}
					Thread.Sleep(50);
				}
				HttpResponseMessage response = request.GetAwaiter().GetResult();
				if (!response.IsSuccessStatusCode)
				{
					Debug.LogWarning("[GitCommit] GitHub returned " + (int)response.StatusCode + " " + response.ReasonPhrase);
					return null;
				}
				EditorUtility.DisplayProgressBar(ProgressTitle, "Reading commit", 0.9f);
				string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult().Trim();
				Match match = Regex.Match(body, "[0-9a-fA-F]{40}");
				if (!match.Success)
				{
					return null;
				}
				return match.Value.Substring(0, ShortLength).ToLowerInvariant();
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning("[GitCommit] " + e.Message);
			return null;
		}
	}
}
