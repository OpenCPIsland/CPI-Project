using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.Locomotion;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class DanceBattleQuestGameController : MonoBehaviour
	{
		private enum DanceBattleQuestGameState
		{
			ShowingMoves,
			InputtingMoves,
			Results
		}

		[Serializable]
		public struct RoundMoveList
		{
			public bool RandomizeMoves;

			public int MoveCount;

			public int[] Moves;

			public int NumMovesToShow;
		}

		private const string MOVE_ICON_INTRO_TRIGGER = "Intro";

		private const int FAILURE_MOVE_ID = 19;

		public PropDefinition DanceProp;

		public GameObject MoveIconContainer;

		public GameObject MoveIconInputContainer;

		public PrefabContentKey MoveIconKey;

		public GameObject[] Screens;

		public string RoundFailedEvent = "RoundFailed";

		public string RoundSuccessEvent = "RoundSuccess";

		public UITimer InputMovesTimer;

		public CameraController DanceCamera;

		public Transform DanceCameraTarget;

		public CameraController ScreenCamera;

		public Transform ScreenCameraTarget;

		public CameraController PCDanceCamera;

		public Transform PCDanceCameraTarget;

		public CameraController PCScreenCamera;

		public Transform PCScreenCameraTarget;

		private CameraController currentCameraController;

		private StateMachineContext trayFSMContext;

		public float DanceIconDelay = 1f;

		public float DanceIconEndDelay = 0f;

		public float InputTime = 5f;

		public float DanceMoveTime = 3f;

		public float PenguinDanceStartDelay = 0f;

		public float PenguinDanceEndDelay = 0f;

		public RoundMoveList[] RoundMovesList;

		private List<int> movesList;

		private List<int> inputMovesList;

		private DanceBattleQuestGameState currentState;

		private int currentRound;

		private GameObject moveIconPrefab;

		private List<DanceBattleMoveIcon> moveIconList;

		private EventDispatcher dispatcher;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				trayFSMContext = gameObject.GetComponent<StateMachineContext>();
			}
			Content.LoadAsync(onMoveIconLoaded, MoveIconKey);
			movesList = new List<int>();
			inputMovesList = new List<int>();
			moveIconList = new List<DanceBattleMoveIcon>();
			dispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
			CoroutineRunner.StopAllForOwner(this);
			changeToDefaultCamera();
			LocomotionUtils.UnEquipProp(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject);
		}

		public void StartGame()
		{
			equipProp();
			currentRound = 0;
			startRound();
		}

		public void EndGame()
		{
			InputMovesTimer.StopCountdown();
			Screens[(int)currentState].SetActive(false);
			changeToDefaultCamera();
		}

		private void onMoveIconLoaded(string path, GameObject prefab)
		{
			moveIconPrefab = prefab;
		}

		private void equipProp()
		{
			if (DanceProp != null && !string.IsNullOrEmpty(DanceProp.GetNameOnServer()))
			{
				Service.Get<PropService>().LocalPlayerRetrieveProp(DanceProp.GetNameOnServer());
			}
		}

		private void SetState(DanceBattleQuestGameState newState)
		{
			Screens[(int)currentState].SetActive(false);
			Screens[(int)newState].SetActive(true);
			currentState = newState;
			switch (newState)
			{
			case DanceBattleQuestGameState.ShowingMoves:
				startTurnSequence();
				changeCameraController(getScreenCamera(), getScreenCameraTarget(), true);
				break;
			case DanceBattleQuestGameState.InputtingMoves:
			{
				destroyMoveIcons();
				inputMovesList.Clear();
				InputMovesTimer.StartCountdown(InputTime);
				int currentMoveCount = getCurrentMoveCount();
				for (int i = 0; i < currentMoveCount; i++)
				{
					DanceBattleMoveIcon component = UnityEngine.Object.Instantiate(moveIconPrefab, MoveIconInputContainer.transform, false).GetComponent<DanceBattleMoveIcon>();
					component.SetState(DanceBattleMoveIcon.MoveIconState.BlankRed);
					moveIconList.Add(component);
				}
				CoroutineRunner.Start(setStateAfterDelay(InputTime, DanceBattleQuestGameState.Results), this, "");
				changeCameraController(getDanceCamera(), getDanceCameraTarget(), false);
				break;
			}
			case DanceBattleQuestGameState.Results:
				InputMovesTimer.StopCountdown();
				CoroutineRunner.Start(showRoundResults(), this, "");
				changeCameraController(getDanceCamera(), getDanceCameraTarget(), true);
				break;
			}
		}

		private IEnumerator setStateAfterDelay(float delay, DanceBattleQuestGameState state)
		{
			yield return new WaitForSeconds(delay);
			SetState(state);
		}

		private void danceAnimationsComplete()
		{
			currentRound++;
			startRound();
		}

		private void startRound()
		{
			if (currentRound > RoundMovesList.Length - 1)
			{
				EndGame();
				return;
			}
			generateMovesList();
			SetState(DanceBattleQuestGameState.ShowingMoves);
		}

		private void generateMovesList()
		{
			movesList.Clear();
			if (RoundMovesList[currentRound].RandomizeMoves)
			{
				int moveCount = RoundMovesList[currentRound].MoveCount;
				for (int i = 0; i < moveCount; i++)
				{
					movesList.Add(UnityEngine.Random.Range(1, 4));
				}
			}
			else
			{
				movesList.AddRange(RoundMovesList[currentRound].Moves);
			}
		}

		private void startTurnSequence()
		{
			CoroutineRunner.Start(showTurnSequence(), this, "ShowDanceBattleTurnSequence");
		}

		private IEnumerator showTurnSequence()
		{
			destroyMoveIcons();
			for (int i = 0; i < movesList.Count; i++)
			{
				DanceBattleMoveIcon moveIcon = UnityEngine.Object.Instantiate(moveIconPrefab, MoveIconContainer.transform, false).GetComponent<DanceBattleMoveIcon>();
				switch (movesList[i])
				{
				case 1:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon1);
					break;
				case 2:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon2);
					break;
				case 3:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon3);
					break;
				}
				moveIcon.GetComponent<Animator>().SetTrigger("Intro");
				moveIconList.Add(moveIcon);
				EventManager.Instance.PostEvent("SFX/UI/DanceGame/NoteAppear", EventAction.PlaySound, null, null);
				yield return new WaitForSeconds(DanceIconDelay);
			}
			yield return new WaitForSeconds(DanceIconEndDelay);
			SetState(DanceBattleQuestGameState.InputtingMoves);
		}

		private void destroyMoveIcons()
		{
			for (int num = MoveIconContainer.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(MoveIconContainer.transform.GetChild(num).gameObject);
			}
			for (int num2 = MoveIconInputContainer.transform.childCount - 1; num2 >= 0; num2--)
			{
				UnityEngine.Object.Destroy(MoveIconInputContainer.transform.GetChild(num2).gameObject);
			}
			moveIconList.Clear();
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (currentState == DanceBattleQuestGameState.InputtingMoves)
			{
				switch (evt.Action)
				{
				case InputEvents.Actions.Action1:
					moveIconList[inputMovesList.Count].SetState(DanceBattleMoveIcon.MoveIconState.Icon1);
					inputMovesList.Add(1);
					break;
				case InputEvents.Actions.Action2:
					moveIconList[inputMovesList.Count].SetState(DanceBattleMoveIcon.MoveIconState.Icon2);
					inputMovesList.Add(2);
					break;
				case InputEvents.Actions.Action3:
					moveIconList[inputMovesList.Count].SetState(DanceBattleMoveIcon.MoveIconState.Icon3);
					inputMovesList.Add(3);
					break;
				}
				if (inputMovesList.Count < movesList.Count)
				{
					EventManager.Instance.PostEvent("SFX/UI/DanceGame/Note1", EventAction.PlaySound, null, null);
				}
				else
				{
					EventManager.Instance.PostEvent("SFX/UI/DanceGame/Note2", EventAction.PlaySound, null, null);
				}
			}
			return false;
		}

		private IEnumerator showRoundResults()
		{
			yield return new WaitForSeconds(PenguinDanceStartDelay);
			int correctMoves = 0;
			for (int i = 0; i < inputMovesList.Count && inputMovesList[i] == movesList[i]; i++)
			{
				correctMoves++;
			}
			float correctPercent = (float)correctMoves / (float)movesList.Count;
			int totalMoves = RoundMovesList[currentRound].NumMovesToShow;
			int movesToShow = Mathf.FloorToInt((float)totalMoves * correctPercent);
			CoroutineRunner.Start(playDanceAnimations(movesToShow, totalMoves), this, "");
			if (correctPercent < 1f)
			{
				Service.Get<QuestService>().SendEvent(RoundFailedEvent);
			}
			else
			{
				Service.Get<QuestService>().SendEvent(RoundSuccessEvent);
			}
		}

		private IEnumerator playDanceAnimations(int numCorrectMoves, int totalMoves)
		{
			for (int i = 0; i < numCorrectMoves; i++)
			{
				playDanceAnimation();
				yield return new WaitForSeconds(DanceMoveTime);
			}
			int numMovesLeft = totalMoves - numCorrectMoves;
			if (numMovesLeft > 0)
			{
				playDanceAnimation(19);
				yield return new WaitForSeconds(DanceMoveTime * (float)numMovesLeft);
			}
			yield return new WaitForSeconds(PenguinDanceEndDelay);
			danceAnimationsComplete();
		}

		private void playDanceAnimation(int danceMoveId)
		{
			Animator component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
			component.SetInteger(AnimationHashes.Params.Emote, danceMoveId);
			component.SetTrigger(AnimationHashes.Params.PlayEmote);
		}

		private void playDanceAnimation()
		{
			int value = UnityEngine.Random.Range(1, 16);
			Animator component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
			component.SetInteger(AnimationHashes.Params.Emote, value);
			component.SetTrigger(AnimationHashes.Params.PlayEmote);
		}

		private int getCurrentMoveCount()
		{
			return RoundMovesList[currentRound].RandomizeMoves ? RoundMovesList[currentRound].MoveCount : RoundMovesList[currentRound].Moves.Length;
		}

		private void changeCameraController(CameraController controller, Transform cameraTarget, bool hideControls)
		{
			if (currentCameraController != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = currentCameraController;
				dispatcher.DispatchEvent(evt);
			}
			CinematographyEvents.CameraLogicChangeEvent evt2 = default(CinematographyEvents.CameraLogicChangeEvent);
			evt2.Controller = controller;
			currentCameraController = controller;
			dispatcher.DispatchEvent(evt2);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(cameraTarget));
			if (hideControls)
			{
				trayFSMContext.SendEvent(new ExternalEvent("Root", "minnpc"));
			}
			else
			{
				trayFSMContext.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
			}
		}

		private void changeToDefaultCamera()
		{
			if (currentCameraController != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = currentCameraController;
				dispatcher.DispatchEvent(evt);
				dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform));
				currentCameraController = null;
			}
		}

		private CameraController getDanceCamera()
		{
			return (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape) ? PCDanceCamera : DanceCamera;
		}

		private CameraController getScreenCamera()
		{
			return (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape) ? PCScreenCamera : ScreenCamera;
		}

		private Transform getDanceCameraTarget()
		{
			return (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape) ? PCDanceCameraTarget : DanceCameraTarget;
		}

		private Transform getScreenCameraTarget()
		{
			return (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape) ? PCScreenCameraTarget : ScreenCameraTarget;
		}
	}
}
