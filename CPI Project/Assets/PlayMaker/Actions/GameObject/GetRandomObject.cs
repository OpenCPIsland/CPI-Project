// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
	public class GetRandomObject : FsmStateAction
	{
		[UIHint(UIHint.Tag)]
        [Tooltip("Only select from Game Objects with this Tag.")]
		public FsmString withTag;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a GameObject Variable.")]
        public FsmGameObject storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			withTag = "Untagged";
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetRandomObject();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetRandomObject();
		}

		void DoGetRandomObject()
		{
			GameObject[] gameObjects;

			if (withTag.Value != "Untagged")
			{
				gameObjects = GameObject.FindGameObjectsWithTag(withTag.Value);
			}
			else
			{
#if UNITY_2022_3_OR_NEWER
				gameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
				gameObjects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
#endif
			}

			if (gameObjects.Length > 0)
			{
				storeResult.Value = gameObjects[Random.Range(0, gameObjects.Length)];
				return;
			}

			storeResult.Value = null;
		}
	}
}