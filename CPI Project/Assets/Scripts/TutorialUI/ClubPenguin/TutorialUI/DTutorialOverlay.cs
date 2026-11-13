using System;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[Serializable]
	public class DTutorialOverlay
	{
		public GameObject Target;

		public Vector2 Position;

		public bool AutoSize;

		public Vector2 Size;

		public TutorialOverlayShape Shape;

		public TutorialOverlayArrowPosition ArrowPosition;

		public Vector2 ArrowOffset;

		public Vector2 TextBoxOffset;

		public Vector2 TextBoxPivot;

		public float MaxTextBoxSize;

		public bool ShowHighlightOutline;

		public string Text;

		public bool ShowArrow;

		public float Opacity;

		public bool DisableUI;

		public bool EnableTarget;

		public bool BlocksRaycast;

		public DTutorialOverlay()
		{
			Target = null;
			Position = Vector2.zero;
			AutoSize = false;
			Size = new Vector2(100f, 100f);
			Shape = TutorialOverlayShape.CIRCLE;
			ArrowPosition = TutorialOverlayArrowPosition.LEFT;
			ArrowOffset = Vector2.zero;
			TextBoxOffset = Vector2.zero;
			TextBoxPivot = new Vector2(0.5f, 0.5f);
			MaxTextBoxSize = 300f;
			ShowHighlightOutline = true;
			Text = "";
			ShowArrow = true;
			Opacity = 0.6f;
			DisableUI = true;
			EnableTarget = true;
			BlocksRaycast = false;
		}
	}
}
