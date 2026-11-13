using ClubPenguin.Core;
using System.Runtime.InteropServices;

namespace ClubPenguin.Igloo
{
	public static class IglooMusicEvents
	{
		public struct PreviewMusicTrack
		{
			public readonly MusicTrackDefinition Definition;

			public PreviewMusicTrack(MusicTrackDefinition definition)
			{
				Definition = definition;
			}
		}

		public struct SetMusicTrack
		{
			public readonly MusicTrackDefinition Definition;

			public SetMusicTrack(MusicTrackDefinition definition)
			{
				Definition = definition;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StopPreviewMusic
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StopAllMusic
		{
		}
	}
}
