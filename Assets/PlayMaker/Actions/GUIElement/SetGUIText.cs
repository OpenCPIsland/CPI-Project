// (c) Copyright HutongGames, LLC. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
#if !UNITY_2019_3_OR_NEWER

	[ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the text used by the <a href=\"http://unity3d.com/support/documentation/Components/class-GuiText.html\">GUI Text</a> component attached to a Game Object.")]
    #if UNITY_2017_2_OR_NEWER
#pragma warning disable 618
    [Obsolete("GUIText is part of the legacy UI system and will be removed in a future release")]
	#endif
	public class SetGUIText : ComponentAction<GUIText>
	{
		[RequiredField]
		[CheckForComponent(typeof(GUIText))]
        [Tooltip("The game object that has the GUIText component.")]
		public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("Set the text used by the GUIText component.")]
		public FsmString text;

        [Tooltip("Repeat every frame. Useful if the text is changing.")]
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			text = "";
		}

		public override void OnEnter()
		{
			DoSetGUIText();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetGUIText();
		}
		
		void DoSetGUIText()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
		        guiText.text = text.Value;
		    }
		}
	}
#else

    [ActionCategory(ActionCategory.GUIElement)]
    [Tooltip("Sets the Text used by the GUIText Component attached to a Game Object.")]
    [Obsolete("GUIText is part of the legacy UI system removed in 2019.3")]
    public class SetGUIText : FsmStateAction
    {
        [ActionSection("Obsolete. Use Unity UI instead.")]

        [UIHint(UIHint.TextArea)]
        public FsmString text;
    }

#endif
}

