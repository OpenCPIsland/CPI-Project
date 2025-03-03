using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class JigsawController : MonoBehaviour
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

		private const string PLAYMAKER_MESG_SUCCESS = "JigsawSuccess";

		private const string PLAYMAKER_MESG_FAILURE = "JigsawFailed";

		[Header("Jigsaw Control")]
		public float SnapDistance = 2f;

		public int snapAssistDuration = 1;

		public Color MouseoverColor = new Color32(byte.MaxValue, 217, 128, byte.MaxValue);

		public Color ShadowColor = new Color32(0, 0, 0, byte.MaxValue);

		public bool IsMeshPuzzle = false;

		[Header("Minigame Control")]
		public iTween.EaseType minigameIntroEaseType = iTween.EaseType.easeOutCubic;

		public iTween.EaseType minigameSolvedEaseType = iTween.EaseType.easeInCubic;

		public float minigameTweenTime = 0.5f;

		public float minigameDelay = 0f;

		public Vector3 introOffset = new Vector3(0f, -120f, 0f);

		[Header("Audio - Puzzle Events")]
		public string audioPuzzleSlideIn = "SFX/UI/Item/WhooshIn";

		public string audioPuzzleSlideOut = "SFX/UI/Item/WhooshOut";

		public string audioPuzzlePieceDrop = "SFX/UI/Item/Drop";

		[Header("Audio - Puzzle Solved - Stinger Support")]
		public string audioPuzzleSolved = "MUS/Quest/Rockhopper/Stinger/ObjCompleteHarp";

		public string switchTo = "";

		public GameObject switchGameObject;

		[Header("Pieces appearance")]
		public iTween.EaseType easeType = iTween.EaseType.easeOutBack;

		public float TweenTimeMin = 0.8f;

		public float TweenTimeMax = 0.8f;

		public float DelayMin = 0.3f;

		public float DelayMax = 0.4f;

		[Header("Pieces solved")]
		public iTween.EaseType pieceEaseType = iTween.EaseType.easeInCubic;

		public float PieceTweenTime = 0.25f;

		public float PieceDelay = 0f;

		public GameObject ParticlesLockPiece;

		public Vector3 PieceParticlePosition = Vector3.zero;

		public Vector3 PieceParticleScale = new Vector3(0.5f, 0.5f, 0.5f);

		[Header("Artwork solved")]
		public iTween.EaseType solveEaseType = iTween.EaseType.easeInCubic;

		public float ArtworkTweenTime = 0.5f;

		public float ArtworkSolveDelay = 0f;

		public GameObject ParticlesSolvePuzzle;

		public Vector3 ArtworkParticlePosition = Vector3.zero;

		public Vector3 ArtworkParticleScale = new Vector3(1f, 1f, 1f);

		public Color SuccessColor = new Color32(byte.MaxValue, 232, 24, byte.MaxValue);

		public Dictionary<string, Vector3> SolvePositions = new Dictionary<string, Vector3>();

		public Dictionary<string, Vector3> StartPositions = new Dictionary<string, Vector3>();

		private EventDispatcher dispatcher;

		private QuestService questService;

		private Transform startContainer;

		private Transform solveContainer;

		private GameObject bkgArtworkObj;

		private Collider bkgArtworkColl;

		private GameObject areaDividerObj;

		private GameObject completedArtworkObj;

		private RectTransform rectTransform;

		private Camera guiCam;

		private Vector3 oldPos;

		private GameObject oldFocus;

		private GameObject focusObj;

		private RaycastHit focusHit;

		private Vector3 pickupOffset;

		private Vector3 shadowLiftOffset;

		private Vector3 shadowOffset = new Vector3(1f, -1f, 0f);

		private float shadowDistanceAdjust = 0f;

		private float shadowScaleAdjustment = 0.05f;

		private float solveEffectsDelay;

		private bool hasInitalizedPuzzle;

		private bool isSolved;

		private int pieceCount;

		private int solveCount;

		private float snapModifier;

		private float snapDecay;

		private List<string> layers = new List<string>();

		private Dictionary<string, GameObject> layerObjects = new Dictionary<string, GameObject>();

		private float xMin;

		private float xMax;

		private float yMin;

		private float yMax;

		private float screenSafetyZone = 0.5f;

		private static readonly int maxRaycasts = 4;

		private RaycastHit[] raycastHits = new RaycastHit[maxRaycasts];

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<JigsawEvents.CloseButton>(onCloseButton);
			dispatcher.AddListener<JigsawEvents.BackgroundSolveComplete>(onBackgroundSolveComplete);
			questService = Service.Get<QuestService>();
		}

		private void Start()
		{
			PuzzleInit();
			playAudioEvent(audioPuzzleSlideIn);
			iTween.MoveFrom(base.gameObject, iTween.Hash("position", base.gameObject.transform.position + introOffset, "easeType", minigameIntroEaseType, "time", minigameTweenTime));
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<JigsawEvents.CloseButton>(onCloseButton);
			dispatcher.RemoveListener<JigsawEvents.BackgroundSolveComplete>(onBackgroundSolveComplete);
		}

		public void PuzzleInit()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
			rectTransform = GetComponent<RectTransform>();
			rectTransform.localPosition = new Vector3(0f, 0f, -100f);
			focusObj = null;
			hasInitalizedPuzzle = false;
			isSolved = false;
			pieceCount = 0;
			solveCount = 0;
			snapModifier = 1f;
			snapDecay = snapModifier / (float)snapAssistDuration;
			shadowLiftOffset = new Vector3(0f - shadowOffset.x, 0f - shadowOffset.y, 0f) * 0.1f;
			shadowDistanceAdjust = Vector2.Distance(Vector2.zero, shadowLiftOffset) * 0.5f;
			SolvePositions.Clear();
			StartPositions.Clear();
			bkgArtworkObj = base.gameObject.transform.Find("Background/Bkg artwork").gameObject;
			if (bkgArtworkObj == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find background artwork"));
				return;
			}
			bkgArtworkColl = bkgArtworkObj.GetComponent<Collider>();
			if (bkgArtworkColl == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find collider on background artwork"));
				return;
			}
			solveContainer = base.gameObject.transform.Find("Solve Positions");
			if (solveContainer == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find solve container"));
			}
			else
			{
				foreach (Transform item in solveContainer)
				{
					GameObject gameObject = item.transform.gameObject;
					if (SolvePositions.ContainsKey(gameObject.name))
					{
						Log.LogError(null, string.Format("O_o\t JigsawController.Start: Found duplicate puzzle piece '{0}'. Please make them unique", gameObject.name));
						return;
					}
					SolvePositions.Add(gameObject.name, gameObject.transform.position);
					Object.Destroy(gameObject);
				}
			}
			startContainer = base.gameObject.transform.Find("Start Positions");
			if (startContainer == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find startContainer object"));
				return;
			}
			foreach (Transform item2 in startContainer)
			{
				GameObject gameObject = item2.transform.gameObject;
				if (StartPositions.ContainsKey(gameObject.name))
				{
					Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- Found duplicate puzzle piece named {0}'. Please make them unique", gameObject.name));
					return;
				}
				StartPositions.Add(gameObject.name, gameObject.transform.position);
				GameObject gameObject2 = null;
				if (!IsMeshPuzzle)
				{
					gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
					JigsawPiece component = gameObject2.GetComponent<JigsawPiece>();
					if (component != null)
					{
						Object.Destroy(component);
					}
				}
				JigsawPiece jigsawPiece = gameObject.AddComponent<JigsawPiece>();
				if (!IsMeshPuzzle)
				{
					gameObject2.transform.SetParent(gameObject.transform, false);
					gameObject2.transform.localScale = Vector3.one;
					gameObject2.transform.localPosition = new Vector3(shadowOffset.x * solveContainer.localScale.x * shadowScaleAdjustment, shadowOffset.y * solveContainer.localScale.y * shadowScaleAdjustment, 0.5f);
					Renderer component2 = gameObject2.GetComponent<Renderer>();
					if (component2.material.HasProperty("_Color"))
					{
						component2.material.color = ShadowColor;
					}
					Collider component3 = gameObject2.GetComponent<Collider>();
					if (component3 != null)
					{
						Object.Destroy(component3);
					}
					gameObject2.name = string.Format("Shadow ({0})", jigsawPiece.Id);
				}
				float num = minigameTweenTime * 0.95f;
				jigsawPiece.Init(MouseoverColor, SuccessColor, TweenTimeMin, TweenTimeMax, DelayMin + num, DelayMax + num, PieceTweenTime, PieceDelay, easeType, pieceEaseType);
				if (gameObject.GetComponent<Collider>() == null)
				{
					gameObject.AddComponent<MeshCollider>();
				}
				if (gameObject.GetComponent<Rigidbody>() == null)
				{
					Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
				gameObject.GetComponent<JigsawPiece>().Appear();
				layers.Add(gameObject.name);
				layerObjects.Add(gameObject.name, gameObject);
			}
			areaDividerObj = base.gameObject.transform.Find("Background/Area Divider").gameObject;
			if (areaDividerObj == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find area divider"));
				return;
			}
			areaDividerObj.SetActive(false);
			completedArtworkObj = base.gameObject.transform.Find("Completed/Completed Artwork").gameObject;
			if (completedArtworkObj == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- could not find completed artwork"));
				return;
			}
			completedArtworkObj.SetActive(true);
			JigsawBackground jigsawBackground = completedArtworkObj.AddComponent<JigsawBackground>();
			jigsawBackground.Init(SuccessColor, ParticlesSolvePuzzle, ArtworkTweenTime, ArtworkSolveDelay, ArtworkParticlePosition, ArtworkParticleScale, solveEaseType);
			completedArtworkObj.SetActive(false);
			if (StartPositions.Count != SolvePositions.Count)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- the number of start positions and solve positions must match ({0} != {1})", StartPositions.Count, SolvePositions.Count));
				return;
			}
			pieceCount = StartPositions.Count;
			guiCam = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_HUD).GetComponentInChildren<Camera>();
			if (guiCam == null)
			{
				Log.LogError(null, string.Format("O_o\t JigsawController.Start: ERROR -- Can't find the GUI camera"));
				return;
			}
			CalculateScreenExtents();
			hasInitalizedPuzzle = true;
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
			if (!hasInitalizedPuzzle || isSolved)
			{
				return;
			}
			Vector3 touchPosition;
			RaycastHit hitInfo;
			Vector3 vector;
			switch (getTouchPhase(out touchPosition))
			{
			case TouchPhaseExtended.Stationary:
			case TouchPhaseExtended.Canceled:
				break;
			case TouchPhaseExtended.NoEvent:
				break;
			case TouchPhaseExtended.Began:
				oldPos = touchPosition;
				if (IsMeshPuzzle)
				{
					focusHit = GetMeshHit(touchPosition);
				}
				else
				{
					focusHit = GetPixelHit(touchPosition);
				}
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
						JigsawPiece component = oldFocus.GetComponent<JigsawPiece>();
						component.PixelMouseExit();
					}
					if (focusObj != null)
					{
						JigsawPiece component = focusObj.GetComponent<JigsawPiece>();
						component.PixelMouseOver();
					}
					oldFocus = focusObj;
				}
				if (focusObj != null && bkgArtworkColl.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, 500f))
				{
					pickupOffset = focusObj.transform.position - hitInfo.point;
					pickupOffset.z = 0f;
					LayerToTop(focusObj.name);
				}
				break;
			case TouchPhaseExtended.Moved:
				if (focusObj != null && bkgArtworkColl.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, 500f))
				{
					JigsawPiece component = focusObj.GetComponent<JigsawPiece>();
					vector = hitInfo.point + pickupOffset + shadowLiftOffset;
					vector.z = 0f;
					if (hitInfo.point.y > areaDividerObj.transform.position.y)
					{
						focusObj.transform.SetParent(solveContainer, false);
					}
					else
					{
						focusObj.transform.SetParent(startContainer, false);
					}
					if (vector.x < xMin)
					{
						vector.x = xMin;
					}
					else if (vector.x > xMax)
					{
						vector.x = xMax;
					}
					if (vector.y < yMin)
					{
						vector.y = yMin;
					}
					else if (vector.y > yMax)
					{
						vector.y = yMax;
					}
					component.PixelMouseDrag(vector);
				}
				break;
			case TouchPhaseExtended.Ended:
			{
				if (!(focusObj != null) || !SolvePositions.ContainsKey(focusObj.name))
				{
					break;
				}
				Vector3 vector2 = SolvePositions[focusObj.name];
				vector2.z = 0f;
				float num = Vector2.Distance(focusObj.transform.position, vector2);
				JigsawPiece component = focusObj.GetComponent<JigsawPiece>();
				if (num <= SnapDistance + shadowDistanceAdjust + snapModifier)
				{
					snapModifier -= snapDecay;
					if (snapModifier < 0f)
					{
						snapModifier = 0f;
					}
					component.Lock(vector2);
					playAudioEvent(audioPuzzlePieceDrop);
					LayerToLocked(focusObj.name);
					if (ParticlesLockPiece != null)
					{
						GameObject gameObject = Object.Instantiate(ParticlesLockPiece, PieceParticlePosition, Quaternion.identity);
						gameObject.transform.SetParent(focusObj.transform, false);
						gameObject.transform.localScale = PieceParticleScale;
						gameObject.layer = LayerMask.NameToLayer("UI");
						if (ParticlesLockPiece != null)
						{
							ParticleSystem component2 = ParticlesLockPiece.GetComponent<ParticleSystem>();
							if (component2 != null)
							{
								solveEffectsDelay = component2.main.duration;
							}
						}
					}
					CoroutineRunner.Start(checkIfPuzzleSolved(component, solveEffectsDelay), this, "checkIfPuzzleSolved");
				}
				else
				{
					vector = focusObj.transform.position;
					component = focusObj.GetComponent<JigsawPiece>();
					component.PixelMouseDrop(vector - shadowLiftOffset);
				}
				break;
			}
			case TouchPhaseExtended.Mouse:
				if (oldPos == touchPosition)
				{
					break;
				}
				oldPos = touchPosition;
				if (IsMeshPuzzle)
				{
					focusHit = GetMeshHit(touchPosition);
				}
				else
				{
					focusHit = GetPixelHit(touchPosition);
				}
				if (focusHit.collider != null)
				{
					if (IsMeshPuzzle)
					{
						focusObj = focusHit.collider.gameObject;
					}
					else
					{
						focusObj = focusHit.collider.gameObject;
					}
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
					JigsawPiece component = oldFocus.GetComponent<JigsawPiece>();
					if (component != null)
					{
						component.PixelMouseExit();
					}
				}
				if (focusObj != null)
				{
					JigsawPiece component = focusObj.GetComponent<JigsawPiece>();
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

		private IEnumerator checkIfPuzzleSolved(JigsawPiece scriptObj, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			solveCount++;
			if (solveCount >= pieceCount)
			{
				scriptObj.PixelMouseExit();
				isSolved = true;
				dispatcher.DispatchEvent(default(JigsawEvents.AllPiecesSolved));
				if (string.IsNullOrEmpty(switchTo))
				{
					playAudioEvent(audioPuzzleSolved);
				}
				else
				{
					audioSetSwitchEvent(audioPuzzleSolved, switchTo, switchGameObject);
				}
			}
		}

		private RaycastHit GetPixelHit(Vector3 touchPosition)
		{
			GameObject gameObject = null;
			RaycastHit result = default(RaycastHit);
			int layerMask = 1 << LayerMask.NameToLayer("UI");
			int num = Physics.RaycastNonAlloc(guiCam.ScreenPointToRay(touchPosition), raycastHits, float.PositiveInfinity, layerMask);
			if (num == 0)
			{
				return result;
			}
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = raycastHits[i];
				Renderer component = raycastHit.transform.GetComponent<Renderer>();
				MeshCollider x = raycastHit.collider as MeshCollider;
				JigsawPiece component2 = raycastHit.collider.gameObject.GetComponent<JigsawPiece>();
				if (!(component2 == null) && !(component == null) && !(component.sharedMaterial == null) && !(component.sharedMaterial.mainTexture == null) && !(x == null))
				{
					Texture2D texture2D = component.material.mainTexture as Texture2D;
					Vector2 textureCoord = raycastHit.textureCoord;
					textureCoord.x *= texture2D.width;
					textureCoord.y *= texture2D.height;
					try
					{
						float a = texture2D.GetPixel((int)textureCoord.x, (int)textureCoord.y).a;
						if (a > 0.5f && (gameObject == null || raycastHit.transform.gameObject.transform.position.z < gameObject.transform.position.z))
						{
							gameObject = raycastHit.transform.gameObject;
							result = raycastHit;
						}
					}
					catch (UnityException)
					{
					}
				}
			}
			return result;
		}

		private RaycastHit GetMeshHit(Vector3 touchPosition)
		{
			int layerMask = 1 << LayerMask.NameToLayer("UI");
			RaycastHit hitInfo;
			if (!Physics.Raycast(guiCam.ScreenPointToRay(touchPosition), out hitInfo, float.PositiveInfinity, layerMask))
			{
				return hitInfo;
			}
			Renderer component = hitInfo.transform.GetComponent<Renderer>();
			MeshCollider x = hitInfo.collider as MeshCollider;
			JigsawPiece component2 = hitInfo.collider.gameObject.GetComponent<JigsawPiece>();
			if (component2 == null || component == null || x == null)
			{
				return default(RaycastHit);
			}
			return hitInfo;
		}

		private void LayerToTop(string layerName)
		{
			int num = layers.IndexOf(layerName);
			if (num > -1)
			{
				layers.RemoveAt(num);
			}
			layers.Add(layerName);
			int count = layers.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = layerObjects[layers[i]];
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = (0f - ((float)i + 1f)) * 1f;
				gameObject.transform.localPosition = localPosition;
			}
		}

		private void LayerToLocked(string layerName)
		{
			int num = layers.IndexOf(layerName);
			if (num > -1)
			{
				GameObject gameObject = layerObjects[layers[num]];
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = 0f;
				gameObject.transform.localPosition = localPosition;
				layers.RemoveAt(num);
			}
		}

		private bool onCloseButton(JigsawEvents.CloseButton e)
		{
			StartEffectsComplete();
			return false;
		}

		private bool onBackgroundSolveComplete(JigsawEvents.BackgroundSolveComplete e)
		{
			isSolved = true;
			StartEffectsComplete();
			return false;
		}

		private void StartEffectsComplete()
		{
			playAudioEvent(audioPuzzleSlideOut);
			iTween.MoveTo(base.gameObject, iTween.Hash("position", base.gameObject.transform.position + introOffset, "easeType", minigameSolvedEaseType, "time", minigameTweenTime, "oncomplete", "onEndingEffectsComplete", "oncompletetarget", base.gameObject));
		}

		private void onEndingEffectsComplete()
		{
			exitJigsaw();
		}

		private void exitJigsaw()
		{
			if (isSolved)
			{
				questService.SendEvent("JigsawSuccess");
			}
			else
			{
				questService.SendEvent("JigsawFailed");
			}
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			Object.Destroy(base.gameObject);
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
	}
}
