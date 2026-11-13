// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("A Vertical Slider linked to a Float Variable.")]
	public class GUILayoutVerticalSlider : GUILayoutAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The Float Variable linked to the slider value.")]
        public FsmFloat floatVariable;

		[RequiredField]
        [Tooltip("The value of the variable at the top of the slider.")]
		public FsmFloat topValue;

		[RequiredField]
        [Tooltip("The value of the variable at the bottom of the slider.")]
        public FsmFloat bottomValue;

        [Tooltip("Optional Event to send when the slider value changes.")]
		public FsmEvent changedEvent;
		
		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			topValue = 100;
			bottomValue = 0;
			changedEvent = null;
		}
		
		public override void OnGUI()
		{
			var guiChanged = GUI.changed;
			GUI.changed = false;
			
			if(floatVariable != null)
			{
				floatVariable.Value = GUILayout.VerticalSlider(floatVariable.Value, topValue.Value, bottomValue.Value, LayoutOptions);
			}
			
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