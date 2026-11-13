using ClubPenguin.Actions;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
    public class ActionSequencer : MonoBehaviour
    {
        public class State
        {
            public readonly EntityId OwnerInstanceId;
            public GameObject Owner;
            public GameObject Trigger;
            public List<ClubPenguin.Actions.Action> Actions;
            public HashSet<GameObject> Targets;
            public bool AbortOnUserInput;

            public State(GameObject _owner, GameObject _trigger)
            {
                Owner = _owner;
                OwnerInstanceId = _owner.GetEntityId();
                Trigger = _trigger;
                Actions = new List<ClubPenguin.Actions.Action>();
                Targets = new HashSet<GameObject>();
                AbortOnUserInput = false;
            }

            public override string ToString()
            {
                return string.Format("[State] Owner={0}, Trigger={1}, Targets={2}, AbortOnUserInput={3}", Owner, Trigger, Targets, AbortOnUserInput);
            }
        }

        private Dictionary<EntityId, State> sequencerDict;
        private List<State> sequencerList;

        public event Action<GameObject> SequenceCompleted;

        private void Awake()
        {
            sequencerDict = new Dictionary<EntityId, State>();
            sequencerList = new List<State>();
            SceneRefs.SetActionSequencer(this);

            ActionSequencer[] array = UnityEngine.Object.FindObjectsByType<ActionSequencer>();
            if (array.Length > 1)
            {
                Log.LogError(this, "There are " + array.Length + " instances of ActionSequencer. Only 1 instance should exist at any given time.");
            }
        }

        public void OnDestroy()
        {
            SequenceCompleted = null;
        }

        private void SafeRemoveInteractor(State state)
        {
            if (state == null || state.Trigger == null)
                return;

            SharedActionGraphState component = state.Trigger.GetComponent<SharedActionGraphState>();
            if (component == null)
                return;

            if (state.Owner != null)
                component.Interactors.Remove(state.Owner);
            else
                component.Interactors.RemoveWhere(go => go == null);
        }

        public GameObject GetTrigger(GameObject owner)
        {
            if (owner == null)
                return null;

            EntityId id = owner.GetEntityId();

            if (sequencerDict.ContainsKey(id))
                return sequencerDict[id].Trigger;

            return null;
        }

        public SharedActionGraphState GetSharedActionGraphState(GameObject trigger)
        {
            if (trigger == null)
                return null;

            return trigger.GetComponent<SharedActionGraphState>();
        }

        public bool StartSequence(GameObject owner, GameObject trigger)
        {
            if (owner == null)
            {
                Log.LogErrorFormatted(this, "owner is null when starting a sequence for trigger {0}", trigger);
                return false;
            }

            EntityId id = owner.GetEntityId();

            if (sequencerDict.ContainsKey(id))
                return false;

            ClubPenguin.Actions.Action[] components = trigger.GetComponents<ClubPenguin.Actions.Action>();

            if (components.Length == 0)
                return false;

            SharedActionGraphState shared = trigger.GetComponent<SharedActionGraphState>();
            if (shared == null)
                shared = trigger.AddComponent<SharedActionGraphState>();

            if (shared.MaxInteractors > -1 && shared.Interactors.Count >= shared.MaxInteractors)
                return false;

            State state = new State(owner, trigger);
            state.Actions.Capacity = components.Length;

            for (int i = 0; i < components.Length; i++)
            {
                GameObject go = components[i].GetTarget();

                if (go == null)
                    go = owner;

                if (!go.IsDestroyed())
                {
                    state.Targets.Add(go);

                    ClubPenguin.Actions.Action action = components[i].AddToGameObject(go);
                    action.Owner = owner;
                    state.Actions.Add(action);
                }
            }

            sequencerDict.Add(id, state);
            sequencerList.Add(state);

            shared.Interactors.Add(owner);

            enableRootActions(state);

            if (owner.CompareTag("Player"))
            {
                Service.Get<EventDispatcher>().DispatchEvent(
                    new ActionSequencerEvents.ActionSequenceStarted(trigger)
                );
            }

            return true;
        }

        public void StopSequence(GameObject owner)
        {
            if (owner == null)
                return;

            EntityId id = owner.GetEntityId();

            if (!sequencerDict.ContainsKey(id))
                return;

            abortSequence(sequencerDict[id]);
        }

        private void enableRootActions(State state)
        {
            for (int i = 0; i < state.Actions.Count; i++)
            {
                ClubPenguin.Actions.Action action = state.Actions[i];

                if (action.ParentId == -1 && action.ParentIdOnFalse == -1)
                    action.enabled = true;
            }
        }

        private void triggerInterrupts(State state, ClubPenguin.Actions.Action completedAction)
        {
            int id = completedAction.Id;

            for (int i = 0; i < state.Actions.Count; i++)
            {
                ClubPenguin.Actions.Action action = state.Actions[i];

                if (action != null && action.InterruptedBy == id && !action.Complete)
                    action.Completed();
            }
        }

        private bool enableDependentActions(State state, ClubPenguin.Actions.Action completedAction, object userData = null, bool conditionBranchValue = true)
        {
            int id = completedAction.Id;
            bool result = true;

            for (int i = 0; i < state.Actions.Count; i++)
            {
                ClubPenguin.Actions.Action action = state.Actions[i];

                if (action == null)
                {
                    result = false;
                    break;
                }

                int parent = conditionBranchValue ? action.ParentId : action.ParentIdOnFalse;

                if (parent == id)
                {
                    action.IncomingUserData = userData;
                    action.enabled = true;
                }
            }

            return result;
        }

        public void ActionCompleted(GameObject owner, ClubPenguin.Actions.Action action, object userData = null, bool conditionBranchValue = true)
        {
            if (owner == null)
            {
                Log.LogErrorFormatted(this, "Owner is null when an action is complete. Action = {0}, UserData = {1}", action, userData);
                OnActionAborted(owner, action);
                return;
            }

            EntityId id = owner.GetEntityId();

            if (!sequencerDict.ContainsKey(id))
                return;

            State state = sequencerDict[id];
            bool abort = false;

            if (action.EndAllOnExit)
            {
                destroyAllActions(state);
            }
            else
            {
                triggerInterrupts(state, action);

                if (enableDependentActions(state, action, userData, conditionBranchValue))
                {
                    state.Actions.Remove(action);
                    UnityEngine.Object.Destroy(action);

                    if (state.Actions.Count > 0)
                    {
                        bool anyEnabled = false;
                        int count = state.Actions.Count;
                        for (int i = 0; i < count; i++)
                        {
                            ClubPenguin.Actions.Action pending = state.Actions[i];
                            if (pending != null && pending.enabled)
                            {
                                anyEnabled = true;
                                break;
                            }
                        }
                        if (!anyEnabled)
                            destroyAllActions(state);
                    }
                }
                else
                {
                    abort = true;
                }
            }

            if (abort)
                abortSequence(state);
            else if (state.Actions.Count == 0)
                CompleteAndRemoveSequence(state);
        }

        private void CompleteAndRemoveSequence(State state)
        {
            SafeRemoveInteractor(state);
            sequencerDict.Remove(state.OwnerInstanceId);
            sequencerList.Remove(state);

            try
            {
                sequenceCompleted(state);
            }
            catch (Exception ex)
            {
                Log.LogError(this, ex.Message);
                Log.LogException(this, ex);
                if (state.Owner != null && state.Trigger != null)
                {
                    ParticipationController component = state.Owner.GetComponent<ParticipationController>();
                    if (component != null)
                    {
                        component.ForceStopParticipation(new ParticipationRequest(ParticipationRequest.Type.Stop, state.Trigger, "ActionSequencer"));
                    }
                }
            }
        }

        public void OnActionAborted(GameObject owner, ClubPenguin.Actions.Action action)
        {
            State state = null;

            if (owner != null)
            {
                EntityId id = owner.GetEntityId();

                if (sequencerDict.ContainsKey(id))
                    state = sequencerDict[id];
            }
            else
            {
                for (int i = 0; i < sequencerList.Count; i++)
                {
                    if (sequencerList[i] != null && sequencerList[i].Actions.Contains(action))
                    {
                        state = sequencerList[i];
                        break;
                    }
                }
            }

            if (state != null)
                abortSequence(state);
        }

        private void abortSequence(State state)
        {
            for (int i = 0; i < state.Actions.Count; i++)
            {
                if (state.Actions[i] != null)
                    UnityEngine.Object.Destroy(state.Actions[i]);
            }

            state.Actions.Clear();

            CompleteAndRemoveSequence(state);
        }

        private void destroyAllActions(State state)
        {
            for (int i = 0; i < state.Actions.Count; i++)
            {
                UnityEngine.Object.Destroy(state.Actions[i]);
            }

            state.Actions.Clear();
        }

        private void sequenceCompleted(State state)
        {
            if (state == null)
                return;

            foreach (GameObject target in state.Targets)
            {
                if (!target.IsDestroyed())
                {
                    PenguinUserControl control = target.GetComponent<PenguinUserControl>();
                    if (control != null)
                        control.enabled = true;

                    Animator anim = target.GetComponent<Animator>();
                    if (anim != null)
                        anim.SetBool(AnimationHashes.Params.Scripted, false);
                }
            }

            if (state.Owner != null && state.Owner.CompareTag("Player"))
            {
                Service.Get<EventDispatcher>().DispatchEvent(
                    default(ActionSequencerEvents.ActionSequenceCompleted)
                );
            }

            if (this.SequenceCompleted != null)
                this.SequenceCompleted(state.Owner);
        }

        public void SetAbortOnUserInput(GameObject owner, bool value)
        {
            if (owner == null)
                return;

            EntityId id = owner.GetEntityId();

            if (!sequencerDict.ContainsKey(id))
                return;

            sequencerDict[id].AbortOnUserInput = value;

            if (!value)
                return;

            foreach (GameObject target in sequencerDict[id].Targets)
            {
                PenguinUserControl control = target.GetComponent<PenguinUserControl>();
                if (control != null)
                    control.enabled = true;
            }
        }

        public void UserInputReceived()
        {
            for (int i = sequencerList.Count - 1; i >= 0; i--)
            {
                if (sequencerList[i].AbortOnUserInput)
                    abortSequence(sequencerList[i]);
            }
        }

        public static GameObject FindActionGraphObject(GameObject trigger)
        {
            if (!trigger.IsDestroyed() && trigger.GetComponent<ClubPenguin.Actions.Action>() != null)
                return trigger.gameObject;

            return null;
        }
    }
}