using System.Runtime.InteropServices;

namespace ClubPenguin.MiniGames.Jigsaw
{
	public static class JigsawEventsSprite
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

		public struct Register
		{
			public readonly int Id;

			public Register(int id)
			{
				Id = id;
			}
		}

		public struct RegisterRespond
		{
			public readonly int Id;

			public RegisterRespond(int id)
			{
				Id = id;
			}
		}
	}
}
