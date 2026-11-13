using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AnimationStartRandomTime : FsmStateAction
	{
		public bool UseOwnerGameObject = false;

		[HutongGames.PlayMaker.Tooltip("Name of target game object")]
		public FsmGameObject Target;

		[HutongGames.PlayMaker.Tooltip("Name of animation to play")]
		public FsmString AnimName;

		[HutongGames.PlayMaker.Tooltip("Layer index (leave as -1 for topmost layer)")]
		public FsmInt LayerIndex = -1;

		[HasFloatSlider(0f, 1f)]
		[HutongGames.PlayMaker.Tooltip("Start time to randomize from")]
		public FsmFloat AnimStart;

		[HasFloatSlider(0f, 1f)]
		[HutongGames.PlayMaker.Tooltip("End time to randomize to")]
		public FsmFloat AnimEnd;

		private Animator anim;

		private void InitializeComponents()
		{
			if (anim == null)
			{
				if (UseOwnerGameObject)
				{
					anim = base.Owner.GetComponentInParent<Animator>();
				}
				else if (Target.Value != null)
				{
					anim = Target.Value.GetComponent<Animator>();
				}
			}
		}

		public override void OnEnter()
		{
			InitializeComponents();
			if (anim != null && !string.IsNullOrEmpty(AnimName.Value))
			{
				float normalizedTime = Random.Range(Mathf.Clamp(AnimStart.Value, 0f, 1f), Mathf.Clamp(AnimEnd.Value, 0f, 1f));
				anim.Play(AnimName.Value, LayerIndex.Value, normalizedTime);
			}
			Finish();
		}
	}
}
