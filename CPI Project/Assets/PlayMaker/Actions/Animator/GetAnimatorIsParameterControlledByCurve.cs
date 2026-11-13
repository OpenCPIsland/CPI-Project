// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
	public class GetAnimatorIsParameterControlledByCurve: ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The parameter's name")]
		public FsmString parameterName;
		
		[ActionSection("Results")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("True if controlled by curve")]
		public FsmBool isControlledByCurve;
		
		[Tooltip("Event send if controlled by curve")]
		public FsmEvent isControlledByCurveEvent;
		
		[Tooltip("Event send if not controlled by curve")]
		public FsmEvent isNotControlledByCurveEvent;

		public override void Reset()
		{
			gameObject = null;
			parameterName = null;
			isControlledByCurve = null;
			isControlledByCurveEvent = null;
			isNotControlledByCurveEvent = null;
        }
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                var _isControlledByCurve = cachedComponent.IsParameterControlledByCurve(parameterName.Value);
                isControlledByCurve.Value = _isControlledByCurve;

                Fsm.Event(_isControlledByCurve ? isControlledByCurveEvent : isNotControlledByCurveEvent);
            }

			Finish();
        }
		
	}
}