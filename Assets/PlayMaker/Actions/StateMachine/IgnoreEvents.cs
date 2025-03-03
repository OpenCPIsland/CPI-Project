// (c) copyright Hutong Games, LLC. All rights reserved.

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Ignore specified events while this action is active.")]
    public class IgnoreEvents : FsmStateAction
    {
        [Serializable]
        public enum EventType
        {
            mouse,
            application,
            collision,
            collision2d,
            trigger,
            trigger2d,
            UI,
            anyUnityEvent
        }

        [Tooltip("Type of events to ignore.")]
        public EventType[] eventTypes;

        [Tooltip("Event names to ignore.")]
        [UIHint(UIHint.FsmEvent)]
        public FsmString[] events;
        
        [ActionSection("Debug")]

        [Tooltip("Log any events blocked by this action. Helpful for debugging.")]
        public FsmBool logIgnoredEvents;

        public override void Reset()
        {
            eventTypes = new EventType[0];
            events = new FsmString[0];
            logIgnoredEvents = false;
        }

        public override void Awake()
        {
            HandlesOnEvent = true;
            BlocksFinish = false;
        }

        /// <summary>
        /// Return true to block the event
        /// </summary>
        public override bool Event(FsmEvent fsmEvent)
        {
            var ignored = DoIgnoreEvent(fsmEvent);

            if (ignored && logIgnoredEvents.Value)
            {
                ActionHelpers.DebugLog(Fsm, LogLevel.Info,"Ignored: " + fsmEvent.Name, true);
            }

            return ignored;
        }

        private bool DoIgnoreEvent(FsmEvent fsmEvent)
        {
            if (fsmEvent == null) return false;

            foreach (var eventType in eventTypes)
            {
                switch (eventType)
                {
                    case EventType.anyUnityEvent:
                        if (fsmEvent.IsUnityEvent) return true;
                        break;
                    case EventType.mouse:
                        if (fsmEvent.IsMouseEvent) return true;
                        break;
                    case EventType.application:
                        if (fsmEvent.IsApplicationEvent) return true;
                        break;
                    case EventType.collision:
                        if (fsmEvent.IsCollisionEvent) return true;
                        break;
                    case EventType.collision2d:
                        if (fsmEvent.IsCollision2DEvent) return true;
                        break;
                    case EventType.trigger:
                        if (fsmEvent.IsTriggerEvent) return true;
                        break;
                    case EventType.trigger2d:
                        if (fsmEvent.IsTrigger2DEvent) return true;
                        break;
                    case EventType.UI:
                        if (fsmEvent.IsUIEvent) return true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var fsmEventName = fsmEvent.Name;

            for (var i = 0; i < events.Length; i++)
            {
                if (events[i].Value == fsmEventName)
                    return true;
            }

            return false;
        }

    }
}
