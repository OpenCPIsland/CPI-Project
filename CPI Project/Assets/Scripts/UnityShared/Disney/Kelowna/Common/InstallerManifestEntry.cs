using System;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class InstallerManifestEntry
	{
		public string InstallerId;

		public string InstallerUrl;

		public string InstallResponseFileUrl;

		public string UninstallResponseFileUrl;

		public string Platform;

		public string Version;

		public string ContentHash;

		public Version GetVersion()
		{
			return new Version(Version);
		}
	}
}
