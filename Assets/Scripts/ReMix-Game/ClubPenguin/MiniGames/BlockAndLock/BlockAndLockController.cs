using ClubPenguin.Adventure;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class BlockAndLockController : MonoBehaviour
	{
		public enum TouchPhaseExtended
		{
			Began,
			Moved,
			Stationary,
			Ended,
			Canceled,
			Mouse,
			NoEvent
		}

		private const string PLAYMAKER_MESG_SUCCESS = "BlockAndLockSuccess";

		private const string PLAYMAKER_MESG_FAILURE = "BlockAndLockFailed";

		private const float SWIPE_THRESHOLD_PERCENTAGE = 0.4f;

		private static Vector3 INVALID_COORDINATE = new Vector3(-1f, -1f, -1f);

		private EventDispatcher dispatcher;

		private QuestService questService;

		private BlockAndLockSettings settings;

		private Transform moveableContainer;

		private Transform solveContainer;

		private Transform obstacleContainer;

		private GameObject bkgArtworkObj;

		private Collider bkgArtworkColl;

		private GameObject beginArtworkObj;

		private GameObject completedArtworkObj;

		private GameObject completedAnchorObj;

		private RectTransform rectTransform;

		private Camera guiCam;

		private Vector3 oldPos;

		private Vector3 touchPositionOrigin;

		private GameObject oldFocus;

		private GameObject focusObj;

		private RaycastHit focusHit;

		private Vector3 pickupOffset;

		private bool hasInitalizedPuzzle;

		private bool isSolved;

		private int pieceCount;

		private int solveCount;

		private float xMin;

		private float xMax;

		private float yMin;

		private float yMax;

		private float screenSafetyZone = 0.5f;

		private BlockAndLockBoardData[,] gameBoard;

		private BlockAndLockBoardData[,] gameBoardSaved;

		private List<string> uniqueNames = new List<string>();

		private float swipeThreshold;

		private bool isTap = false;

		private GameObject previousSelection;

		private Transform arrowContainer;

		private GameObject arrowLeft;

		private GameObject arrowRight;

		private GameObject arrowUp;

		private GameObject arrowDown;

		private GameObject restartButton;

		private Grid2 turnArrowOff = new Grid2(-1, -1);

		private Dictionary<int, GameObject> solveObjects = new Dictionary<int, GameObject>();

		private Dictionary<int, GameObject> moveableObjects = new Dictionary<int, GameObject>();

		private static readonly int maxPieces = 32;

		private BlockAndLockBoardData[] prevBoardData = new BlockAndLockBoardData[maxPieces];

		private BlockAndLockBoardData currentContents = new BlockAndLockBoardData(PieceCategory.Empty, 0);

		private bool isMoving = false;

		private bool isRestarting = false;

		private bool isClosing = false;

		private bool isAutoSelectDone = false;

		private bool hasMadeMove = false;

		private Text fieldRestart;

		private EventChannel eventChannel;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(dispatcher);
			questService = Service.Get<QuestService>();
		}

		private void Start()
		{
			eventChannel.AddListener<BlockAndLockEvents.CloseButton>(onCloseButton);
			eventChannel.AddListener<BlockAndLockEvents.RestartButton>(onRestartButton);
			eventChannel.AddListener<BlockAndLockEvents.BackgroundSolveComplete>(onBackgroundSolveComplete);
			if (PuzzleInit())
			{
				playAudioEvent(settings.audioPuzzleSlideIn);
				iTween.MoveFrom(base.gameObject, iTween.Hash("position", base.gameObject.transform.position + settings.IntroOffset, "easeType", settings.MinigameIntroEaseType, "time", settings.MinigameTweenTime, "oncomplete", "onPuzzleInitComplete", "oncompletetarget", base.gameObject));
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			CoroutineRunner.StopAllForOwner(this);
		}

		private bool checkWasFound(object obj, string objectToFind)
		{
			UnityEngine.Object x = obj as UnityEngine.Object;
			if (obj == null || (obj != null && x == null))
			{
				throw new Exception("BlockAndLock - Could not find object: " + objectToFind);
			}
			return true;
		}

		public bool PuzzleInit()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
			rectTransform = GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(0f, 0f, -100f);
			swipeThreshold = Screen.dpi * 0.4f;
			if (swipeThreshold <= 0f)
			{
				swipeThreshold = 50f;
			}
			focusObj = null;
			oldFocus = null;
			hasInitalizedPuzzle = false;
			isSolved = false;
			pieceCount = 0;
			solveCount = 0;
			hasMadeMove = false;
			isTap = false;
			solveObjects.Clear();
			moveableObjects.Clear();
			settings = base.gameObject.GetComponentInParent<BlockAndLockSettings>();
			if (settings == null)
			{
				throw new MissingComponentException("Could not find BlockAndLockSettings component");
			}
			UnityEngine.Object.Instantiate(settings.AudioPrefab, base.transform);
			settings.MarkerTopLeft.SetActive(false);
			settings.MarkerBottomRight.SetActive(false);
			gameBoard = new BlockAndLockBoardData[settings.GridWidth, settings.GridHeight];
			for (int i = 0; i < settings.GridHeight; i++)
			{
				for (int j = 0; j < settings.GridWidth; j++)
				{
					gameBoard[j, i] = new BlockAndLockBoardData(PieceCategory.Empty, 0);
				}
			}
			for (int k = 0; k < maxPieces; k++)
			{
				prevBoardData[k] = new BlockAndLockBoardData(PieceCategory.Empty, 0);
			}
			bkgArtworkObj = base.gameObject.transform.Find("Common/Bkg common").gameObject;
			if (checkWasFound(bkgArtworkObj, "common background artwork"))
			{
				bkgArtworkColl = bkgArtworkObj.GetComponent<Collider>();
				if (bkgArtworkColl == null)
				{
					Log.LogError(this, string.Format("BlockAndLock -  could not find collider on background artwork"));
					return false;
				}
				bkgArtworkObj.layer = LayerMask.NameToLayer("UI");
			}
			beginArtworkObj = base.gameObject.transform.Find("Begin/Begin artwork").gameObject;
			if (checkWasFound(beginArtworkObj, "beginning artwork"))
			{
				beginArtworkObj.layer = LayerMask.NameToLayer("UI");
			}
			completedArtworkObj = base.gameObject.transform.Find("Completed/Completed Artwork").gameObject;
			if (checkWasFound(completedArtworkObj, "completed artwork"))
			{
				completedArtworkObj.layer = LayerMask.NameToLayer("UI");
				completedArtworkObj.SetActive(false);
			}
			completedAnchorObj = base.gameObject.transform.Find("Completed/Completed particles anchor").gameObject;
			if (checkWasFound(completedAnchorObj, "Completed particles anchor"))
			{
				completedAnchorObj.layer = LayerMask.NameToLayer("UI");
			}
			restartButton = base.gameObject.transform.Find("RestartBtn").gameObject;
			if (checkWasFound(restartButton, "restart button"))
			{
				restartButton.layer = LayerMask.NameToLayer("UI");
				restartButton.SetActive(false);
				fieldRestart = restartButton.GetComponentInChildren<Text>();
				if (checkWasFound(fieldRestart, "text field on the restart button"))
				{
					if (string.IsNullOrEmpty(settings.RestartToken))
					{
						throw new Exception("BlockAndLock - token for Restart text must be set");
					}
					fieldRestart.text = Service.Get<Localizer>().GetTokenTranslation(settings.RestartToken).ToUpper();
				}
			}
			arrowContainer = base.gameObject.transform.Find("Arrows");
			if (checkWasFound(arrowContainer, "arrow container"))
			{
				foreach (Transform item in arrowContainer)
				{
					GameObject gameObject = item.transform.gameObject;
					if (uniqueNames.Contains(gameObject.name))
					{
						Log.LogError(this, string.Format("Found duplicate arrow piece '{0}'. Please make them unique", gameObject.name));
						return false;
					}
					if (gameObject.name == "ArrowLeft")
					{
						arrowLeft = gameObject;
					}
					else if (gameObject.name == "ArrowRight")
					{
						arrowRight = gameObject;
					}
					else if (gameObject.name == "ArrowUp")
					{
						arrowUp = gameObject;
					}
					else
					{
						if (!(gameObject.name == "ArrowDown"))
						{
							Log.LogError(this, string.Format("Piece '{0}' is not an expected name. Please rename or delete it", gameObject.name));
							return false;
						}
						arrowDown = gameObject;
					}
					uniqueNames.Add(gameObject.name);
					if (gameObject.GetComponent<Collider>() == null)
					{
						gameObject.AddComponent<BoxCollider>();
					}
					BlockAndLockPiece blockAndLockPiece = gameObject.GetComponent<BlockAndLockPiece>();
					gameObject.layer = LayerMask.NameToLayer("UI");
					if (blockAndLockPiece == null)
					{
						blockAndLockPiece = gameObject.AddComponent<BlockAndLockPiece>();
					}
					blockAndLockPiece.Init(PieceCategory.Arrow);
				}
			}
			solveContainer = base.gameObject.transform.Find("Solve Positions");
			int result;
			if (checkWasFound(solveContainer, "solve container"))
			{
				foreach (Transform item2 in solveContainer)
				{
					GameObject gameObject = item2.transform.gameObject;
					if (uniqueNames.Contains(gameObject.name))
					{
						Log.LogError(this, string.Format("'{0}'. Please make them unique", gameObject.name));
						return false;
					}
					Grid2 grid = PositionToGrid(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
					if (gameBoard[grid.x, grid.y].Category != 0)
					{
						Log.LogError(this, string.Format("'{0}' can't go into grid position ({1}, {2}), it's not empty.", gameObject.name, grid.x, grid.y));
						return false;
					}
					uniqueNames.Add(gameObject.name);
					Match match = Regex.Match(gameObject.name, "\\(([0-9]+)\\)", RegexOptions.None);
					if (!match.Success)
					{
						Log.LogError(this, string.Format("'{0}' doesn't have an id, should be named like 'SolvedPostion (1)'.", gameObject.name, grid.x, grid.y));
						return false;
					}
					if (!int.TryParse(match.Groups[1].Value, out result))
					{
						Log.LogError(this, string.Format("'{0}' cannot parse matched value of (1)'.", gameObject.name, match.Groups[1].Value));
						return false;
					}
					gameBoard[grid.x, grid.y] = new BlockAndLockBoardData(PieceCategory.SolvePosition, result);
					if (gameObject.GetComponent<Collider>() == null)
					{
						gameObject.AddComponent<BoxCollider>();
					}
					gameObject.layer = LayerMask.NameToLayer("UI");
					solveObjects.Add(result, gameObject);
					BlockAndLockPiece blockAndLockPiece = gameObject.GetComponent<BlockAndLockPiece>();
					if (blockAndLockPiece == null)
					{
						blockAndLockPiece = gameObject.AddComponent<BlockAndLockPiece>();
					}
					blockAndLockPiece.Init(PieceCategory.SolvePosition);
				}
			}
			moveableContainer = base.gameObject.transform.Find("Moveable Positions");
			if (moveableContainer == null)
			{
				Log.LogError(this, string.Format("could not find moveable container object"));
				return false;
			}
			foreach (Transform item3 in moveableContainer)
			{
				GameObject gameObject = item3.transform.gameObject;
				if (uniqueNames.Contains(gameObject.name))
				{
					Log.LogError(this, string.Format("Found duplicate start piece '{0}'. Please make them unique", gameObject.name));
					return false;
				}
				Grid2 grid = PositionToGrid(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
				if (gameBoard[grid.x, grid.y].Category != 0)
				{
					Log.LogError(this, string.Format("'{0}' can't go into grid position ({1}, {2}), it's not empty.", gameObject.name, grid.x, grid.y));
					return false;
				}
				uniqueNames.Add(gameObject.name);
				Match match = Regex.Match(gameObject.name, "\\(([0-9]+)\\)", RegexOptions.None);
				if (!match.Success)
				{
					Log.LogError(this, string.Format("'{0}' doesn't have an id, should be named like 'StartPostion (1)'.", gameObject.name, grid.x, grid.y));
					return false;
				}
				if (!int.TryParse(match.Groups[1].Value, out result))
				{
					Log.LogError(this, string.Format("'{0}' cannot parse matched value of (1)'.", gameObject.name, match.Groups[1].Value));
					return false;
				}
				gameBoard[grid.x, grid.y] = new BlockAndLockBoardData(PieceCategory.MoveableObject, result);
				if (gameObject.GetComponent<Collider>() == null)
				{
					gameObject.AddComponent<BoxCollider>();
				}
				gameObject.layer = LayerMask.NameToLayer("UI");
				moveableObjects.Add(result, gameObject);
				BlockAndLockPiece blockAndLockPiece = gameObject.GetComponent<BlockAndLockPiece>();
				if (blockAndLockPiece == null)
				{
					blockAndLockPiece = gameObject.AddComponent<BlockAndLockPiece>();
				}
				blockAndLockPiece.Init(PieceCategory.MoveableObject);
			}
			obstacleContainer = base.gameObject.transform.Find("Obstacle Positions");
			if (obstacleContainer == null)
			{
				Log.LogError(this, string.Format("could not find obstacle container"));
				return false;
			}
			foreach (Transform item4 in obstacleContainer)
			{
				GameObject gameObject = item4.transform.gameObject;
				if (uniqueNames.Contains(gameObject.name))
				{
					Log.LogError(this, string.Format("Found duplicate obstacle piece '{0}'. Please make them unique", gameObject.name));
					return false;
				}
				Grid2 grid = PositionToGrid(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
				if (gameBoard[grid.x, grid.y].Category != 0)
				{
					Log.LogError(this, string.Format("'{0}' can't go into grid position ({1}, {2}), it's not empty.", gameObject.name, grid.x, grid.y));
					return false;
				}
				uniqueNames.Add(gameObject.name);
				Match match = Regex.Match(gameObject.name, "\\(([0-9]+)\\)", RegexOptions.None);
				if (!match.Success)
				{
					Log.LogError(this, string.Format("'O_o\t BlockAndLockController.Start: ''{0}' doesn't have an id, should be named like 'ObstaclePostion (1)'.", gameObject.name, grid.x, grid.y));
					return false;
				}
				if (!int.TryParse(match.Groups[1].Value, out result))
				{
					Log.LogError(this, string.Format("'{0}' cannot parse matched value of (1)'.", gameObject.name, match.Groups[1].Value));
					return false;
				}
				gameBoard[grid.x, grid.y] = new BlockAndLockBoardData(PieceCategory.Obstacle, result);
				if (gameObject.GetComponent<Collider>() == null)
				{
					gameObject.AddComponent<BoxCollider>();
				}
				gameObject.layer = LayerMask.NameToLayer("UI");
				BlockAndLockPiece blockAndLockPiece = gameObject.GetComponent<BlockAndLockPiece>();
				if (blockAndLockPiece == null)
				{
					blockAndLockPiece = gameObject.AddComponent<BlockAndLockPiece>();
				}
				blockAndLockPiece.Init(PieceCategory.Obstacle);
			}
			if (moveableObjects.Count != solveObjects.Count)
			{
				Log.LogError(this, string.Format("the number of moveable positions and solve positions must match ({0} != {1})", moveableObjects.Count, solveObjects.Count));
				return false;
			}
			pieceCount = solveObjects.Count;
			guiCam = GameObject.Find("PopupCamera").GetComponentInChildren<Camera>();
			if (guiCam == null)
			{
				Log.LogError(this, string.Format("Can't find the GUI camera"));
				return false;
			}
			CalculateScreenExtents();
			gameBoardSaved = new BlockAndLockBoardData[settings.GridWidth, settings.GridHeight];
			for (int i = 0; i < settings.GridHeight; i++)
			{
				for (int j = 0; j < settings.GridWidth; j++)
				{
					gameBoardSaved[j, i] = gameBoard[j, i];
				}
			}
			touchPositionOrigin = INVALID_COORDINATE;
			hasInitalizedPuzzle = true;
			return true;
		}

		private IEnumerator RestartGame()
		{
			if (isRestarting || isSolved || isClosing)
			{
				yield return null;
			}
			if (iTween.Count() > 0)
			{
				iTween.Stop();
				yield return new WaitForSeconds(0.1f);
			}
			playAudioEvent(settings.audioRestartButton);
			if (settings.restartParticleObj != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(settings.restartParticleObj, arrowContainer);
				gameObject.transform.localPosition = settings.restartOffset;
				gameObject.transform.localScale = Vector2.one;
				gameObject.layer = LayerMask.NameToLayer("UI");
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.SetStartColor(settings.restartParticleColour);
					component.Play();
				}
			}
			isRestarting = true;
			arrowLeft.transform.localScale = Vector3.one;
			arrowRight.transform.localScale = Vector3.one;
			arrowUp.transform.localScale = Vector3.one;
			arrowDown.transform.localScale = Vector3.one;
			setArrowState(arrowLeft, turnArrowOff);
			setArrowState(arrowRight, turnArrowOff);
			setArrowState(arrowUp, turnArrowOff);
			setArrowState(arrowDown, turnArrowOff);
			focusObj = null;
			oldFocus = null;
			previousSelection = null;
			isSolved = false;
			solveCount = 0;
			isAutoSelectDone = false;
			isMoving = false;
			isClosing = false;
			hasMadeMove = false;
			touchPositionOrigin = INVALID_COORDINATE;
			gameBoard = new BlockAndLockBoardData[settings.GridWidth, settings.GridHeight];
			for (int i = 0; i < settings.GridHeight; i++)
			{
				for (int j = 0; j < settings.GridWidth; j++)
				{
					BlockAndLockBoardData blockAndLockBoardData = gameBoardSaved[j, i];
					gameBoard[j, i] = blockAndLockBoardData;
					switch (blockAndLockBoardData.Category)
					{
					case PieceCategory.MoveableObject:
					{
						GameObject gameObject = moveableObjects[blockAndLockBoardData.Id];
						gameObject.SetActive(true);
						Vector2 vector = GridToPosition(j, i);
						gameObject.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
						BlockAndLockPiece component2 = gameObject.GetComponent<BlockAndLockPiece>();
						if (component2 != null)
						{
							component2.Reset();
						}
						break;
					}
					case PieceCategory.SolvePosition:
					{
						GameObject gameObject = solveObjects[blockAndLockBoardData.Id];
						gameObject.SetActive(true);
						Vector2 vector = GridToPosition(j, i);
						gameObject.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
						BlockAndLockPiece component2 = gameObject.GetComponent<BlockAndLockPiece>();
						if (component2 != null)
						{
							component2.Reset();
						}
						break;
					}
					}
				}
			}
			for (int k = 0; k < maxPieces; k++)
			{
				prevBoardData[k] = new BlockAndLockBoardData(PieceCategory.Empty, 0);
			}
			hasMadeMove = false;
			restartButton.SetActive(false);
			isRestarting = false;
		}

		private void CalculateScreenExtents()
		{
			Renderer component = bkgArtworkObj.GetComponent<Renderer>();
			if (component != null)
			{
				xMin = component.bounds.min.x + screenSafetyZone;
				yMin = component.bounds.min.y + screenSafetyZone;
				xMax = component.bounds.max.x - screenSafetyZone;
				yMax = component.bounds.max.y - screenSafetyZone;
			}
		}

		private void Update()
		{
			if (!hasInitalizedPuzzle || isSolved || isMoving || isRestarting || isClosing)
			{
				return;
			}
			Vector3 touchPosition;
			TouchPhaseExtended touchPhaseExtended = getTouchPhase(out touchPosition);
			if (moveableObjects.Count == 1 && !isAutoSelectDone)
			{
				touchPhaseExtended = TouchPhaseExtended.Ended;
				foreach (KeyValuePair<int, GameObject> moveableObject in moveableObjects)
				{
					focusObj = moveableObject.Value;
				}
				previousSelection = null;
				isAutoSelectDone = true;
				touchPosition = (oldPos = focusObj.transform.position);
			}
			RaycastHit hitInfo;
			switch (touchPhaseExtended)
			{
			case TouchPhaseExtended.Stationary:
				break;
			case TouchPhaseExtended.Canceled:
				break;
			case TouchPhaseExtended.NoEvent:
				break;
			case TouchPhaseExtended.Began:
				isTap = false;
				if (touchPositionOrigin == INVALID_COORDINATE)
				{
					touchPositionOrigin = touchPosition;
				}
				oldPos = touchPosition;
				focusHit = GetPixelHit(touchPosition);
				if (focusHit.collider != null)
				{
					focusObj = focusHit.collider.gameObject;
				}
				else
				{
					focusObj = null;
				}
				if (focusObj != oldFocus)
				{
					if (oldFocus != null)
					{
						BlockAndLockPiece component = oldFocus.GetComponent<BlockAndLockPiece>();
						component.PixelMouseExit();
					}
					if (focusObj != null)
					{
						BlockAndLockPiece component = focusObj.GetComponent<BlockAndLockPiece>();
						component.PixelMouseOver();
					}
					oldFocus = focusObj;
				}
				if (focusObj != null && bkgArtworkColl.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, 500f))
				{
					pickupOffset = focusObj.transform.position - hitInfo.point;
					pickupOffset.z = 0f;
				}
				break;
			case TouchPhaseExtended.Moved:
				if (focusObj != null && bkgArtworkColl.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, 500f))
				{
					BlockAndLockPiece component = focusObj.GetComponent<BlockAndLockPiece>();
					Vector3 newPos = hitInfo.point + pickupOffset;
					newPos.z = 0f;
					if (newPos.x < xMin)
					{
						newPos.x = xMin;
					}
					else if (newPos.x > xMax)
					{
						newPos.x = xMax;
					}
					if (newPos.y < yMin)
					{
						newPos.y = yMin;
					}
					else if (newPos.y > yMax)
					{
						newPos.y = yMax;
					}
					component.PixelMouseDrag(newPos);
				}
				break;
			case TouchPhaseExtended.Ended:
			{
				if (!(focusObj != null))
				{
					break;
				}
				if (Vector3.Distance(oldPos, touchPosition) < swipeThreshold)
				{
					isTap = true;
				}
				else
				{
					isTap = false;
				}
				BlockAndLockPiece component = focusObj.GetComponent<BlockAndLockPiece>();
				if (component != null)
				{
					if (component.Category == PieceCategory.MoveableObject)
					{
						if (isTap)
						{
							Grid2 grid;
							if (focusObj == previousSelection)
							{
								grid = turnArrowOff;
								previousSelection = null;
								playAudioEvent(settings.audioDeselectPiece);
							}
							else
							{
								if (previousSelection != null)
								{
									previousSelection.GetComponent<BlockAndLockPiece>().PixelMouseTapped(touchPosition);
								}
								previousSelection = focusObj;
								grid = PositionToGrid(focusObj.transform.localPosition.x, focusObj.transform.localPosition.y);
								playAudioEvent(settings.audioSelectPiece);
							}
							component.PixelMouseTapped(touchPosition);
							setArrowState(arrowLeft, new Grid2(grid.x - 1, grid.y));
							setArrowState(arrowRight, new Grid2(grid.x + 1, grid.y));
							setArrowState(arrowUp, new Grid2(grid.x, grid.y - 1));
							setArrowState(arrowDown, new Grid2(grid.x, grid.y + 1));
						}
						else
						{
							float num = touchPosition.x - touchPositionOrigin.x;
							float num2 = touchPositionOrigin.y - touchPosition.y;
							component.Select();
							Grid2 grid = PositionToGrid(focusObj.transform.localPosition.x, focusObj.transform.localPosition.y);
							Grid2 grid2 = new Grid2(grid.x, grid.y);
							GameObject gameObject;
							if (Mathf.Abs(num) >= Mathf.Abs(num2))
							{
								gameObject = ((!(num < 0f)) ? arrowRight : arrowLeft);
								grid2.x += (int)Mathf.Sign(num);
							}
							else
							{
								gameObject = ((!(num2 < 0f)) ? arrowDown : arrowUp);
								grid2.y += (int)Mathf.Sign(num2);
							}
							if (isWithinGrid(grid2))
							{
								if (previousSelection != null && previousSelection != focusObj)
								{
									component = previousSelection.GetComponent<BlockAndLockPiece>();
									if (component != null)
									{
										component.Deselect();
									}
								}
								BlockAndLockBoardData blockAndLockBoardData = gameBoard[grid2.x, grid2.y];
								if (blockAndLockBoardData.Category == PieceCategory.Empty || blockAndLockBoardData.Category == PieceCategory.SolvePosition)
								{
									previousSelection = focusObj;
									focusObj = gameObject;
									component = gameObject.GetComponent<BlockAndLockPiece>();
								}
								else
								{
									setArrowState(arrowLeft, new Grid2(grid.x - 1, grid.y));
									setArrowState(arrowRight, new Grid2(grid.x + 1, grid.y));
									setArrowState(arrowUp, new Grid2(grid.x, grid.y - 1));
									setArrowState(arrowDown, new Grid2(grid.x, grid.y + 1));
									previousSelection = focusObj;
								}
							}
						}
					}
					if (component.Category == PieceCategory.Arrow)
					{
						playAudioEvent(settings.audioArrowTapped);
						setArrowState(arrowLeft, turnArrowOff);
						setArrowState(arrowRight, turnArrowOff);
						setArrowState(arrowUp, turnArrowOff);
						setArrowState(arrowDown, turnArrowOff);
						Grid2 grid = PositionToGrid(previousSelection.transform.localPosition.x, previousSelection.transform.localPosition.y);
						int id = gameBoard[grid.x, grid.y].Id;
						Grid2 grid3;
						int num3;
						if (focusObj.name == "ArrowLeft")
						{
							grid3 = new Grid2(-1, 0);
							num3 = grid.x;
						}
						else if (focusObj.name == "ArrowRight")
						{
							grid3 = new Grid2(1, 0);
							num3 = settings.GridWidth - grid.x - 1;
						}
						else if (focusObj.name == "ArrowUp")
						{
							grid3 = new Grid2(0, -1);
							num3 = grid.y;
						}
						else if (focusObj.name == "ArrowDown")
						{
							grid3 = new Grid2(0, 1);
							num3 = settings.GridHeight - grid.y - 1;
						}
						else
						{
							grid3 = new Grid2(0, 0);
							num3 = 0;
						}
						Grid2 grid4 = new Grid2(grid.x, grid.y);
						Grid2 grid5 = new Grid2(grid.x, grid.y);
						bool flag = false;
						int num4 = 0;
						bool hitPiece = false;
						for (int i = 0; i < num3; i++)
						{
							grid4.x += grid3.x;
							grid4.y += grid3.y;
							num4++;
							BlockAndLockBoardData blockAndLockBoardData2 = gameBoard[grid4.x, grid4.y];
							if (blockAndLockBoardData2.Category != 0)
							{
								bool flag2 = true;
								if (blockAndLockBoardData2.Category == PieceCategory.SolvePosition)
								{
									if (blockAndLockBoardData2.Id == id)
									{
										grid5 = new Grid2(grid4.x, grid4.y);
										flag = true;
									}
									else
									{
										flag2 = false;
									}
								}
								else
								{
									if (blockAndLockBoardData2.Category == PieceCategory.MoveableObject)
									{
										hitPiece = true;
									}
									num4--;
								}
								if (flag2)
								{
									break;
								}
							}
							grid5 = new Grid2(grid4.x, grid4.y);
						}
						currentContents = gameBoard[grid5.x, grid5.y];
						float num5 = 0f;
						float num6 = settings.MoveTimePerCell;
						for (int i = 0; i < num4; i++)
						{
							num5 += num6;
							num6 -= settings.MoveDecayPerCell;
						}
						Vector2 vector = GridToPosition(grid5.x, grid5.y);
						Vector3 vector2 = new Vector3(vector.x, vector.y, 0f);
						isMoving = true;
						if (!hasMadeMove)
						{
							hasMadeMove = true;
							restartButton.SetActive(true);
						}
						playAudioEvent(settings.audioMovePiece);
						if (flag)
						{
							gameBoard[grid.x, grid.y] = prevBoardData[id];
							int id2 = gameBoard[grid5.x, grid5.y].Id;
							iTween.MoveTo(previousSelection, iTween.Hash("islocal", true, "position", vector2, "easeType", settings.PieceMoveEaseType, "time", num5, "oncomplete", "onMovementLockComplete", "oncompleteparams", id2, "oncompletetarget", base.gameObject));
							ParticleSystem component2 = previousSelection.GetComponent<ParticleSystem>();
							float num7 = 1f;
							if (component2 != null)
							{
								num7 = component2.main.duration;
							}
							float delayTime = num5 + num7;
							CoroutineRunner.Start(checkIfPuzzleSolved(component, delayTime), this, "checkIfPuzzleSolved");
						}
						else
						{
							gameBoard[grid5.x, grid5.y] = gameBoard[grid.x, grid.y];
							gameBoard[grid.x, grid.y] = prevBoardData[id];
							MoveData moveData = new MoveData(grid5, hitPiece);
							iTween.MoveTo(previousSelection, iTween.Hash("islocal", true, "position", vector2, "easeType", settings.PieceMoveEaseType, "time", num5, "oncomplete", "onMovementNonLockComplete", "oncompleteparams", moveData, "oncompletetarget", base.gameObject));
						}
						prevBoardData[id] = currentContents;
						focusObj = null;
					}
				}
				touchPositionOrigin = INVALID_COORDINATE;
				break;
			}
			case TouchPhaseExtended.Mouse:
				if (oldPos == touchPosition)
				{
					break;
				}
				oldPos = touchPosition;
				focusHit = GetPixelHit(touchPosition);
				if (focusHit.collider != null)
				{
					focusObj = focusHit.collider.gameObject;
				}
				else
				{
					focusObj = null;
				}
				if (!(focusObj != oldFocus))
				{
					break;
				}
				if (oldFocus != null)
				{
					BlockAndLockPiece component = oldFocus.GetComponent<BlockAndLockPiece>();
					if (component != null)
					{
						component.PixelMouseExit();
					}
				}
				if (focusObj != null)
				{
					BlockAndLockPiece component = focusObj.GetComponent<BlockAndLockPiece>();
					if (component != null)
					{
						component.PixelMouseOver();
					}
				}
				oldFocus = focusObj;
				break;
			}
		}

		private TouchPhaseExtended getTouchPhase(out Vector3 touchPosition)
		{
			TouchPhaseExtended touchPhaseExtended = TouchPhaseExtended.NoEvent;
			touchPhaseExtended = ((!InputWrapper.GetMouseButtonDown(0)) ? (InputWrapper.GetMouseButton(0) ? TouchPhaseExtended.Moved : ((!InputWrapper.GetMouseButtonUp(0)) ? TouchPhaseExtended.Mouse : TouchPhaseExtended.Ended)) : TouchPhaseExtended.Began);
			if (UnityEngine.Input.touchSupported && InputWrapper.touchCount > 0)
			{
				touchPhaseExtended = (TouchPhaseExtended)InputWrapper.GetTouch(0).phase;
				touchPosition = InputWrapper.GetTouch(0).position;
			}
			else
			{
				touchPosition = InputWrapper.mousePosition;
			}
			return touchPhaseExtended;
		}

		private IEnumerator checkIfPuzzleSolved(BlockAndLockPiece scriptObj, float delayTime)
		{
			solveCount++;
			if (solveCount < pieceCount)
			{
				yield break;
			}
			scriptObj.PixelMouseExit();
			isSolved = true;
			restartButton.SetActive(false);
			yield return new WaitForSeconds(delayTime);
			completedArtworkObj.SetActive(true);
			float completeParticleDelay = 2f;
			if (settings.completeParticleObj != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(settings.completeParticleObj, completedAnchorObj.transform);
				gameObject.transform.localPosition = Vector2.zero;
				gameObject.transform.localScale = Vector2.one;
				gameObject.layer = LayerMask.NameToLayer("UI");
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.SetStartColor(settings.completeParticleColour);
					component.Play();
					completeParticleDelay = component.main.duration;
				}
			}
			if (string.IsNullOrEmpty(settings.switchTo))
			{
				playAudioEvent(settings.audioPuzzleSolved);
			}
			else
			{
				audioSetSwitchEvent(settings.audioPuzzleSolved, settings.switchTo, settings.switchGameObject);
			}
			yield return new WaitForSeconds(completeParticleDelay);
			dispatcher.DispatchEvent(default(BlockAndLockEvents.BackgroundSolveComplete));
		}

		private RaycastHit GetPixelHit(Vector3 touchPosition)
		{
			RaycastHit hitInfo = default(RaycastHit);
			int layerMask = 1 << LayerMask.NameToLayer("UI");
			if (Physics.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, 1000f, layerMask))
			{
			}
			return hitInfo;
		}

		private bool onCloseButton(BlockAndLockEvents.CloseButton e)
		{
			isClosing = true;
			StartEffectsComplete();
			return false;
		}

		private bool onRestartButton(BlockAndLockEvents.RestartButton e)
		{
			CoroutineRunner.Start(RestartGame(), this, "RestartGame");
			return false;
		}

		private bool onBackgroundSolveComplete(BlockAndLockEvents.BackgroundSolveComplete e)
		{
			isSolved = true;
			restartButton.SetActive(false);
			StartEffectsComplete();
			return false;
		}

		private void StartEffectsComplete()
		{
			playAudioEvent(settings.audioPuzzleSlideOut);
			iTween.MoveTo(base.gameObject, iTween.Hash("position", base.gameObject.transform.position + settings.IntroOffset, "easeType", settings.MinigameSolvedEaseType, "time", settings.MinigameTweenTime, "oncomplete", "onEndingEffectsComplete", "oncompletetarget", base.gameObject));
		}

		private void onEndingEffectsComplete()
		{
			exitBlockAndLock();
		}

		private void onPuzzleInitComplete()
		{
			dispatcher.DispatchEvent(default(BlockAndLockEvents.PuzzleInitialized));
		}

		private void onMovementLockComplete(int solveObjId)
		{
			playAudioEvent(settings.audioLockPiece);
			previousSelection.SetActive(false);
			solveObjects[solveObjId].SetActive(true);
			BlockAndLockPiece component = solveObjects[solveObjId].GetComponent<BlockAndLockPiece>();
			if (component != null)
			{
				component.Lock();
			}
			setArrowState(arrowLeft, turnArrowOff);
			setArrowState(arrowRight, turnArrowOff);
			setArrowState(arrowUp, turnArrowOff);
			setArrowState(arrowDown, turnArrowOff);
			isMoving = false;
		}

		private void onMovementNonLockComplete(MoveData data)
		{
			if (data.hitPiece)
			{
				playAudioEvent(settings.audioStopPiece);
			}
			else
			{
				playAudioEvent(settings.audioStopObstacle);
			}
			setArrowState(arrowLeft, new Grid2(data.stopPos.x - 1, data.stopPos.y));
			setArrowState(arrowRight, new Grid2(data.stopPos.x + 1, data.stopPos.y));
			setArrowState(arrowUp, new Grid2(data.stopPos.x, data.stopPos.y - 1));
			setArrowState(arrowDown, new Grid2(data.stopPos.x, data.stopPos.y + 1));
			isMoving = false;
		}

		private void exitBlockAndLock()
		{
			if (isSolved)
			{
				questService.SendEvent("BlockAndLockSuccess");
			}
			else
			{
				questService.SendEvent("BlockAndLockFailed");
			}
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void playAudioEvent(string audioEventName)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
			}
		}

		private void audioSetSwitchEvent(string eventName, string childComponentName, GameObject go = null)
		{
			if (!string.IsNullOrEmpty(eventName) && !string.IsNullOrEmpty(childComponentName))
			{
				if (go == null)
				{
					EventManager.Instance.PostEvent(eventName, EventAction.SetSwitch, childComponentName);
				}
				else
				{
					EventManager.Instance.PostEvent(eventName, EventAction.SetSwitch, childComponentName, go);
				}
			}
		}

		private Grid2 PositionToGrid(float posX, float posY)
		{
			int x = (int)(Mathf.Floor(posX / settings.GridUnitWidth) + (float)(settings.GridWidth / 2));
			int y = (int)((float)(settings.GridHeight - 1) - (Mathf.Floor(posY / settings.GridUnitHeight) + (float)(settings.GridHeight / 2)));
			return new Grid2(x, y);
		}

		private Vector2 GridToPosition(int gridX, int gridY)
		{
			float x = ((float)gridX - (float)(settings.GridWidth / 2)) * settings.GridUnitWidth + settings.GridUnitWidth / 2f + settings.GridAdjustHorz;
			float y = ((float)(settings.GridHeight - 1) - (float)gridY - (float)settings.GridHeight / 2f) * settings.GridUnitHeight + settings.GridUnitHeight / 2f + settings.GridAdjustVert;
			return new Vector2(x, y);
		}

		private bool isWithinGrid(Grid2 gridPos)
		{
			if (gridPos.x < 0 || gridPos.x > settings.GridWidth - 1 || gridPos.y < 0 || gridPos.y > settings.GridHeight - 1)
			{
				return false;
			}
			return true;
		}

		private void setArrowState(GameObject gameObj, Grid2 testPos)
		{
			if (isWithinGrid(testPos))
			{
				PieceCategory category = gameBoard[testPos.x, testPos.y].Category;
				if (category == PieceCategory.Empty || category == PieceCategory.SolvePosition)
				{
					gameObj.transform.localPosition = GridToPosition(testPos.x, testPos.y);
					gameObj.SetActive(true);
					iTween.ScaleFrom(gameObj, iTween.Hash("scale", new Vector3(0.1f, 0.1f, 0.1f), "easeType", iTween.EaseType.easeOutExpo, "time", 0.1f));
					return;
				}
			}
			gameObj.SetActive(false);
		}

		[Conditional("UNITY_EDITOR")]
		private void Debug_DisplayGameBoard(BlockAndLockBoardData[,] boardData)
		{
			string str = "";
			for (int i = 0; i < settings.GridHeight; i++)
			{
				for (int j = 0; j < settings.GridWidth; j++)
				{
					switch (boardData[j, i].Category)
					{
					case PieceCategory.Empty:
						str += "|__";
						break;
					case PieceCategory.Obstacle:
						str += "[=]";
						break;
					case PieceCategory.MoveableObject:
						str += string.Format("S{0}_", boardData[j, i].Id);
						break;
					case PieceCategory.SolvePosition:
						str += string.Format("E{0}_", boardData[j, i].Id);
						break;
					}
				}
				str += "\n";
			}
		}
	}
}
