// (c) Copyright HutongGames, LLC. All rights reserved.

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
#if !UNITY_2019_3_OR_NEWER

	[ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the texture used by the <a href=\"http://unity3d.com/support/documentation/Components/class-GuiTexture.html\">GUI Texture</a> attached to a Game Object.")]
    #if UNITY_2017_2_OR_NEWER
#pragma warning disable 618
    [Obsolete("GUITexture is part of the legacy UI system and will be removed in a future release")]
	#endif
	public class SetGUITexture : ComponentAction<GUITexture>
	{
		[RequiredField]
		[CheckForComponent(typeof(GUITexture))]
		[Tooltip("The GameObject that owns the GUITexture.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Texture to apply.")]
		public FsmTexture texture;
		
		public override void Reset()
		{
			gameObject = null;
			texture = null;
		}

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
				guiTexture.texture = texture.Value;
			}
			
			Finish();
		}
	}
#else

    [ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
    [Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
    public class SetGUITexture : FsmStateAction
    {
        [ActionSection("Obsolete. Use Unity UI instead.")]

        public FsmTexture texture;
    }

#endif


}

