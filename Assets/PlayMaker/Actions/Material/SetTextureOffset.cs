// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the Offset of a named texture in a Game Object's Material. Useful for scrolling texture effects.")]
	public class SetTextureOffset : ComponentAction<Renderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
        [Tooltip("The target Game Object.")]
        public FsmOwnerDefault gameObject;
        [Tooltip("The index of the material on the object.")]
        public FsmInt materialIndex;
		[RequiredField]
        [Tooltip("The named texture. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Material.SetTextureOffset.html\" rel=\"nofollow\">SetTextureOffset</a>")]
        [UIHint(UIHint.NamedColor)]
		public FsmString namedTexture;
		[RequiredField]
        [Tooltip("The amount to offset in X axis. 1 = full width of texture.")]
        public FsmFloat offsetX;
		[RequiredField]
        [Tooltip("The amount to offset in Y axis. 1 = full height of texture.")]
        public FsmFloat offsetY;
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			namedTexture = "_MainTex";
			offsetX = 0;
			offsetY = 0;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetTextureOffset();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetTextureOffset();
		}

		void DoSetTextureOffset()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (!UpdateCache(go))
		    {
		        return;
		    }
			
			if (renderer.material == null)
			{
				LogError("Missing Material!");
				return;
			}
			
			if (materialIndex.Value == 0)
			{
				renderer.material.SetTextureOffset(namedTexture.Value, new Vector2(offsetX.Value, offsetY.Value));
			}
			else if (renderer.materials.Length > materialIndex.Value)
			{
				var materials = renderer.materials;
				materials[materialIndex.Value].SetTextureOffset(namedTexture.Value, new Vector2(offsetX.Value, offsetY.Value));
				renderer.materials = materials;
			}
		}

	}
}