namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerPresets : MonoBehaviour
	{
		public ColorPickerControl picker;

		public GameObject[] presets;

		public Image createPresetImage;

		private void Awake()
		{
			picker.onValueChanged.AddListener(ColorChanged);
		}

		public void CreatePresetButton()
		{
			int num = 0;
			while (true)
			{
				if (num < presets.Length)
				{
					if (!presets[num].activeSelf)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			presets[num].SetActive(true);
			presets[num].GetComponent<Image>().color = picker.CurrentColor;
		}

		public void PresetSelect(Image sender)
		{
			picker.CurrentColor = sender.color;
		}

		private void ColorChanged(Color color)
		{
			createPresetImage.color = color;
		}
	}
}
