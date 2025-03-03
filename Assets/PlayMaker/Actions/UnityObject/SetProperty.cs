// (c) Copyright HutongGames, LLC. All rights reserved.

#if !UNITY_FLASH

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UnityObject)]
    [ActionTarget(typeof(Component), "targetProperty")]
    [ActionTarget(typeof(GameObject), "targetProperty")]
    [Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class SetProperty : FsmStateAction
	{
        [Tooltip("Target Property. See below for more details.")]
		public FsmProperty targetProperty;

        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			targetProperty = new FsmProperty {setProperty = true};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			targetProperty.SetValue();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			targetProperty.SetValue();
		}

#if UNITY_EDITOR
	    public override string AutoName()
        {
            var targetName = ActionHelpers.GetValueLabel(targetProperty.TargetObject);
            var name = targetName.Trim() + "." + (string.IsNullOrEmpty(targetProperty.PropertyName) ? "[none]" : targetProperty.PropertyName);
            var value = ActionHelpers.GetValueLabel(targetProperty.GetVariable());
	        return string.Format("{0} = {1}", name, value);
	    }
#endif
	}
}

#endif