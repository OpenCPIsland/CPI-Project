// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Sets the current Time of an Animation. Useful for manually controlling playback of an animation. Check Every Frame to update the time continuously.")]
	public class SetAnimationTime : BaseAnimationAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
        [Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;
		[RequiredField]
		[UIHint(UIHint.Animation)]
        [Tooltip("The name of the animation.")]
		public FsmString animName;
        [Tooltip("The time to set the animation to.")]
		public FsmFloat time;
        [Tooltip("Use normalized time: 0 = start ; 1 = end. Useful if you don't care about the length of the exact length of the animation.")]
		public bool normalized;
        [Tooltip("Set time every frame. Useful if you're using a variable as Time.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			time = null;
			normalized = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAnimationTime(gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value);
			
			if (!everyFrame)
				Finish();
		}

		public override void OnUpdate()
		{
			DoSetAnimationTime(gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value);
		}

	    private void DoSetAnimationTime(GameObject go)
		{
		    if (!UpdateCache(go))
		    {
		        return;
		    }

			animation.Play(animName.Value);

			var anim = animation[animName.Value];
			if (anim == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				return;
			}

			if (normalized)
			{
				anim.normalizedTime = time.Value;
			}
			else
			{
				anim.time = time.Value;
			}
			
			// TODO: need to do this?
		    if (everyFrame)
		    {
		        anim.speed = 0;
		    }
		}

	}
}