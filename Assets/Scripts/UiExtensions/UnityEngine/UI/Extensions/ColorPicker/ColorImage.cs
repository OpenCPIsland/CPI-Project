namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(Image))]
	public class ColorImage : MonoBehaviour
	{
		public ColorPickerControl picker;

		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
			picker.onValueChanged.AddListener(ColorChanged);
		}

		private void OnDestroy()
		{
			picker.onValueChanged.RemoveListener(ColorChanged);
		}

		private void ColorChanged(Color newColor)
		{
			image.color = newColor;
		}
	}
}
