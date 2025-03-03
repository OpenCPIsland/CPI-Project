// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Texture projected by a Light.")]
	public class SetLightCookie : ComponentAction<Light>
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
        [Tooltip("The Game Object with the Light Component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The texture to project.")]
        public FsmTexture lightCookie;

		public override void Reset()
		{
			gameObject = null;
			lightCookie = null;
		}

		public override void OnEnter()
		{
			DoSetLightCookie();
			Finish();
		}
	
		void DoSetLightCookie()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
                light.cookie = lightCookie.Value;
			}
		}
	}
}