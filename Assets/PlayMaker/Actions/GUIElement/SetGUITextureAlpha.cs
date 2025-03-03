// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
#if !UNITY_2019_3_OR_NEWER

	[ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the alpha of the <a href=\"http://unity3d.com/support/documentation/Components/class-GuiTexture.html\">GUI Texture</a> attached to a game object. Useful for fading gui elements in/out.")]
    #if UNITY_2017_2_OR_NEWER
#pragma warning disable 618
    [Obsolete("GUITexture is part of the legacy UI system and will be removed in a future release")]
	#endif
	public class SetGUITextureAlpha : ComponentAction<GUITexture>
	{
		[RequiredField]
		[CheckForComponent(typeof(GUITexture))]
        [Tooltip("The Game Object that has the GUITexture component.")]
		public FsmOwnerDefault gameObject;
		[RequiredField]
        [Tooltip("The alpha to use. HINT: Use {{Animate Float}} for cool effects!")]
        public FsmFloat alpha;
        [Tooltip("Update the alpha every frame.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			alpha = 1.0f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGUITextureAlpha();
			
			if(!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGUITextureAlpha();
		}
		
		void DoGUITextureAlpha()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
				var color = guiTexture.color;
				guiTexture.color = new Color(color.r, color.g, color.b, alpha.Value);
			}			
		}
	}

#else

    [ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the Alpha of the GUITexture attached to a Game Object. Useful for fading GUI elements in/out.")]
    [Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
    public class SetGUITextureAlpha : FsmStateAction
    {
        [ActionSection("Obsolete. Use Unity UI instead.")]

        public FsmFloat alpha;
    }

#endif
}