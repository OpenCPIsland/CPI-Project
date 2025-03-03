// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Deactivate the GameObject that owns the FSM.")]
	public class DeactivateSelf : FsmStateAction
    {
        [Tooltip("Seconds to wait before deactivating. " +
                 "Can be useful to let other actions finish first. E.g. a fade-out effect." +
                 "\nNOTE: If you use a delay and the FSM switches away from this state before it is finished, the GameObject will not be deactivated.")]
        public FsmFloat delay;

        [Tooltip("Ignore any time scaling.")]
        public FsmBool realTime;

        private float elapsedTime;

		public override void Reset()
        {
            delay = null;
		}

		public override void OnEnter()
		{
            if (delay.Value < 0.001f)
            {
                DoDeactivateSelf();
                Finish();
            }

            elapsedTime = 0;
        }

        public override void OnUpdate()
        {
            elapsedTime += realTime.Value ? Time.unscaledDeltaTime : Time.deltaTime;

            if (elapsedTime > delay.Value)
            {
                DoDeactivateSelf();
                Finish();
            }
        }

        private void DoDeactivateSelf()
        {
            if (Owner != null)
            {
                Owner.SetActive(false);
            }
        }

#if UNITY_EDITOR

        public override string AutoName()
        {
            if (delay.Value > 0f)
                return "DeactivateSelf" + ActionHelpers.colon + " " + delay.Value + "s";
            return "Deactivate Self";
        }

        public override float GetProgress()
        {
            if (delay.Value > 0f)
                return Mathf.Clamp01(State.StateTime / delay.Value);
            return 0f;
        }

#endif
    }
}