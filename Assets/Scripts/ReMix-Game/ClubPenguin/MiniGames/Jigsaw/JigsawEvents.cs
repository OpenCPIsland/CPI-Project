using System.Runtime.InteropServices;

namespace ClubPenguin.MiniGames.Jigsaw
{
	public static class JigsawEvents
	{
		public enum EventTypes
		{
			LockPiece,
			CloseButton
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CloseButton
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PieceSolveComplete
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AllPiecesSolved
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BackgroundSolveComplete
		{
		}
	}
}
