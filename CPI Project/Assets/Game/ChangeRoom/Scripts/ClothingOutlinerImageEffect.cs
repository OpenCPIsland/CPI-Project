using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
internal class ClothingOutlinerImageEffect : MonoBehaviour
{
	public static int PROP_LOOKUPS_ID;

	public static int PROP_LOOKUP_DISTANCE_ID;

	public static int PROP_OUTLINE_COLOR_ID;

	public static Shader OUTLINE_SHADER;

	[Range(1f, 8f)]
	[Header("Outline")]
	public int OutlineLookups = 3;

	public float OutlineLookupDistance = 0.0015f;

	public Color OutlineColor = Color.white;

	[Header("Animation")]
	public float AnimationDuration = 0.666f;

	public float AnimationStrength = 0.0005f;

	public AnimationCurve AnimationCurve;

	private float animT;

	private float animDelta;

	private Material outlinerMaterial;

	private GameObject overlayCanvasObject;

	private RawImage overlayImage;

	public Texture OutlineTexture
	{
		set
		{
			if (outlinerMaterial != null)
			{
				outlinerMaterial.SetTexture("_OutlineTex", value);
				overlayImage.texture = value;
			}
		}
	}

	private void Awake()
	{
		if (OUTLINE_SHADER == null)
		{
			OUTLINE_SHADER = Shader.Find("Hidden/ClothingOutlinerImageEffect");
			PROP_LOOKUPS_ID = Shader.PropertyToID("_OutlineLookups");
			PROP_LOOKUP_DISTANCE_ID = Shader.PropertyToID("_OutlineLookupDistance");
			PROP_OUTLINE_COLOR_ID = Shader.PropertyToID("_OutlineColor");
		}
		outlinerMaterial = new Material(OUTLINE_SHADER);
		outlinerMaterial.SetInt(PROP_LOOKUPS_ID, OutlineLookups);
		outlinerMaterial.SetColor(PROP_OUTLINE_COLOR_ID, OutlineColor);
		createOverlayCanvas();
	}

	public void Update()
	{
		if (AnimationDuration > 0f)
		{
			animT += Time.deltaTime;
			while (animT >= AnimationDuration)
			{
				animT -= AnimationDuration;
			}
			float num = Mathf.Clamp01(animT / AnimationDuration);
			if (AnimationCurve != null && AnimationCurve.length > 0)
			{
				num = AnimationCurve.Evaluate(num);
			}
			animDelta = num * AnimationStrength;
		}
	}

	private void createOverlayCanvas()
	{
		Camera targetCamera = GetComponent<Camera>();
		overlayCanvasObject = new GameObject("ClothingOutlinerOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
		overlayCanvasObject.layer = LayerMask.NameToLayer("UI");
		Canvas canvas = overlayCanvasObject.GetComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.worldCamera = targetCamera;
		canvas.planeDistance = Mathf.Max(targetCamera.nearClipPlane + 0.1f, 1f);
		canvas.overrideSorting = true;
		canvas.sortingOrder = 32767;
		overlayCanvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

		RectTransform canvasRect = (RectTransform)overlayCanvasObject.transform;
		canvasRect.anchorMin = Vector2.zero;
		canvasRect.anchorMax = Vector2.one;
		canvasRect.offsetMin = Vector2.zero;
		canvasRect.offsetMax = Vector2.zero;

		GameObject imageObject = new GameObject("Outline", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
		imageObject.layer = LayerMask.NameToLayer("UI");
		imageObject.transform.SetParent(canvasRect, false);
		RectTransform imageRect = (RectTransform)imageObject.transform;
		imageRect.anchorMin = Vector2.zero;
		imageRect.anchorMax = Vector2.one;
		imageRect.offsetMin = Vector2.zero;
		imageRect.offsetMax = Vector2.zero;

		overlayImage = imageObject.GetComponent<RawImage>();
		overlayImage.texture = Texture2D.whiteTexture;
		overlayImage.material = outlinerMaterial;
		overlayImage.raycastTarget = false;
		overlayImage.maskable = false;
	}

	private void OnEnable()
	{
		if (overlayCanvasObject != null)
		{
			overlayCanvasObject.SetActive(true);
		}
	}

	private void OnDisable()
	{
		if (overlayCanvasObject != null)
		{
			overlayCanvasObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (overlayCanvasObject != null)
		{
			Destroy(overlayCanvasObject);
		}
		if (outlinerMaterial != null)
		{
			Destroy(outlinerMaterial);
		}
	}
}
