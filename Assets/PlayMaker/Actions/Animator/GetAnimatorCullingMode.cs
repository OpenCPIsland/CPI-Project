// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the culling of this Animator component. Optionally sends events.\n" +
		"If true ('AlwaysAnimate'): always animate the entire character. Object is animated even when offscreen.\n" +
		 "If False ('BasedOnRenderers') animation is disabled when renderers are not visible.")]
	public class GetAnimatorCullingMode : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[ActionSection("Results")]
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
		public FsmBool alwaysAnimate;
		
		[Tooltip("Event send if culling mode is 'AlwaysAnimate'")]
		public FsmEvent alwaysAnimateEvent;
		
		[Tooltip("Event send if culling mode is 'BasedOnRenders'")]
		public FsmEvent basedOnRenderersEvent;
        
		public override void Reset()
		{
			gameObject = null;
			alwaysAnimate = null;
			alwaysAnimateEvent = null;
			basedOnRenderersEvent = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                var _alwaysOn = cachedComponent.cullingMode == AnimatorCullingMode.AlwaysAnimate;

                alwaysAnimate.Value = _alwaysOn;

                Fsm.Event(_alwaysOn ? alwaysAnimateEvent : basedOnRenderersEvent);
            }

            Finish();
        }
	}
}