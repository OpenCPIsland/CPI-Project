namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("UI/Effects/Extensions/Shining Effect")]
	public class ShineEffector : MonoBehaviour
	{
		public ShineEffect effector;

		[SerializeField]
		[HideInInspector]
		private GameObject effectRoot;

		[Range(-1f, 1f)]
		public float yOffset = -1f;

		[Range(0.1f, 1f)]
		public float width = 0.5f;

		private RectTransform effectorRect;

		public float YOffset
		{
			get
			{
				return yOffset;
			}
			set
			{
				ChangeVal(value);
				yOffset = value;
			}
		}

		private void OnEnable()
		{
			if (effector == null)
			{
				GameObject gameObject = new GameObject("effector");
				effectRoot = new GameObject("ShineEffect");
				effectRoot.transform.SetParent(base.transform);
				effectRoot.AddComponent<Image>().sprite = base.gameObject.GetComponent<Image>().sprite;
				effectRoot.GetComponent<Image>().type = base.gameObject.GetComponent<Image>().type;
				effectRoot.AddComponent<Mask>().showMaskGraphic = false;
				effectRoot.transform.localScale = Vector3.one;
				effectRoot.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
				effectRoot.GetComponent<RectTransform>().anchorMax = Vector2.one;
				effectRoot.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				effectRoot.GetComponent<RectTransform>().offsetMax = Vector2.zero;
				effectRoot.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				effectRoot.transform.SetAsFirstSibling();
				gameObject.AddComponent<RectTransform>();
				gameObject.transform.SetParent(effectRoot.transform);
				effectorRect = gameObject.GetComponent<RectTransform>();
				effectorRect.localScale = Vector3.one;
				effectorRect.anchoredPosition3D = Vector3.zero;
				effectorRect.gameObject.AddComponent<ShineEffect>();
				effectorRect.anchorMax = Vector2.one;
				effectorRect.anchorMin = Vector2.zero;
				effectorRect.Rotate(0f, 0f, -8f);
				effector = gameObject.GetComponent<ShineEffect>();
				effectorRect.offsetMax = Vector2.zero;
				effectorRect.offsetMin = Vector2.zero;
				OnValidate();
			}
		}

		private void OnValidate()
		{
			effector.Yoffset = yOffset;
			effector.Width = width;
			if (yOffset <= -1f || yOffset >= 1f)
			{
				effectRoot.SetActive(false);
			}
			else if (!effectRoot.activeSelf)
			{
				effectRoot.SetActive(true);
			}
		}

		private void ChangeVal(float value)
		{
			effector.Yoffset = value;
			if (value <= -1f || value >= 1f)
			{
				effectRoot.SetActive(false);
			}
			else if (!effectRoot.activeSelf)
			{
				effectRoot.SetActive(true);
			}
		}

		private void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(effectRoot);
			}
			else
			{
				Object.Destroy(effectRoot);
			}
		}
	}
}
