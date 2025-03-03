// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Intensity of a Light.")]
	public class SetLightIntensity : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("The intensity of the light.")]
        public FsmFloat lightIntensity;

        [Tooltip("Update every frame. Useful if the intensity is animated.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			lightIntensity = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetLightIntensity();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnUpdate()
		{
			DoSetLightIntensity();
		}
		
		void DoSetLightIntensity()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
                light.intensity = lightIntensity.Value;
		    }
		}
	}
}