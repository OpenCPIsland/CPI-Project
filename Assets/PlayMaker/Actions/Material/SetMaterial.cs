// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the material on a Game Object.")]
	public class SetMaterial : ComponentAction<Renderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
        [Tooltip("A Game Object with a Renderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The index of the material on the object.")]
        public FsmInt materialIndex;
		
        [RequiredField]
        [Tooltip("The material to apply.")]
        public FsmMaterial material;

		public override void Reset()
		{
			gameObject = null;
			material = null;
			materialIndex = 0;
		}

		public override void OnEnter()
		{
			DoSetMaterial();
			
			Finish();
		}

		void DoSetMaterial()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (!UpdateCache(go))
		    {
		        return;
		    }

			if (materialIndex.Value == 0)
			{
				renderer.material = material.Value;
			}
			else if (renderer.materials.Length > materialIndex.Value)
			{
				var materials = renderer.materials;
				materials[materialIndex.Value] = material.Value;
				renderer.materials = materials;
			}
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, material);
	    }
#endif
	}
}