using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	public class BlockAndLockEvents : MonoBehaviour
	{
		public enum EventTypes
		{
			LockPiece,
			CloseButton,
			RestartButton,
			AllPiecesSolved,
			BackgroundSolveComplete,
			PuzzleInitialized
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CloseButton
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RestartButton
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

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PuzzleInitialized
		{
		}
	}
}
