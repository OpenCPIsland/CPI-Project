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
			public readonly int OwnerInstanceId;

			public GameObject Owner;

			public GameObject Trigger;

			public List<ClubPenguin.Actions.Action> Actions;

			public HashSet<GameObject> Targets;

			public bool AbortOnUserInput;

			public State(GameObject _owner, GameObject _trigger)
			{
				Owner = _owner;
				OwnerInstanceId = _owner.GetInstanceID();
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

		private Dictionary<int, State> sequencerDict;

		private List<State> sequencerList;

		public event Action<GameObject> SequenceCompleted;

		private void Awake()
		{
			sequencerDict = new Dictionary<int, State>();
			sequencerList = new List<State>();
			SceneRefs.SetActionSequencer(this);
			ActionSequencer[] array = UnityEngine.Object.FindObjectsOfType<ActionSequencer>();
			if (array.Length > 1)
			{
				Log.LogError(this, "There are " + array.Length + " instances of ActionSequencer. Only 1 instance should exist at any given time.");
			}
		}

		public void OnDestroy()
		{
			this.SequenceCompleted = null;
		}

		public bool StartSequence(GameObject owner, GameObject trigger)
		{
			bool result = false;
			if (owner == null)
			{
				Log.LogErrorFormatted(this, "owner is null when starting a sequence for trigger {0}", trigger);
				return result;
			}
			int instanceID = owner.GetInstanceID();
			if (sequencerDict.ContainsKey(instanceID))
			{
				State state = sequencerDict[instanceID];
				if (!state.Trigger.IsDestroyed())
				{
				}
			}
			else
			{
				ClubPenguin.Actions.Action[] components = trigger.GetComponents<ClubPenguin.Actions.Action>();
				if (components.Length > 0)
				{
					SharedActionGraphState sharedActionGraphState = trigger.GetComponent<SharedActionGraphState>();
					if (sharedActionGraphState == null)
					{
						sharedActionGraphState = trigger.AddComponent<SharedActionGraphState>();
					}
					if (sharedActionGraphState.MaxInteractors > -1 && sharedActionGraphState.Interactors.Count >= sharedActionGraphState.MaxInteractors)
					{
						result = false;
					}
					else
					{
						State state2 = new State(owner, trigger);
						int num = components.Length;
						state2.Actions.Capacity = num;
						for (int i = 0; i < num; i++)
						{
							GameObject gameObject = components[i].GetTarget();
							if (gameObject == null)
							{
								gameObject = owner;
							}
							if (!gameObject.IsDestroyed())
							{
								if (state2.Targets.Add(gameObject))
								{
									sequenceStarted(state2);
								}
								ClubPenguin.Actions.Action action = components[i].AddToGameObject(gameObject);
								action.Owner = owner;
								state2.Actions.Add(action);
							}
						}
						addSequencer(owner, state2);
						enableRootActions(state2);
						result = true;
						if (owner.CompareTag("Player"))
						{
							Service.Get<EventDispatcher>().DispatchEvent(new ActionSequencerEvents.ActionSequenceStarted(trigger));
						}
					}
				}
			}
			return result;
		}

		public SharedActionGraphState GetSharedActionGraphState(GameObject trigger)
		{
			return trigger.GetComponent<SharedActionGraphState>();
		}

		public GameObject GetTrigger(GameObject owner)
		{
			if (owner == null)
			{
				Log.LogError(this, "Owner is null when trying to get a trigger");
				return null;
			}
			int instanceID = owner.GetInstanceID();
			if (sequencerDict.ContainsKey(instanceID))
			{
				return sequencerDict[instanceID].Trigger;
			}
			return null;
		}

		private void addSequencer(GameObject owner, State state)
		{
			if (owner == null)
			{
				Log.LogErrorFormatted(this, "Owner is null when trying to add a state to the sequencer. State: {0}", state);
				return;
			}
			int instanceID = owner.GetInstanceID();
			if (state.Trigger != null)
			{
				SharedActionGraphState component = state.Trigger.GetComponent<SharedActionGraphState>();
				if (component != null)
				{
					component.Interactors.Add(owner);
				}
			}
			sequencerDict.Add(instanceID, state);
			sequencerList.Add(state);
		}

		private void removeSequencer(State state)
		{
			if (state.Owner != null && state.Trigger != null)
			{
				SharedActionGraphState component = state.Trigger.GetComponent<SharedActionGraphState>();
				if (component != null)
				{
					component.Interactors.Remove(state.Owner);
				}
			}
			sequencerDict.Remove(state.OwnerInstanceId);
			sequencerList.Remove(state);
		}

		private void enableRootActions(State state)
		{
			int count = state.Actions.Count;
			for (int i = 0; i < count; i++)
			{
				ClubPenguin.Actions.Action action = state.Actions[i];
				if (action.ParentId == -1 && action.ParentIdOnFalse == -1)
				{
					action.enabled = true;
				}
			}
		}

		private bool enableDependentActions(State state, ClubPenguin.Actions.Action completedAction, object userData = null, bool conditionBranchValue = true)
		{
			int id = completedAction.Id;
			int count = state.Actions.Count;
			bool result = true;
			for (int i = 0; i < count; i++)
			{
				ClubPenguin.Actions.Action action = state.Actions[i];
				if (action == null)
				{
					result = false;
					break;
				}
				int num = conditionBranchValue ? action.ParentId : action.ParentIdOnFalse;
				if (num == id)
				{
					action.IncomingUserData = userData;
					action.enabled = true;
				}
			}
			return result;
		}

		private void triggerInterrupts(State state, ClubPenguin.Actions.Action completedAction)
		{
			int id = completedAction.Id;
			int count = state.Actions.Count;
			ClubPenguin.Actions.Action[] array = new ClubPenguin.Actions.Action[count];
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				ClubPenguin.Actions.Action action = state.Actions[i];
				if (action.InterruptedBy == id && !action.Complete)
				{
					array[num] = action;
					num++;
				}
			}
			for (int i = 0; i < num; i++)
			{
				array[i].Completed();
			}
		}

		private void sequenceStarted(State state)
		{
		}

		private void destroyAllActions(State state)
		{
			int count = state.Actions.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.Destroy(state.Actions[i]);
			}
			state.Actions.Clear();
		}

		public void ActionCompleted(GameObject owner, ClubPenguin.Actions.Action action, object userData = null, bool conditionBranchValue = true)
		{
			if (owner == null)
			{
				Log.LogErrorFormatted(this, "Owner is null when an action is complete. Action = {0}, UserData = {1}", action, userData);
				OnActionAborted(owner, action);
				return;
			}
			int instanceID = owner.GetInstanceID();
			if (!sequencerDict.ContainsKey(instanceID))
			{
				return;
			}
			State state = sequencerDict[instanceID];
			bool flag = false;
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
						bool flag2 = false;
						int count = state.Actions.Count;
						for (int i = 0; i < count; i++)
						{
							if (state.Actions[i].enabled)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							destroyAllActions(state);
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				abortSequence(state);
			}
			else if (state.Actions.Count == 0)
			{
				CompleteAndRemoveSequence(state);
			}
		}

		private void CompleteAndRemoveSequence(State state)
		{
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
			finally
			{
				removeSequencer(state);
			}
		}

		public void OnActionAborted(GameObject owner, ClubPenguin.Actions.Action action)
		{
			State state = null;
			if (owner != null)
			{
				int instanceID = owner.GetInstanceID();
				if (sequencerDict.ContainsKey(instanceID))
				{
					state = sequencerDict[instanceID];
				}
			}
			else
			{
				for (int i = 0; i < sequencerList.Count; i++)
				{
					State state2 = sequencerList[i];
					if (state2 != null && state2.Actions.Contains(action))
					{
						state = state2;
						break;
					}
				}
			}
			if (state != null)
			{
				abortSequence(state);
			}
		}

		private void abortSequence(State state)
		{
			int count = state.Actions.Count;
			for (int i = 0; i < count; i++)
			{
				if (state.Actions[i] != null)
				{
					UnityEngine.Object.Destroy(state.Actions[i]);
				}
			}
			state.Actions.Clear();
			CompleteAndRemoveSequence(state);
		}

		private void sequenceCompleted(State state)
		{
			if (state != null && !state.Equals(null))
			{
				foreach (GameObject target in state.Targets)
				{
					if (!target.IsDestroyed())
					{
						PenguinUserControl component = target.GetComponent<PenguinUserControl>();
						if (component != null)
						{
							component.enabled = true;
						}
						Animator component2 = target.GetComponent<Animator>();
						if (component2 != null)
						{
							component2.SetBool(AnimationHashes.Params.Scripted, false);
						}
					}
				}
				if (state.Owner != null && state.Owner.CompareTag("Player"))
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(ActionSequencerEvents.ActionSequenceCompleted));
				}
				if (state.Owner != null && !state.Owner.Equals(null) && this.SequenceCompleted != null && this.SequenceCompleted.Target != null && !this.SequenceCompleted.Target.Equals(null))
				{
					this.SequenceCompleted(state.Owner);
				}
			}
		}

		public void SetAbortOnUserInput(GameObject owner, bool value)
		{
			if (owner == null)
			{
				Log.LogError(this, "Owner is null when trying to abort user input.");
				return;
			}
			int instanceID = owner.GetInstanceID();
			if (!sequencerDict.ContainsKey(instanceID))
			{
				return;
			}
			State state = sequencerDict[instanceID];
			if (state != null)
			{
				state.AbortOnUserInput = value;
				if (value)
				{
					foreach (GameObject target in state.Targets)
					{
						PenguinUserControl component = target.GetComponent<PenguinUserControl>();
						if (component != null)
						{
							component.enabled = true;
						}
					}
				}
			}
		}

		public void UserInputReceived()
		{
			int num = sequencerList.Count;
			for (int i = 0; i < num; i++)
			{
				State state = sequencerList[i];
				if (state.AbortOnUserInput)
				{
					abortSequence(state);
					num--;
				}
			}
		}

		public static GameObject FindActionGraphObject(GameObject trigger)
		{
			if (!trigger.IsDestroyed() && trigger.GetComponent<ClubPenguin.Actions.Action>() != null)
			{
				return trigger.gameObject;
			}
			return null;
		}
	}
}
