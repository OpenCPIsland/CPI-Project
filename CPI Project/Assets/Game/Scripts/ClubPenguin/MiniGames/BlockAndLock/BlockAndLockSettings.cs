using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	[ExecuteInEditMode]
	public class BlockAndLockSettings : MonoBehaviour
	{
		[Header("Grid Layout")]
		public int GridWidth = 6;

		public int GridHeight = 10;

		public float GridAdjustHorz = 0f;

		public float GridAdjustVert = 3f;

		public GameObject MarkerTopLeft;

		public GameObject MarkerBottomRight;

		[Header("Localization")]
		[LocalizationToken]
		public string RestartToken = "GlobalUI.Adventure.Menu.Restart";

		[Header("Movement Timing")]
		public float MoveTimePerCell = 0.09f;

		public float MoveDecayPerCell = 0.01f;

		public iTween.EaseType PieceMoveEaseType = iTween.EaseType.easeOutCubic;

		[Header("Minigame Control")]
		public iTween.EaseType MinigameIntroEaseType = iTween.EaseType.easeOutCubic;

		public iTween.EaseType MinigameSolvedEaseType = iTween.EaseType.easeInCubic;

		public float MinigameTweenTime = 0.5f;

		public Vector3 IntroOffset = new Vector3(0f, -120f, 0f);

		[Header("Audio - Puzzle Events")]
		public string audioPuzzleSlideIn = "SFX/UI/Item/WhooshIn";

		public string audioPuzzleSlideOut = "SFX/UI/Item/WhooshOut";

		public string audioSelectPiece;

		public string audioDeselectPiece;

		public string audioStopObstacle;

		public string audioStopPiece;

		public string audioLockPiece;

		public string audioMovePiece;

		public string audioArrowTapped;

		public string audioRestartButton;

		[Header("Audio - Puzzle Solved - Stinger Support")]
		public string audioPuzzleSolved = "MUS/Quest/Rockhopper/Stinger/ObjCompleteHarp";

		public string switchTo = "";

		public GameObject switchGameObject;

		[Header("Audio - Prefab")]
		public GameObject AudioPrefab;

		[Header("Particles - Complete")]
		public GameObject completeParticleObj;

		public Color completeParticleColour = Color.white;

		public Vector3 completeOffset = Vector3.zero;

		[Header("Particles - Restart")]
		public GameObject restartParticleObj;

		public Color restartParticleColour = Color.white;

		public Vector3 restartOffset = Vector3.zero;

		public bool HideLockedPiecesOnSolve = false;

		[HideInInspector]
		public float GridUnitWidth = 0f;

		[HideInInspector]
		public float GridUnitHeight = 0f;

		private void Awake()
		{
			CalculateGrid();
		}

		private void OnValidate()
		{
			CalculateGrid();
		}

		private void CalculateGrid()
		{
			if (MarkerTopLeft == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockSettings.Start: ERROR -- GridTopLeft marker needs to be set"));
				return;
			}
			if (MarkerBottomRight == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockSettings.Start: ERROR -- GridBottomRight marker needs to be set"));
				return;
			}
			float num = Mathf.Abs(MarkerBottomRight.transform.localPosition.x - MarkerTopLeft.transform.localPosition.x);
			float num2 = Mathf.Abs(MarkerBottomRight.transform.localPosition.y - MarkerTopLeft.transform.localPosition.y);
			GridUnitWidth = num / ((float)GridWidth - 1f);
			GridUnitHeight = num2 / ((float)GridHeight - 1f);
		}
	}
}
