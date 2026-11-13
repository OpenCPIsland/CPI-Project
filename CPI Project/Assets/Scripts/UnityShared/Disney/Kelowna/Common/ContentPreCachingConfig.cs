using UnityEngine;

namespace Disney.Kelowna.Common
{
	[CreateAssetMenu]
	public class ContentPreCachingConfig : ScriptableObject
	{
		public uint BundlePrecacheSeconds = 180u;

		public uint MaxConcurrentForegroundDownloads = 20u;

		public uint MaxConcurrentBackgroundDownloads = 1u;
	}
}
