namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(Text))]
	public class ColorLabel : MonoBehaviour
	{
		public ColorPickerControl picker;

		public ColorValues type;

		public string prefix = "R: ";

		public float minValue = 0f;

		public float maxValue = 255f;

		public int precision = 0;

		private Text label;

		private void Awake()
		{
			label = GetComponent<Text>();
		}

		private void OnEnable()
		{
			if (Application.isPlaying && picker != null)
			{
				picker.onValueChanged.AddListener(ColorChanged);
				picker.onHSVChanged.AddListener(HSVChanged);
			}
		}

		private void OnDestroy()
		{
			if (picker != null)
			{
				picker.onValueChanged.RemoveListener(ColorChanged);
				picker.onHSVChanged.RemoveListener(HSVChanged);
			}
		}

		private void ColorChanged(Color color)
		{
			UpdateValue();
		}

		private void HSVChanged(float hue, float sateration, float value)
		{
			UpdateValue();
		}

		private void UpdateValue()
		{
			if (picker == null)
			{
				label.text = prefix + "-";
				return;
			}
			float value = minValue + picker.GetValue(type) * (maxValue - minValue);
			label.text = prefix + ConvertToDisplayString(value);
		}

		private string ConvertToDisplayString(float value)
		{
			if (precision > 0)
			{
				return value.ToString("f " + precision);
			}
			return Mathf.FloorToInt(value).ToString();
		}
	}
}
