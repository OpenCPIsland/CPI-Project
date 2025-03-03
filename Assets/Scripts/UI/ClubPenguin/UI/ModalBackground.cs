using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class ModalBackground : MonoBehaviour
	{
		private const string NAME = "ModalBackground";

		private const int TILE_MIN_DIMENSION = 20;

		public Color ModalColor = new Color32(9, 4, 78, 153);

		[Tooltip("Use a tiled sprite as the background of this modal. Do NOT use a small sprite for this field. It will create a fullscreen tile and create too many vertices.")]
		public Sprite TileSprite;

		[Tooltip("Optional material can be supplied")]
		public Material TileMaterial;

		private GameObject modalBackground;

		private bool isInitialized;

		private static Stack<GameObject> modalBackgrounds = new Stack<GameObject>();

		private void OnValidate()
		{
			if (TileSprite != null && (TileSprite.rect.width < 20f || TileSprite.rect.height < 20f))
			{
				TileSprite = null;
				Log.LogErrorFormatted(this, "TileSprite dimensions were below {0}, tile sprite will not be used", 20);
			}
		}

		private void Awake()
		{
			modalBackground = new GameObject("ModalBackground", typeof(RectTransform));
			Service.Get<EventDispatcher>().DispatchEvent(new UIEvents.ModalBackgroundShown(modalBackground));
		}

		private void Start()
		{
			modalBackground.transform.SetParent(base.transform.parent, false);
			Image image = modalBackground.AddComponent<Image>();
			image.type = Image.Type.Tiled;
			if (TileSprite != null)
			{
				image.sprite = TileSprite;
			}
			else
			{
				image.color = ModalColor;
			}
			if (TileMaterial != null)
			{
				image.material = TileMaterial;
			}
			CanvasGroup canvasGroup = modalBackground.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = true;
			RectTransform component = modalBackground.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.offsetMin = Vector2.zero;
			component.offsetMax = Vector2.zero;
			modalBackground.transform.SetSiblingIndex(base.gameObject.transform.GetSiblingIndex());
			pushModal();
			isInitialized = true;
		}

		private void pushModal()
		{
			if (modalBackgrounds.Count > 0)
			{
				modalBackgrounds.Peek().SetActive(false);
			}
			modalBackground.SetActive(true);
			modalBackgrounds.Push(modalBackground);
		}

		private void popModal()
		{
			modalBackground.SetActive(false);
			modalBackgrounds.Pop();
			if (modalBackgrounds.Count > 0)
			{
				modalBackgrounds.Peek().SetActive(true);
			}
		}

		private void OnEnable()
		{
			if (isInitialized)
			{
				pushModal();
			}
		}

		private void OnDisable()
		{
			if (isInitialized)
			{
				popModal();
			}
		}

		private void OnDestroy()
		{
			Object.Destroy(modalBackground);
		}
	}
}
