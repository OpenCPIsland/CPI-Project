namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(BoxSlider), typeof(RawImage))]
	[ExecuteInEditMode]
	public class SVBoxSlider : MonoBehaviour
	{
		public ColorPickerControl picker;

		private BoxSlider slider;

		private RawImage image;

		private float lastH = -1f;

		private bool listen = true;

		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		private void Awake()
		{
			slider = GetComponent<BoxSlider>();
			image = GetComponent<RawImage>();
			RegenerateSVTexture();
		}

		private void OnEnable()
		{
			if (Application.isPlaying && picker != null)
			{
				slider.OnValueChanged.AddListener(SliderChanged);
				picker.onHSVChanged.AddListener(HSVChanged);
			}
		}

		private void OnDisable()
		{
			if (picker != null)
			{
				slider.OnValueChanged.RemoveListener(SliderChanged);
				picker.onHSVChanged.RemoveListener(HSVChanged);
			}
		}

		private void OnDestroy()
		{
			if (image.texture != null)
			{
				Object.DestroyImmediate(image.texture);
			}
		}

		private void SliderChanged(float saturation, float value)
		{
			if (listen)
			{
				picker.AssignColor(ColorValues.Saturation, saturation);
				picker.AssignColor(ColorValues.Value, value);
			}
			listen = true;
		}

		private void HSVChanged(float h, float s, float v)
		{
			if (lastH != h)
			{
				lastH = h;
				RegenerateSVTexture();
			}
			if (s != slider.NormalizedValueX)
			{
				listen = false;
				slider.NormalizedValueX = s;
			}
			if (v != slider.NormalizedValueY)
			{
				listen = false;
				slider.NormalizedValueY = v;
			}
		}

		private void RegenerateSVTexture()
		{
			double h = (!(picker != null)) ? 0f : (picker.H * 360f);
			if (image.texture != null)
			{
				Object.DestroyImmediate(image.texture);
			}
			Texture2D texture2D = new Texture2D(100, 100);
			texture2D.hideFlags = HideFlags.DontSave;
			Texture2D texture2D2 = texture2D;
			for (int i = 0; i < 100; i++)
			{
				Color32[] array = new Color32[100];
				for (int j = 0; j < 100; j++)
				{
					array[j] = HSVUtil.ConvertHsvToRgb(h, (float)i / 100f, (float)j / 100f, 1f);
				}
				texture2D2.SetPixels32(i, 0, 1, 100, array);
			}
			texture2D2.Apply();
			image.texture = texture2D2;
		}
	}
}
