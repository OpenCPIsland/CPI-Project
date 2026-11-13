namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerTester : MonoBehaviour
	{
		public Renderer pickerRenderer;

		public ColorPickerControl picker;

		private void Awake()
		{
			pickerRenderer = GetComponent<Renderer>();
		}

		private void Start()
		{
			picker.onValueChanged.AddListener(delegate(Color color)
			{
				pickerRenderer.material.color = color;
			});
		}
	}
}
