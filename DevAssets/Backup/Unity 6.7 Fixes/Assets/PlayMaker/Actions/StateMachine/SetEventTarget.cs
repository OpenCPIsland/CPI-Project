// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
    public class SetEventTarget : FsmStateAction
    {
        [Tooltip("Set the target.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            eventTarget = null;
            everyFrame = true;
        }

        public override void Awake()
        {
            BlocksFinish = false;
        }

        public override void OnEnter()
        {
            Fsm.EventTarget = eventTarget;

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            Fsm.EventTarget = eventTarget;
        }
    }
}