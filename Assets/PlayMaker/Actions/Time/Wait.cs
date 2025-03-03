// (c) Copyright HutongGames, LLC. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [Tooltip("Delays a State from finishing. Optionally send an event after the specified time. " + 
             "NOTE: Other actions continue running and can send events before this action finishes.")]
    public class Wait : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Time to wait in seconds.")]
        public FsmFloat time;

        [Tooltip("Event to send after the specified time.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale. E.g., if the game is paused using Scale Time.")]
        public bool realTime;

        private float startTime;
        private float timer;

        public override void Reset()
        {
            time = 1f;
            finishEvent = null;
            realTime = false;
        }

        public override void OnEnter()
        {
            if (time.Value <= 0)
            {
                Fsm.Event(finishEvent);
                Finish();
                return;
            }

            startTime = FsmTime.RealtimeSinceStartup;
            timer = 0f;
        }

        public override void OnUpdate()
        {
            // update time

            if (realTime)
            {
                timer = FsmTime.RealtimeSinceStartup - startTime;
            }
            else
            {
                timer += Time.deltaTime;
            }

            if (timer >= time.Value)
            {
                Finish();
                if (finishEvent != null)
                {
                    Fsm.Event(finishEvent);
                }
            }
        }



#if UNITY_EDITOR

        public override string AutoName()
        {
            return "Wait " + ActionHelpers.GetValueLabel(time) + "s" + " " + (finishEvent != null ? finishEvent.Name : "");
        }

        public override float GetProgress()
        {
            return Mathf.Min(timer / time.Value, 1f);
        }

#endif
    }
}
