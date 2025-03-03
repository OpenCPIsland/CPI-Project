using System;
using System.Collections.Generic;
using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class VersionedFileManifest : IFileManifest
	{
		private const string VERSION_KEY = "version";

		private const string HASHES_KEY = "hashes";

		private IDictionary<string, object> hashes;

		private IDictionary<string, string> translatedUrls;

		private List<string> cdnRoots;

		private FmsOptions options;

		private bool isReady = false;

		private string version;

		public VersionedFileManifest()
		{
			translatedUrls = new Dictionary<string, string>();
		}

		public void Prepare(FmsOptions options, string json)
		{
			this.options = options;
			if (options.JSONParser.Parse(json))
			{
				IDictionary<string, object> dictionary = options.JSONParser.AsDictionary();
				hashes = (IDictionary<string, object>)dictionary["hashes"];
				if (dictionary["version"] != null)
				{
					version = dictionary["version"].ToString();
				}
				isReady = true;
			}
			Debug.Log(string.Format("Versioned manifest version #{0} is ready with {1} files.", version, hashes.Count));
		}

		public string TranslateFileUrl(string relativePath)
		{
			AssertReady();
			if (!translatedUrls.ContainsKey(relativePath))
			{
				if (!hashes.ContainsKey(relativePath))
				{
					Debug.LogWarning(string.Format("Unable to find '{0}' version in the manifest.", relativePath));
					return "";
				}
				string text = "{root}{codename}/{environment}/{relativePath}/{hash}.{filename}";
				text = text.Replace("{root}", options.BaseURL);
				text = text.Replace("{codename}", options.CodeName);
				text = text.Replace("{environment}", options.Env.ToString().ToLower());
				text = text.Replace("{relativePath}", relativePath);
				text = text.Replace("{hash}", hashes[relativePath].ToString());
				string newValue = relativePath.Substring(relativePath.LastIndexOf("/") + 1);
				text = text.Replace("{filename}", newValue);
				translatedUrls[relativePath] = text;
			}
			return translatedUrls[relativePath];
		}

		public int GetVersionFromFileUrl(string relativePath)
		{
			AssertReady();
			if (!hashes.ContainsKey(relativePath))
			{
				Debug.LogWarning(string.Format("Unable to find '{0}' url in the manifest.", relativePath));
				return 0;
			}
			string text = hashes[relativePath].ToString();
			return Convert.ToInt32(text.Substring(text.Length - 8), 16);
		}

		public string GetManifestVersion()
		{
			AssertReady();
			return version;
		}

		private void AssertReady()
		{
			if (!isReady)
			{
				throw new Exception("Versioned manifest is not ready. Call VersionedFileManifest.Prepare first.");
			}
		}
	}
}
