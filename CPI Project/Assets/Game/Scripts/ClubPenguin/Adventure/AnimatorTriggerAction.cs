using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Animation")]
	public class AnimatorTriggerAction : FsmStateAction
	{
		public bool UseOwnerGameObject = false;

		public FsmGameObject Target;

		public FsmString Parameter;

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
            if (anim != null && !isRepeatDoorRequest())
            {
                anim.SetTrigger(Parameter.Value);
            }
            Finish();
        }

        private bool isRepeatDoorRequest()
        {
            if (anim.runtimeAnimatorController == null || !anim.runtimeAnimatorController.name.Contains("DoorDoubleHinged"))
            {
                return false;
            }
            AnimatorStateInfo state = anim.IsInTransition(0) ? anim.GetNextAnimatorStateInfo(0) : anim.GetCurrentAnimatorStateInfo(0);
            if (Parameter.Value == "Opening") return state.IsName("Opening") || state.IsName("Opened");
            if (Parameter.Value == "Closing") return state.IsName("Closing") || state.IsName("Closed");
            return false;
        }
    }
}
