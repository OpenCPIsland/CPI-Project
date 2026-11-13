using UnityEngine;
using UnityEngine.UI;

namespace DevonLocalization
{
	[RequireComponent(typeof(Text))]
	public class LocalizedText : AbstractLocalizedText
	{
		private Text textField;

		public override string TextFieldText
		{
			get
			{
				return GetComponent<Text>().text;
			}
		}

		protected override void awake()
		{
			textField = GetComponent<Text>();
		}

		protected override void setText(string text)
		{
			textField.text = text;
		}
	}
}
