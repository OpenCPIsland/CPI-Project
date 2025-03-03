using UnityEngine;

namespace DevonLocalization
{
	[RequireComponent(typeof(TextMesh))]
	public class LocalizedTextMesh : AbstractLocalizedText
	{
		private TextMesh textMesh;

		public override string TextFieldText
		{
			get
			{
				return GetComponent<TextMesh>().text;
			}
		}

		protected override void setText(string text)
		{
			textMesh.text = text;
		}

		protected override void awake()
		{
			textMesh = GetComponent<TextMesh>();
		}
	}
}
