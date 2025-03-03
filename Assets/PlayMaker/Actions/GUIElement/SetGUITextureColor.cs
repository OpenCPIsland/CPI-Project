// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
#if !UNITY_2019_3_OR_NEWER

	[ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the color of the <a href=\"http://unity3d.com/support/documentation/Components/class-GuiTexture.html\">GUI Texture</a> attached to a game object.")]
    #if UNITY_2017_2_OR_NEWER
#pragma warning disable 618
    [Obsolete("GUITexture is part of the legacy UI system and will be removed in a future release")]
	#endif
	public class SetGUITextureColor : ComponentAction<GUITexture>
	{
		[RequiredField]
		[CheckForComponent(typeof(GUITexture))]
        [Tooltip("The Game Object that has the GUITexture component.")]
        public FsmOwnerDefault gameObject;
		[RequiredField]
        [Tooltip("The color to use. Useful for tinting textures in different states, e.g., rollover.")]
        public FsmColor color;
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			color = Color.white;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetGUITextureColor();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetGUITextureColor();
		}

		void DoSetGUITextureColor()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
				guiTexture.color = color.Value;
			}
		}
	}

#else

    [ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
    [Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
    public class SetGUITextureColor : FsmStateAction
    {
        [ActionSection("Obsolete. Use Unity UI instead.")]

        public FsmColor color;
    }

#endif
}