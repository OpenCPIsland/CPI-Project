using TMPro;
using UnityEngine;

namespace DevonLocalization
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizedTextMeshProUGUI : AbstractLocalizedText
	{
		private TextMeshProUGUI textField;

		public override string TextFieldText
		{
			get
			{
				return GetComponent<TextMeshProUGUI>().text;
			}
		}

		protected override void awake()
		{
			textField = GetComponent<TextMeshProUGUI>();
		}

		protected override void setText(string text)
		{
			textField.text = text;
		}
	}
}
