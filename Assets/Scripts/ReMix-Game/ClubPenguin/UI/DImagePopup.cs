using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	public class DImagePopup
	{
		public SpriteContentKey ImageContentKey;

		public Vector2 ImageOffset;

		public Vector2 ImageScale;

		public string Text;

		public DTextStyle TextStyle;

		public TextAnchor TextAlignment;

		public Vector2 TextOffset;

		public DImagePopup()
		{
			ImageContentKey = null;
			ImageOffset = default(Vector2);
			ImageScale = Vector2.one;
			Text = "";
			TextStyle = new DTextStyle();
			TextAlignment = TextAnchor.MiddleCenter;
			TextOffset = default(Vector2);
		}
	}
}
