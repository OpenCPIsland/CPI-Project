// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Select a Random String from an array of Strings.")]
	public class SelectRandomString : FsmStateAction
	{
		[CompoundArray("Strings", "String", "Weight")]
        [Tooltip("A possible String choice.")]
        public FsmString[] strings;
		[HasFloatSlider(0, 1)]
        [Tooltip("The relative probability of this string being picked. " +
                 "E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
        public FsmFloat[] weights;
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the chosen String in a String Variable.")]
        public FsmString storeString;
		
		public override void Reset()
		{
			strings = new FsmString[3];
			weights = new FsmFloat[] {1,1,1};
			storeString = null;
		}

		public override void OnEnter()
		{
			DoSelectRandomString();
			Finish();
		}
		
		void DoSelectRandomString()
		{
			if (strings == null) return;
			if (strings.Length == 0) return;
			if (storeString == null) return;

			int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			
			if (randomIndex != -1)
			{
				storeString.Value = strings[randomIndex].Value;
			}
		}
	}
}