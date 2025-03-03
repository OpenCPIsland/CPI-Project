// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the Background Color used by the Camera.")]
	public class SetBackgroundColor : ComponentAction<Camera>
	{
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
        [Tooltip("The game object that owns the Camera component.")]
		public FsmOwnerDefault gameObject;
		[RequiredField]
        [Tooltip("The background color.")]
		public FsmColor backgroundColor;

        [Tooltip("Repeat every frame. Useful if the color is animated.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			backgroundColor = Color.black;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetBackgroundColor();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetBackgroundColor();
		}
		
		void DoSetBackgroundColor()
		{
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
                camera.backgroundColor = backgroundColor.Value;
		    }
		}
	}
}