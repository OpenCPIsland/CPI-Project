// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObject : FsmStateAction
	{
		[CompoundArray("Game Objects", "Game Object", "Weight")]
        [Tooltip("A possible GameObject choice.")]
        public FsmGameObject[] gameObjects;
		[HasFloatSlider(0, 1)]
        [Tooltip("The relative probability of this GameObject being picked. " +
                 "E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
        public FsmFloat[] weights;
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the selected GameObject in a GameObject Variable.")]
        public FsmGameObject storeGameObject;
		
		public override void Reset ()
		{
			gameObjects = new FsmGameObject[3];
			weights = new FsmFloat[] {1,1,1};
			storeGameObject = null;
		}
		
		public override void OnEnter ()
		{
			DoSelectRandomGameObject();
			Finish();
		}
		
		void DoSelectRandomGameObject()
		{
			if (gameObjects == null) return;
			if (gameObjects.Length == 0) return;
			if (storeGameObject == null) return;

			int randomIndex = ActionHelpers.GetRandomWeightedIndex(weights);
			
			if (randomIndex != -1)
			{
				storeGameObject.Value = gameObjects[randomIndex].Value;
			}
			
		}
	}
}