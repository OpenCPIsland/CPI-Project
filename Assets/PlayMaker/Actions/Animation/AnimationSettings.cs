// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Applies animation settings to the specified animation. " +
             "Note: Settings are applied once, on entering the state, not continuously. " +
             "Use {{Set Animation Speed}}, {{Set Animation Time}} etc. if you need to update those animation settings every frame." +
             "\\nSee <a href=\"https://docs.unity3d.com/Manual/AnimationScripting.html\" rel =\"nofollow\" target=\"_blank\">Unity Animation Docs</a> " +
             "for detailed descriptions of Wrap Mode, Blend Mode, Speed and Layer settings.")]

    public class AnimationSettings : BaseAnimationAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("A GameObject with an Animation Component.")]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation. Use the browse button to select from animations on the Game Object (if available).")]
		public FsmString animName;
		
		[Tooltip("Set how the animation wraps (Loop, PingPong etc.). " +
                 "NOTE: Because of the way WrapMode is defined by Unity you cannot select Once, but Clamp is the same as Once.")]
		public WrapMode wrapMode;
		
		[Tooltip("How the animation is blended with other animations on the Game Object.")]
		public AnimationBlendMode blendMode;
		
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Speed up or slow down the animation. 1 is normal speed, 0.5 is half speed...")]
		public FsmFloat speed;
		
		[Tooltip("You can play animations on different layers to combine them into a final animation. " +
                 "See the Unity Animation docs for more details.")]
		public FsmInt layer;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			wrapMode = WrapMode.Loop;
			blendMode = AnimationBlendMode.Blend;
			speed = 1.0f;
			layer = 0;
		}

		public override void OnEnter()
		{
			DoAnimationSettings();
			
			Finish();
		}

	    private void DoAnimationSettings()
		{
            if (string.IsNullOrEmpty(animName.Value))
            {
                return;
            }

			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(go))
			{
				return;
			}

			var anim = animation[animName.Value];
			if (anim == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				return;
			}

			anim.wrapMode = wrapMode;
			anim.blendMode = blendMode;
			
			if (!layer.IsNone)
			{
				anim.layer = layer.Value;
			}
			
			if (!speed.IsNone)
			{
				anim.speed = speed.Value;
			}
		}
	}
}