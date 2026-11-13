// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
	public class GUIButton : GUIContentAction
	{
        [Tooltip("The Event to send when the button is pressed.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the button state in a bool variable.")]
		public FsmBool storeButtonState;
	
		public override void Reset()
		{
			base.Reset();
			sendEvent = null;
			storeButtonState = null;
			style = "Button";
		}
		
		public override void OnGUI()
		{
			base.OnGUI();
			
			bool pressed = false;
			
			if (GUI.Button(rect, content, style.Value))
			{
				Fsm.Event(sendEvent);
				pressed = true;
			}
			
			if (storeButtonState != null)
			{
				storeButtonState.Value = pressed;
			}
		}
	}
}