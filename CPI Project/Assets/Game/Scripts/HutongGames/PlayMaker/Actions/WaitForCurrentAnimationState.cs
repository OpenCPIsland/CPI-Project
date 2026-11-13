using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Delays a State from finishing by the specified time of the current animation state.")]
	[ActionCategory("Animation")]
	public class WaitForCurrentAnimationState : FsmStateAction
	{
		public FsmEvent finishEvent;

		public bool realTime;

		private float startTime;

		public FsmInt LayerIndex = 0;

		public bool UseOwnerGameObject = false;

		public FsmGameObject Target;

		private Animator anim;

		private float timer;

		private float time;

		private void InitializeComponents()
		{
			if (anim == null)
			{
				if (UseOwnerGameObject)
				{
					anim = base.Owner.GetComponentInParent<Animator>();
				}
				else
				{
					anim = Target.Value.GetComponent<Animator>();
				}
			}
		}

		public override void Reset()
		{
			time = 1f;
			finishEvent = null;
			realTime = false;
		}

		public override void OnEnter()
		{
			InitializeComponents();
			startTime = FsmTime.RealtimeSinceStartup;
			timer = 0f;
			if (anim != null)
			{
				int layerIndex = anim.GetLayerIndex("Base Layer");
				AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(layerIndex);
				time = currentAnimatorStateInfo.length;
				timer = currentAnimatorStateInfo.length * currentAnimatorStateInfo.normalizedTime;
			}
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				timer = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				timer += Time.deltaTime;
			}
			if (timer >= time)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
		}
	}
}
