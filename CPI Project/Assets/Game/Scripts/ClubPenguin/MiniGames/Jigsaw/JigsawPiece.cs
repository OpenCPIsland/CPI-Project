using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class JigsawPiece : MonoBehaviour
	{
		private Color mouseOverColor = new Color32(byte.MaxValue, 217, 128, byte.MaxValue);

		private Color successColor = new Color32(byte.MaxValue, 232, 24, byte.MaxValue);

		private float tweenTimeMin = 0.8f;

		private float tweenTimeMax = 0.8f;

		private float delayMin = 0.3f;

		private float delayMax = 0.4f;

		private float pieceTweenTime = 0.25f;

		private float pieceDelay = 0f;

		private iTween.EaseType easeType = iTween.EaseType.easeOutBack;

		private iTween.EaseType pieceEaseType = iTween.EaseType.easeInCubic;

		private GameObject completedObj;

		private GameObject shadowObj;

		private bool isLocked = false;

		private static int poolID;

		private int id;

		private Renderer rend;

		private Color originalColor;

		private Collider coll;

		private Vector3 originalScale;

		private EventDispatcher dispatcher;

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
			coll = GetComponent<Collider>();
			coll.enabled = false;
			rend = GetComponent<Renderer>();
			if (rend.material.HasProperty("_Color"))
			{
				originalColor = rend.material.color;
			}
			originalScale = base.transform.localScale;
			base.transform.localScale = Vector3.zero;
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<JigsawEvents.AllPiecesSolved>(onAllPiecesSolved);
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<JigsawEvents.AllPiecesSolved>(onAllPiecesSolved);
		}

		public void Init(Color _mouseOverColor, Color _successColor, float _tweenTimeMin, float _tweenTimeMax, float _delayMin, float _delayMax, float _pieceTweenTime, float _pieceDelay, iTween.EaseType _easeType, iTween.EaseType _pieceEaseType)
		{
			mouseOverColor = _mouseOverColor;
			successColor = _successColor;
			tweenTimeMin = _tweenTimeMin;
			tweenTimeMax = _tweenTimeMax;
			delayMin = _delayMin;
			delayMax = _delayMax;
			pieceTweenTime = _pieceTweenTime;
			pieceDelay = _pieceDelay;
			easeType = _easeType;
			pieceEaseType = _pieceEaseType;
			Transform transform = base.gameObject.transform.Find(string.Format("Shadow ({0})", id));
			if (transform != null)
			{
				shadowObj = transform.gameObject;
				if (shadowObj != null)
				{
					shadowObj.SetActive(false);
				}
			}
		}

		public void PixelMouseOver()
		{
			if (!isLocked && rend.material.HasProperty("_Color"))
			{
				rend.material.color = mouseOverColor;
			}
		}

		public void PixelMouseExit()
		{
			if (!isLocked)
			{
				if (rend.material.HasProperty("_Color"))
				{
					rend.material.color = originalColor;
				}
				if (shadowObj != null)
				{
					shadowObj.SetActive(false);
				}
			}
		}

		public void PixelMouseDrag(Vector3 newPos)
		{
			DragAndDrop(newPos, true);
		}

		public void PixelMouseDrop(Vector3 newPos)
		{
			DragAndDrop(newPos, false);
		}

		private void DragAndDrop(Vector3 newPos, bool shadowEnabled)
		{
			if (!isLocked)
			{
				newPos.z = base.gameObject.transform.position.z;
				base.gameObject.transform.position = newPos;
				if (shadowObj != null)
				{
					shadowObj.SetActive(shadowEnabled);
				}
			}
		}

		public void Lock(Vector3 newPos)
		{
			if (!isLocked)
			{
				PixelMouseDrag(newPos);
				base.gameObject.GetComponent<Collider>().enabled = false;
				if (rend.material.HasProperty("_Color"))
				{
					rend.material.color = originalColor;
				}
				if (shadowObj != null)
				{
					Object.Destroy(shadowObj);
				}
				JigsawForeground component = GetComponent<JigsawForeground>();
				if (component != null)
				{
					component.OnPieceLocked();
				}
				isLocked = true;
			}
		}

		public void Appear()
		{
			float num = Random.Range(tweenTimeMin, tweenTimeMax);
			float num2 = Random.Range(delayMin, delayMax);
			iTween.ScaleTo(base.gameObject, iTween.Hash("scale", originalScale, "easeType", easeType, "time", num, "delay", num2, "oncomplete", "onAppearEffectComplete", "oncompletetarget", base.gameObject));
		}

		private void onAppearEffectComplete()
		{
			coll.enabled = true;
		}

		private bool onAllPiecesSolved(JigsawEvents.AllPiecesSolved e)
		{
			if (rend.material.HasProperty("_Color"))
			{
				iTween.ColorTo(base.gameObject, iTween.Hash("color", successColor, "easeType", pieceEaseType, "time", pieceTweenTime, "delay", pieceDelay, "oncomplete", "onPieceSolvedEffectComplete", "oncompletetarget", base.gameObject));
			}
			else
			{
				onPieceSolvedEffectComplete();
			}
			return false;
		}

		private void onPieceSolvedEffectComplete()
		{
			if (id == poolID - 1)
			{
				dispatcher.DispatchEvent(default(JigsawEvents.PieceSolveComplete));
			}
			base.gameObject.SetActive(false);
		}
	}
}
