// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

#if !(UNITY_SWITCH || UNITY_TVOS || UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID || UNITY_FLASH || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_BLACKBERRY || UNITY_METRO || UNITY_WP8 || UNITY_PSM || UNITY_WEBGL)

using System;
using UnityEngine;

#if UNITY_2018_2_OR_NEWER
#pragma warning disable 618  
#endif

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.Material)]
#if UNITY_2019_3_OR_NEWER
    // Mark Obsolete but keep parameters
    // so user can convert them to new actions.
    [Obsolete("Use VideoPlayer actions instead.")]
#endif
    [Tooltip("Sets a named texture in a game object's material to a movie texture.")]
	public class SetMaterialMovieTexture : ComponentAction<Renderer>
	{
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;
		
		[UIHint(UIHint.NamedTexture)]
		[Tooltip("A named texture in the shader.")]
		public FsmString namedTexture;

		[RequiredField]
#if !UNITY_2019_3_OR_NEWER
        [ObjectType(typeof(MovieTexture))]
#endif
        [Tooltip("The Movie Texture to use.")]
        public FsmObject movieTexture;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			material = null;
			namedTexture = "_MainTex";
			movieTexture = null;
		}

		public override void OnEnter()
		{
			DoSetMaterialTexture();
			Finish();
		}

		void DoSetMaterialTexture()
		{
#if !UNITY_2019_3_OR_NEWER
			var movie = movieTexture.Value as MovieTexture;

			var namedTex = namedTexture.Value;
			if (namedTex == "") namedTex = "_MainTex";

			if (material.Value != null)
			{
				material.Value.SetTexture(namedTex, movie);
				return;
			}

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
				renderer.material.SetTexture(namedTex, movie);
			}
			else if (renderer.materials.Length > materialIndex.Value)
			{
				var materials = renderer.materials;
				materials[materialIndex.Value].SetTexture(namedTex, movie);
				renderer.materials = materials;
			}
#endif
        }

#if UNITY_2019_3_OR_NEWER
        public override string ErrorCheck()
        {
            return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
        }
#endif
    }
}

#endif