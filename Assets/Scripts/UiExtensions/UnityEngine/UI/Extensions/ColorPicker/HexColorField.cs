using System.Globalization;
using System.Text.RegularExpressions;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[RequireComponent(typeof(InputField))]
	public class HexColorField : MonoBehaviour
	{
		public ColorPickerControl ColorPicker;

		public bool displayAlpha;

		private InputField hexInputField;

		private const string hexRegex = "^#?(?:[0-9a-fA-F]{3,4}){1,2}$";

		private void Awake()
		{
			hexInputField = GetComponent<InputField>();
			hexInputField.onEndEdit.AddListener(UpdateColor);
			ColorPicker.onValueChanged.AddListener(UpdateHex);
		}

		private void OnDestroy()
		{
			hexInputField.onValueChanged.RemoveListener(UpdateColor);
			ColorPicker.onValueChanged.RemoveListener(UpdateHex);
		}

		private void UpdateHex(Color newColor)
		{
			hexInputField.text = ColorToHex(newColor);
		}

		private void UpdateColor(string newHex)
		{
			Color32 color;
			if (HexToColor(newHex, out color))
			{
				ColorPicker.CurrentColor = color;
			}
			else
			{
				Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
			}
		}

		private string ColorToHex(Color32 color)
		{
			if (displayAlpha)
			{
				return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
			}
			return string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
		}

		public static bool HexToColor(string hex, out Color32 color)
		{
			if (Regex.IsMatch(hex, "^#?(?:[0-9a-fA-F]{3,4}){1,2}$"))
			{
				int num = hex.StartsWith("#") ? 1 : 0;
				if (hex.Length == num + 8)
				{
					color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 6, 2), NumberStyles.AllowHexSpecifier));
				}
				else if (hex.Length == num + 6)
				{
					color = new Color32(byte.Parse(hex.Substring(num, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(hex.Substring(num + 4, 2), NumberStyles.AllowHexSpecifier), byte.MaxValue);
				}
				else if (hex.Length != num + 4)
				{
					color = new Color32(byte.Parse("" + hex[num] + hex[num], NumberStyles.AllowHexSpecifier), byte.Parse("" + hex[num + 1] + hex[num + 1], NumberStyles.AllowHexSpecifier), byte.Parse("" + hex[num + 2] + hex[num + 2], NumberStyles.AllowHexSpecifier), byte.MaxValue);
				}
				else
				{
					color = new Color32(byte.Parse("" + hex[num] + hex[num], NumberStyles.AllowHexSpecifier), byte.Parse("" + hex[num + 1] + hex[num + 1], NumberStyles.AllowHexSpecifier), byte.Parse("" + hex[num + 2] + hex[num + 2], NumberStyles.AllowHexSpecifier), byte.Parse("" + hex[num + 3] + hex[num + 3], NumberStyles.AllowHexSpecifier));
				}
				return true;
			}
			color = default(Color32);
			return false;
		}
	}
}
