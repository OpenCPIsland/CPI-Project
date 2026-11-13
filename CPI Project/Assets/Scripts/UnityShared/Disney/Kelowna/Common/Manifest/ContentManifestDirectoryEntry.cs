using System;

namespace Disney.Kelowna.Common.Manifest
{
	public class ContentManifestDirectoryEntry
	{
		public string client;

		public string platform;

		public string environment;

		public string content;

		public bool useEmbeddedContent = true;

		public string url;

		public string abtest;

		public string contentVersion;

		public string releaseScheduleName;

		public string releaseDate;

		public string decryptionKey;

		public bool IsEqualTo(string clientVersionStr, string platform, string environment)
		{
			return IsEqualTo(ClientInfo.ParseClientVersion(clientVersionStr), platform, environment);
		}

		public bool IsEqualTo(Version clientVersion, string platform, string environment)
		{
			Version version = ClientInfo.ParseClientVersion(client);
			return version != null && clientVersion != null && version.Equals(clientVersion) && this.platform.Equals(platform, StringComparison.Ordinal) && this.environment.Equals(environment, StringComparison.Ordinal);
		}

		public bool IsEqualTo(string platform, string environment)
		{
			return this.platform.Equals(platform, StringComparison.Ordinal) && this.environment.Equals(environment, StringComparison.Ordinal);
		}

		public bool IsEmbeddedContent()
		{
			if (useEmbeddedContent || string.IsNullOrEmpty(url) || content == ContentManifestDirectory.EMBEDDED_CONTENT)
			{
				return true;
			}
			return false;
		}

		public string ToDebugString()
		{
			return string.Format("client:{0}, platform:{1}, environment:{2}", client, platform, environment);
		}
	}
}
