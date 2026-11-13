// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Set Spot, Directional, or Point Light type.")]
	public class SetLightType : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
		public FsmOwnerDefault gameObject;
		
        [ObjectType(typeof(LightType))]
        [Tooltip("Spot, directional, or point light.")]
        public FsmEnum lightType;

		public override void Reset()
		{
			gameObject = null;
			lightType = LightType.Point;
		}

		public override void OnEnter()
		{
			DoSetLightType();
			Finish();
		}
		
		void DoSetLightType()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
                light.type = (LightType)lightType.Value;
		    }
		}
	}
}