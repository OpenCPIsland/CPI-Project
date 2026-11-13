// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Select a random Color from an array of Colors.")]
	public class SelectRandomColor : FsmStateAction
	{
		[CompoundArray("Colors", "Color", "Weight")]
        [Tooltip("A possible Color choice.")]
        public FsmColor[] colors;
		[HasFloatSlider(0, 1)]
        [Tooltip("The relative probability of this color being picked. " +
                 "E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
        public FsmFloat[] weights;
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the selected Color in a Color Variable.")]
		public FsmColor storeColor;

		public override void Reset()
		{
			colors = new FsmColor[3];
			weights = new FsmFloat[] {1,1,1};
			storeColor = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomColor();
			Finish();
		}
		
		void DoSelectRandomColor()
		{
			if (colors == null) return;
			if (colors.Length == 0) return;
			if (storeColor == null) return;
			
			int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			
			if (randomIndex != -1)
			{
				storeColor.Value = colors[randomIndex].Value;	
			}
		}
	}
}