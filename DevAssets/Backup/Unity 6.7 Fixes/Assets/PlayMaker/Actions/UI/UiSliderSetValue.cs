// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the value of a UI Slider component.")]
	public class UiSliderSetValue : ComponentAction<UnityEngine.UI.Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(UnityEngine.UI.Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The value of the UI Slider component.")]
		public FsmFloat value;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private UnityEngine.UI.Slider slider;
	    private float originalValue;

		public override void Reset()
		{
			gameObject = null;
			value = null;
			resetOnExit = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
		    {
                Finish();
                return;
            }

            slider = cachedComponent;

            originalValue = slider.value;

			DoSetValue();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoSetValue();
		}

	    private void DoSetValue()
		{
			if (slider != null)
			{
				slider.value = value.Value;
			}
		}

		public override void OnExit()
		{
			if (slider == null) return;
			
			if (resetOnExit.Value)
			{
				slider.value = originalValue;
			}
		}


#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("SliderSet", this.Fsm, gameObject, value);
        }
#endif
    }
}