// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns if additional layers affects the mass center")]
	public class GetAnimatorLayersAffectMassCenter : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
		
		[ActionSection("Results")]
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("If true, additional layers affects the mass center")]
		public FsmBool affectMassCenter;
		
		[Tooltip("Event send if additional layers affects the mass center")]
		public FsmEvent affectMassCenterEvent;
		
		[Tooltip("Event send if additional layers do no affects the mass center")]
		public FsmEvent doNotAffectMassCenterEvent;

        public override void Reset()
		{
			gameObject = null;
			affectMassCenter = null;
			affectMassCenterEvent = null;
			doNotAffectMassCenterEvent = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                bool _affect = cachedComponent.layersAffectMassCenter;

                affectMassCenter.Value = _affect;

                Fsm.Event(_affect ? affectMassCenterEvent : doNotAffectMassCenterEvent);
            }

            Finish();
        }
    }
}