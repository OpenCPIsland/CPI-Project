// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
	public class GUILayoutToggle : GUILayoutAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Link the button state to this Bool Variable.")]
		public FsmBool storeButtonState;

        [Tooltip("Texture to display.")]
		public FsmTexture image;

        [Tooltip("Text to display.")]
		public FsmString text;

        [Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
        public FsmString tooltip;

        [Tooltip("Optional named style in the current GUISkin")]
        public FsmString style;

        [Tooltip("Optional Event to send when the toggle changes.")]
		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			storeButtonState = null;
			text = "";
			image = null;
			tooltip = "";
			style = "Toggle";
			changedEvent = null;
		}
		
		public override void OnGUI()
		{
			var guiChanged = GUI.changed;
			GUI.changed = false;
			
			storeButtonState.Value = GUILayout.Toggle(storeButtonState.Value, new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, LayoutOptions);
			
			if (GUI.changed)
			{
				Fsm.Event(changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = guiChanged;
			}
		}
	}
}