using TMPro;
using UnityEngine;

namespace DevonLocalization
{
	[RequireComponent(typeof(TextMeshPro))]
	public class LocalizedTextMeshPro : AbstractLocalizedText
	{
		private TextMeshPro textField;

		public override string TextFieldText
		{
			get
			{
				return GetComponent<TextMeshPro>().text;
			}
		}

		protected override void awake()
		{
			textField = GetComponent<TextMeshPro>();
		}

		protected override void setText(string text)
		{
			textField.text = text;
		}
	}
}
