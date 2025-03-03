// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetColorRGBA : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The Color Variable to set.")]
        public FsmColor colorVariable;
		
		[HasFloatSlider(0,1)]
        [Tooltip("Set the red channel of the color variable.")]
		public FsmFloat red;
		
		[HasFloatSlider(0,1)]
        [Tooltip("Set the green channel of the color variable.")]
		public FsmFloat green;
		
		[HasFloatSlider(0,1)]
        [Tooltip("Set the blue channel of the color variable.")]
		public FsmFloat blue;
		
		[HasFloatSlider(0,1)]
        [Tooltip("Set the alpha channel of the color variable.")]
		public FsmFloat alpha;
		
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			colorVariable = null;

			// default to variable dropdown with None selected.
			
			red = 0;
			green = 0;
			blue = 0;
			alpha = 1;

			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetColorRGBA();
			
			if (!everyFrame)
				Finish();		
		}

		public override void OnUpdate()
		{
			DoSetColorRGBA();
		}

		void DoSetColorRGBA()
		{
			if (colorVariable == null) return;
			
			var newColor = colorVariable.Value;

			if (!red.IsNone)
			{
				newColor.r = red.Value;
			}

			if (!green.IsNone)
			{
				newColor.g = green.Value;
			}
			
			if (!blue.IsNone)
			{
				newColor.b = blue.Value;
			}
			
			if (!alpha.IsNone)
			{
				newColor.a = alpha.Value;
			}
			
			colorVariable.Value = newColor;
		}
	}
}