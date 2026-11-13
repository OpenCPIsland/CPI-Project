using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class BlockAndLockPiece : MonoBehaviour
	{
		public Sprite SpriteNormal;

		public Sprite SpriteSelected;

		public Sprite SpriteLocked;

		public GameObject particleObj;

		public Color particleColour = Color.white;

		[HideInInspector]
		public PieceCategory Category;

		private bool isLocked = false;

		private static int poolID;

		private int id;

		private SpriteRenderer rend;

		private Color originalColor;

		private Collider coll;

		private Vector3 originalScale;

		private EventDispatcher dispatcher;

		private BlockAndLockSettings settings;

		private PieceCategory pieceType;

		private bool isSelected = false;

		private ParticleSystem particleSys;

		private EventChannel eventChannel;

		public int Id
		{
			get
			{
				return id;
			}
		}

		private void Awake()
		{
			id = poolID++;
			coll = base.gameObject.GetComponent<Collider>();
			if (coll != null)
			{
				coll.enabled = true;
			}
			originalScale = base.transform.localScale;
			dispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(dispatcher);
		}

		private void Start()
		{
			eventChannel.AddListener<BlockAndLockEvents.AllPiecesSolved>(onAllPiecesSolved);
			eventChannel.AddListener<BlockAndLockEvents.PuzzleInitialized>(onPuzzleInitialized);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Init(PieceCategory category)
		{
			if (SpriteNormal == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockPiece: ERROR -- Normal sprite name must be set on {0}", base.gameObject.name));
				return;
			}
			if (SpriteSelected == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockPiece: ERROR -- Selected sprite name must be set on {0}", base.gameObject.name));
				return;
			}
			if (SpriteLocked == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockPiece: ERROR -- Locked sprite name must be set on {0}", base.gameObject.name));
				return;
			}
			rend = base.gameObject.GetComponent<SpriteRenderer>();
			if (rend == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockPiece: ERROR -- Could not find SpriteRenderer component on {0}", base.gameObject.name));
				return;
			}
			settings = base.gameObject.GetComponentInParent<BlockAndLockSettings>();
			if (settings == null)
			{
				Log.LogError(null, string.Format("O_o\t BlockAndLockPiece: ERROR -- Could not find settings component"));
				return;
			}
			Category = category;
			switch (Category)
			{
			case PieceCategory.Obstacle:
				base.gameObject.SetActive(true);
				break;
			case PieceCategory.SolvePosition:
				base.gameObject.SetActive(true);
				break;
			case PieceCategory.MoveableObject:
				base.gameObject.SetActive(true);
				break;
			case PieceCategory.Arrow:
				base.gameObject.SetActive(false);
				break;
			}
			if (particleObj != null)
			{
				GameObject gameObject = Object.Instantiate(particleObj, base.gameObject.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector2.one;
				gameObject.layer = LayerMask.NameToLayer("UI");
				particleSys = gameObject.GetComponent<ParticleSystem>();
				if (particleSys != null)
				{
					particleSys.SetStartColor(particleColour);
				}
			}
		}

		public void PixelMouseOver()
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
				base.transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 1.2f, originalScale.z);
			}
		}

		public void PixelMouseExit()
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
				base.transform.localScale = originalScale;
			}
		}

		public void PixelMouseDrag(Vector3 newPos)
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
			}
		}

		public void PixelMouseDrop(Vector3 newPos)
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
			}
		}

		public void PixelMouseTapped(Vector3 newPos)
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
				isSelected = !isSelected;
				if (isSelected)
				{
					rend.sprite = SpriteSelected;
				}
				else
				{
					rend.sprite = SpriteNormal;
				}
			}
		}

		public void PixelMouseDragged(Vector3 newPos)
		{
			if (Category == PieceCategory.MoveableObject && !isLocked)
			{
				isSelected = true;
				rend.sprite = SpriteSelected;
			}
		}

		private void DragAndDrop(Vector3 newPos, bool shadowEnabled)
		{
			if (!isLocked)
			{
				newPos.z = base.gameObject.transform.position.z;
				base.gameObject.transform.position = newPos;
			}
		}

		public void Lock()
		{
			if (!isLocked && Category == PieceCategory.SolvePosition)
			{
				rend.sprite = SpriteLocked;
				isLocked = true;
				if (particleSys != null)
				{
					particleSys.SetStartColor(particleColour);
					particleSys.Play();
				}
			}
		}

		public void Reset()
		{
			rend.sprite = SpriteNormal;
			isLocked = false;
			isSelected = false;
		}

		public void Select()
		{
			rend.sprite = SpriteSelected;
			isSelected = true;
		}

		public void Deselect()
		{
			rend.sprite = SpriteNormal;
			isSelected = false;
		}

		private void onAppearEffectComplete()
		{
			if (coll != null)
			{
				coll.enabled = true;
			}
		}

		private bool onAllPiecesSolved(BlockAndLockEvents.AllPiecesSolved e)
		{
			onPieceSolvedEffectComplete();
			return false;
		}

		private bool onPuzzleInitialized(BlockAndLockEvents.PuzzleInitialized e)
		{
			return false;
		}

		private void onPieceSolvedEffectComplete()
		{
			if (id == poolID - 1)
			{
				dispatcher.DispatchEvent(default(BlockAndLockEvents.PieceSolveComplete));
			}
			if (Category == PieceCategory.SolvePosition && settings.HideLockedPiecesOnSolve)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
