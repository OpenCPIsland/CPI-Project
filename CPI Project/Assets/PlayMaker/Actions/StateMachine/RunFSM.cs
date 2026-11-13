// (c) copyright Hutong Games, LLC. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Creates an FSM at runtime from a saved {{Template}}. The FSM is only active while the state is active. " +
             "This lets you nest FSMs inside states.\nThis is a very powerful action! " +
             "It allows you to create a library of FSM Templates that can be re-used in your project. " +
             "You can edit the template in one place and the changes are reflected everywhere." +
             "\nNOTE: You can also specify a template in the {{FSM Inspector}}.")]
    public class RunFSM : RunFSMAction
    {
        [Tooltip("The Template to use. You can drag and drop, use the Unity object browser, or the categorized popup browser to select a template.")]
        public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();

        [Tooltip("Event to send when the FSM has finished (usually because it ran a {{Finish FSM}} action).")]
        public FsmEvent finishEvent;

        [ActionSection]

        //[UIHint(UIHint.Variable)]
        //[Tooltip("This allows other actions to reference the created FSM (not widely used yet).")]
        //public FsmInt storeID;

        [Tooltip("Repeat every frame. Waits for the sub Fsm to finish before calling it again.")]
        public bool everyFrame;

        // Restart FSM if Finished and everyFrame == true
        private bool restart;

        public override void Reset()
        {
            fsmTemplateControl = new FsmTemplateControl();
            //storeID = null;
            runFsm = null;
            everyFrame = false;
        }

        /// <summary>
        /// Initialize FSM on awake so it doesn't cause hitches later
        /// </summary>
        public override void Awake()
        {
            HandlesOnEvent = true;

            fsmTemplateControl.Init();

            if (fsmTemplateControl.fsmTemplate != null && Application.isPlaying)
            {
                runFsm = Fsm.CreateSubFsm(fsmTemplateControl);
            }
        }

        /// <summary>
        /// Start the FSM on entering the state
        /// </summary>
        public override void OnEnter()
        {
            if (runFsm == null)
            {
                Finish();
                return;
            }

            fsmTemplateControl.UpdateValues();
            fsmTemplateControl.ApplyOverrides(runFsm);

            runFsm.OnEnable();
            runFsm.OnOutputEvent += OnOutputEvent;

            if (!runFsm.Started)
            {
                runFsm.Start();
            }

            //storeID.Value = fsmTemplateControl.ID;

            fsmTemplateControl.UpdateOutput(Fsm);

            CheckIfFinished();
        }


        public override void OnExit()
        {
            if (runFsm == null) return;

            runFsm.OnOutputEvent -= OnOutputEvent;
        }

        private void OnOutputEvent(FsmEvent fsmEvent)
        {
            // grab output variables again before processing the event
            fsmTemplateControl.UpdateOutput(Fsm);

            var outEvent = fsmTemplateControl.MapEvent(fsmEvent);
            if (outEvent == null) return;
            Fsm.Event(outEvent);
        }

        // Update Output variables in both Update and LateUpdate
        // May want to expose this as an option for performance
        // But right now it's the safest option...

        public override void OnUpdate()
        {
            if (restart)
            {
                OnEnter();
                restart = false;
            }
            else if (runFsm != null)
            {
                runFsm.Update();
                fsmTemplateControl.UpdateOutput(Fsm);
                CheckIfFinished();
            }
            else
            {
                Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (runFsm != null)
            {
                runFsm.LateUpdate();
                fsmTemplateControl.UpdateOutput(Fsm);
                CheckIfFinished();
            }
            else
            {
                Finish();
            }
        }

        // Other functionality covered in RunFSMAction base class

        protected override void CheckIfFinished()
        {
            if (runFsm == null)
            {
                Finish();
                return;
            }

            if (runFsm.Finished)
            {
                if (!everyFrame)
                {
                    Finish();
                    Fsm.Event(finishEvent);
                }
                else
                {
                    restart = true;
                }
            }
        }

#if UNITY_EDITOR

        public override string AutoName()
        {
            var template = fsmTemplateControl != null ? fsmTemplateControl.fsmTemplate : null;
            return "Run: " + (template != null ? template.name : "[none]");
        }

#endif
    }
}
