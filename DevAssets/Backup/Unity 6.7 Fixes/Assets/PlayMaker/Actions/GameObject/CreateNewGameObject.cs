// (c) Copyright HutongGames, LLC. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Creates a new GameObject.\nUse a GameObject and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
	public class CreateNewGameObject : FsmStateAction
    {
        [Tooltip("Name of the new GameObject")]
        public FsmString name;

        [Tooltip("Optional Parent.")]
        public FsmGameObject parent;

		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		public override void Reset()
		{
			name = new FsmString { Value = "GameObject"};
            parent = null;
			spawnPoint = null;
			position = new FsmVector3 { UseVariable = true };
			rotation = new FsmVector3 { UseVariable = true };
			storeObject = null;
		}

		public override void OnEnter()
		{
            var spawnPosition = Vector3.zero;
			var spawnRotation = Vector3.zero;
			
			if (spawnPoint.Value != null)
			{
				spawnPosition = spawnPoint.Value.transform.position;
				
                if (!position.IsNone)
					spawnPosition += position.Value;
				
				if (!rotation.IsNone)
					spawnRotation = rotation.Value;
				else
					spawnRotation = spawnPoint.Value.transform.eulerAngles;
			}
			else
			{
				if (!position.IsNone)
					spawnPosition = position.Value;
				
				if (!rotation.IsNone)
					spawnRotation = rotation.Value;
			}

            var newObject = new GameObject(name.Value);

            if (parent.Value != null)
            {
                newObject.transform.SetParent(parent.Value.transform);
            }

            newObject.transform.position = spawnPosition;
            newObject.transform.eulerAngles = spawnRotation;

            storeObject.Value = newObject;

			Finish();
		}

#if UNITY_EDITOR

        public override string AutoName()
        {
            return "Create: " + ActionHelpers.GetValueLabel(name);
        }

#endif
	}
}