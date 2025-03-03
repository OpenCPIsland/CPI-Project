using UnityEngine;

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

	public Texture OutlineTexture
	{
		set
		{
			if (outlinerMaterial != null)
			{
				outlinerMaterial.SetTexture("_OutlineTex", value);
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

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		float value = OutlineLookupDistance + animDelta;
		outlinerMaterial.SetFloat(PROP_LOOKUP_DISTANCE_ID, value);
		Graphics.Blit(source, destination, outlinerMaterial);
	}
}
