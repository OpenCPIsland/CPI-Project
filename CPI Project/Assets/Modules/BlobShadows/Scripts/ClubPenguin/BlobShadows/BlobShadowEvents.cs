using System.Runtime.InteropServices;

namespace ClubPenguin.BlobShadows
{
	public static class BlobShadowEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableBlobShadows
		{
		}

		public struct DisableBlobShadows
		{
			public readonly bool IncludeLocalPlayerShadow;

			public DisableBlobShadows(bool includeLocalPlayerShadow)
			{
				IncludeLocalPlayerShadow = includeLocalPlayerShadow;
			}
		}
	}
}
