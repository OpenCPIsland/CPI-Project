// (c) copyright Hutong Games, LLC. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Block events while this action is active.")]
    public class BlockEvents : FsmStateAction
    {
        public enum Options
        {
            Timeout,
            WhileTrue,
            WhileFalse,
            UntilTrue,
            UntilFalse,
            UntilEvent
        };

        [Tooltip("When to block events.")]
        public Options condition;

        [Tooltip("Context sensitive parameter. Depends on Condition.")]
        public FsmFloat floatParam;
        [Tooltip("Context sensitive parameter. Depends on Condition.")]
        public FsmBool boolParam;

        [EventNotSent]
        [Tooltip("Context sensitive parameter. Depends on Condition.")]
        public FsmEvent eventParam;

        [ActionSection("Debug")]

        [Tooltip("Log any events blocked by this action. Helpful for debugging.")]
        public FsmBool logBlockedEvents;

        private bool firstTime = true;

        public override void Reset()
        {
            condition = Options.Timeout;
            floatParam = null;
            boolParam = null;
            eventParam = null;
            logBlockedEvents = false;
        }

        public override void Awake()
        {
            HandlesOnEvent = true;
        }

        public override void OnUpdate()
        {
            switch (condition)
            {
                case Options.Timeout:

                    if (boolParam.Value) // real time
                    {
                        if (FsmTime.RealtimeSinceStartup - State.RealStartTime > floatParam.Value)
                            Finish();
                    }
                    else
                    {
                        if (State.StateTime > floatParam.Value)
                            Finish();
                    }

                    break;

                case Options.WhileFalse:
                case Options.WhileTrue:

                    // Does not finish
                    break;

                case Options.UntilFalse:

                    if (!boolParam.Value)
                        Finish();
                    break;

                case Options.UntilTrue:

                    if (boolParam.Value)
                        Finish();
                    break;

                case Options.UntilEvent:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Return true to block the event
        /// </summary>
        public override bool Event(FsmEvent fsmEvent)
        {
            if (firstTime)
            {
                if (!Validate())
                    Finish();

                firstTime = false;
            }

            if (Finished) return false;

            if (Fsm.EventData.SentByState == State || fsmEvent == FsmEvent.Finished)
                return false;

            var blocked = DoBlockEvent(fsmEvent);

            if (blocked && logBlockedEvents.Value)
            {
                ActionHelpers.DebugLog(Fsm, LogLevel.Info,"Blocked: " + fsmEvent.Name, true);
            }

            return blocked;
        }

        private bool Validate()
        {
            switch (condition)
            {
                case Options.Timeout:
                    return !floatParam.IsNone;
                case Options.WhileTrue:
                case Options.WhileFalse:
                case Options.UntilTrue:
                case Options.UntilFalse:
                    return !boolParam.IsNone;
                case Options.UntilEvent:
                    return !string.IsNullOrEmpty(eventParam.Name);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool DoBlockEvent(FsmEvent fsmEvent)
        {
            switch (condition)
            {
                case Options.Timeout:

                    if (boolParam.Value) // real time
                    {
                        if (FsmTime.RealtimeSinceStartup - State.RealStartTime < floatParam.Value) 
                            return true;
                    }
                    else
                    {
                        if (State.StateTime < floatParam.Value) 
                            return true;
                    }
                    
                    return false;

                case Options.WhileFalse:

                    return !boolParam.Value;

                case Options.WhileTrue:

                    return boolParam.Value;

                case Options.UntilFalse:

                    return boolParam.Value;

                case Options.UntilTrue:

                    return !boolParam.Value;

                case Options.UntilEvent:

                    if (fsmEvent.Name == eventParam.Name)
                    {
                        Finish();

                        if (boolParam.Value) // use this event
                        {
                            return false;
                        }
                    }

                    return true;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR

        public override float GetProgress()
        {
            if (condition == Options.Timeout)
            {
                if (boolParam.Value) // real time
                {
                    var elapsed = FsmTime.RealtimeSinceStartup - State.RealStartTime;
                    return Mathf.Min(elapsed / floatParam.Value, 1f);
                }

                return Mathf.Min(State.StateTime / floatParam.Value, 1f);
            }

            return 0f;
        }

#endif
    }
}
