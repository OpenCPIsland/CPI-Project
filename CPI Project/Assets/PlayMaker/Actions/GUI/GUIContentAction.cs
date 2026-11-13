// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// base type for GUI actions with GUIContent parameters
	
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIContentAction : GUIAction
	{
        [Tooltip("Optional image to display.")]
		public FsmTexture image;

        [Tooltip("Optional text to display.")]
		public FsmString text;

        [Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
		public FsmString tooltip;

        [Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;
		
		internal GUIContent content;
		
		public override void Reset()
		{
			base.Reset();
			image = null;
			text = "";
			tooltip = "";
			style = "";
		}
		
		public override void OnGUI()
		{
			base.OnGUI();
			
			content = new GUIContent(text.Value, image.Value, tooltip.Value);
		}
	}
}
