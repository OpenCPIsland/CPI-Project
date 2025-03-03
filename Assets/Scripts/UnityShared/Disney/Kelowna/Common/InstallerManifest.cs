using System;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class InstallerManifest
	{
		public InstallerManifestEntry[] Entries;

		public static string CurrentPlatform
		{
			get
			{
				switch (IntPtr.Size)
				{
				case 8:
					return "windows64";
				case 4:
					return "windows32";
				default:
					throw new Exception("ERROR - InstallerManifest.CurrentPlatform: unknown Windows OS bit size!");
				}
			}
		}
	}
}
