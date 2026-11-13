// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#if !UNITY_FLASH

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
    [ActionTarget(typeof(Component), "targetProperty")]
    [ActionTarget(typeof(GameObject), "targetProperty")]
	[Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class GetProperty : FsmStateAction
	{
        [Tooltip("TargetObject:\nAny object derived from UnityEngine.Object. " +
                 "For example, you can drag a Component from the Unity Inspector into this field. " +
                 "HINT: Use {{Lock}} to lock the current FSM selection if you need to drag a component from another GameObject." +
                 "\nProperty:\nUse the property selection menu to select the property to get. " +
                 "Note: You can drill into the property, e.g., transform.localPosition.x." +
                 "\nStore Result:\nStore the result in a variable.")]
		public FsmProperty targetProperty;

        [Tooltip("Repeat every frame. Useful if the property is changing over time.")]
        public bool everyFrame;

		public override void Reset()
		{
			targetProperty = new FsmProperty { setProperty = false };
			everyFrame = false;
		}

		public override void OnEnter()
		{
			targetProperty.GetValue();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			targetProperty.GetValue();
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            var name = string.IsNullOrEmpty(targetProperty.PropertyName) ? "[none]" : targetProperty.PropertyName;
            return "Get Property: "+ name;
            //var value = ActionHelpers.GetValueLabel(targetProperty.GetVariable());
            //return string.Format("Get {0} to {1}", name, value);
        }
#endif
	}
}

#endif