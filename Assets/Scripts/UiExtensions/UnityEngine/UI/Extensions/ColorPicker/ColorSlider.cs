namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(Slider))]
	public class ColorSlider : MonoBehaviour
	{
		public ColorPickerControl ColorPicker;

		public ColorValues type;

		private Slider slider;

		private bool listen = true;

		private void Awake()
		{
			slider = GetComponent<Slider>();
			ColorPicker.onValueChanged.AddListener(ColorChanged);
			ColorPicker.onHSVChanged.AddListener(HSVChanged);
			slider.onValueChanged.AddListener(SliderChanged);
		}

		private void OnDestroy()
		{
			ColorPicker.onValueChanged.RemoveListener(ColorChanged);
			ColorPicker.onHSVChanged.RemoveListener(HSVChanged);
			slider.onValueChanged.RemoveListener(SliderChanged);
		}

		private void ColorChanged(Color newColor)
		{
			listen = false;
			switch (type)
			{
			case ColorValues.R:
				slider.normalizedValue = newColor.r;
				break;
			case ColorValues.G:
				slider.normalizedValue = newColor.g;
				break;
			case ColorValues.B:
				slider.normalizedValue = newColor.b;
				break;
			case ColorValues.A:
				slider.normalizedValue = newColor.a;
				break;
			}
		}

		private void HSVChanged(float hue, float saturation, float value)
		{
			listen = false;
			switch (type)
			{
			case ColorValues.Hue:
				slider.normalizedValue = hue;
				break;
			case ColorValues.Saturation:
				slider.normalizedValue = saturation;
				break;
			case ColorValues.Value:
				slider.normalizedValue = value;
				break;
			}
		}

		private void SliderChanged(float newValue)
		{
			if (listen)
			{
				newValue = slider.normalizedValue;
				ColorPicker.AssignColor(type, newValue);
			}
			listen = true;
		}
	}
}
