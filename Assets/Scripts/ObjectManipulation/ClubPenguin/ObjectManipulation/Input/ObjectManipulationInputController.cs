using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.ObjectManipulation.Input
{
	[DisallowMultipleComponent]
	public class ObjectManipulationInputController : MonoBehaviour
	{
		public class Cancelable
		{
			private bool cancelled;

			public void Cancel()
			{
				cancelled = true;
			}

			internal bool isCancelled()
			{
				return cancelled;
			}
		}

		public enum SelectState
		{
			Drag,
			Active
		}

		public Action<ObjectManipulator, Action<bool>> BeforeDragComplete;

		[Tooltip("The layer mask to specify which layer(s) to interact with the input")]
		public LayerMask TargetLayerMask = -1;

		[Tooltip("Minimum time when no selection to ignore swipe instead register a tap")]
		public float MinTimeToMoveInput = 0.1f;

		[Tooltip("Enable so the drag uses an offset instead of snapping to the middle of the finger")]
		public bool NaturalDragFromTouchPosition = true;

		private Transform oldParentOfSelectedObject;

		private AbstractInputInteractionState currentState;

		private ActiveSelectionState activeSelectionState;

		private SwipeScreenInputState swipeScreenInputState;

		private NoSelectionState noSelectionState;

		private DragItemInputInteractionState dragItemInputInteractionState;

		private GameObject currentlySelectedObject;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		public bool SkipOneFrame;

		private GameObject oldObject;

		private Transform oldParent;

		private Dictionary<string, CollisionRuleSetDefinition> decorationInstallationRulesets = new Dictionary<string, CollisionRuleSetDefinition>();

		private Vector2 touchOffset = Vector2.zero;

		private bool inputProcessingPaused = false;

		[Tweakable("Igloo.NaturalDrag", Description = "Enable to use an offset for dragging an item instead of snapping to the center of the object.")]
		public static bool EnableNaturalDrag
		{
			get
			{
				ObjectManipulationInputController objectManipulationInputController = UnityEngine.Object.FindObjectOfType<ObjectManipulationInputController>();
				if (objectManipulationInputController != null)
				{
					return objectManipulationInputController.NaturalDragFromTouchPosition;
				}
				return false;
			}
			set
			{
				ObjectManipulationInputController objectManipulationInputController = UnityEngine.Object.FindObjectOfType<ObjectManipulationInputController>();
				if (objectManipulationInputController != null)
				{
					objectManipulationInputController.NaturalDragFromTouchPosition = value;
				}
			}
		}

		public ObjectManipulator CurrentObjectManipulator
		{
			get
			{
				if (CurrentlySelectedObject == null)
				{
					return null;
				}
				return CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
			}
		}

		public GameObject CurrentlySelectedObject
		{
			get
			{
				return currentlySelectedObject;
			}
			private set
			{
				activeSelectionState.CurrentlySelectedObject = (dragItemInputInteractionState.CurrentlySelectedObject = (currentlySelectedObject = value));
				if (value != null)
				{
					oldParentOfSelectedObject = value.transform.parent;
				}
			}
		}

		private bool InputProcessingPaused
		{
			get
			{
				return inputProcessingPaused;
			}
			set
			{
				inputProcessingPaused = value;
			}
		}

		public AbstractInputInteractionState CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				currentState = value;
				if (this.InteractionStateChanged != null)
				{
					this.InteractionStateChanged.InvokeSafe(currentState.State);
				}
			}
		}

		public Transform Container
		{
			private get;
			set;
		}

		public event Action<InteractionState> InteractionStateChanged;

		public event Action<ManipulatableObject> ObjectSelected;

		public event Action<ObjectManipulator> ObjectDeselected;

		public event Action<Vector2, Cancelable> BeforeDragPosition;

		public event Action<ManipulatableObject> NewObjectAdded;

		public event Action<ManipulatableObject> ObjectBeforeDelete;

		public event Action<Vector2> SwipedScreen;

		public event Action<GameObject, TouchEquivalent> DragStateStationary;

		public event Action<GameObject, TouchEquivalent> DragStateMoved;

		[Invokable("Igloo.SetMinTimeMoveTouch", Description = "Set the minimum time to register a move rather than a tap.")]
		public static void SetTouchMoveMinThreshold(float min)
		{
			ObjectManipulationInputController objectManipulationInputController = UnityEngine.Object.FindObjectOfType<ObjectManipulationInputController>();
			if (objectManipulationInputController != null)
			{
				objectManipulationInputController.MinTimeToMoveInput = min;
			}
		}

		public void Awake()
		{
			activeSelectionState = new ActiveSelectionState();
			dragItemInputInteractionState = new DragItemInputInteractionState();
			swipeScreenInputState = new SwipeScreenInputState();
			noSelectionState = new NoSelectionState();
			activeSelectionState.TouchPhaseEnded += OnActiveSelectionStateTouchPhaseEnded;
			activeSelectionState.TouchPhaseMoved += OnActiveSelectionStateTouchPhaseMoved;
			dragItemInputInteractionState.TouchPhaseEnded += OnDragItemInputInteractionStateTouchPhaseEnded;
			dragItemInputInteractionState.TouchPhaseMoved += OnDragItemInputInteractionStateTouchPhaseMoved;
			dragItemInputInteractionState.TouchPhaseStationary += OnDragItemInputInteractionStateTouchPhaseStationary;
			noSelectionState.TouchPhaseEnded += OnNoSelectionStateTouchPhaseEnded;
			noSelectionState.TouchPhaseMoved += OnNoSelectionStateTouchPhaseMoved;
			swipeScreenInputState.TouchPhaseMoved += OnSwipeScreenInputStateTouchPhaseMoved;
			swipeScreenInputState.TouchPhaseEnded += OnSwipeScreenInputStateTouchPhaseEnded;
			currentState = noSelectionState;
			currentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
			SceneRefs.Set(this);
			decorationInstallationRulesets = Service.Get<IGameData>().Get<Dictionary<string, CollisionRuleSetDefinition>>();
			setupEventListeners();
		}

		private void setupEventListeners()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			eventChannel.AddListener<ObjectManipulationEvents.DeleteSelectedItemEvent>(OnDeleteSelectedItem);
			eventChannel.AddListener<ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent>(OnConfirmPlacementSelectedItemEvent);
			eventChannel.AddListener<ObjectManipulationEvents.BeginDragInventoryItem>(OnBeginDragInventoryItem);
			eventChannel.AddListener<ObjectManipulationEvents.EndDragInventoryItem>(OnEndDragInventoryItem);
			eventChannel.AddListener<ObjectManipulationEvents.ResetSelectedItem>(OnResetSelectedItem);
		}

		public void OnDisable()
		{
			Reset();
		}

		public void OnDestroy()
		{
			activeSelectionState.TouchPhaseEnded -= OnActiveSelectionStateTouchPhaseEnded;
			activeSelectionState.TouchPhaseMoved -= OnActiveSelectionStateTouchPhaseMoved;
			dragItemInputInteractionState.TouchPhaseEnded -= OnDragItemInputInteractionStateTouchPhaseEnded;
			dragItemInputInteractionState.TouchPhaseMoved -= OnDragItemInputInteractionStateTouchPhaseMoved;
			dragItemInputInteractionState.TouchPhaseStationary -= OnDragItemInputInteractionStateTouchPhaseStationary;
			noSelectionState.TouchPhaseEnded -= OnNoSelectionStateTouchPhaseEnded;
			noSelectionState.TouchPhaseMoved -= OnNoSelectionStateTouchPhaseMoved;
			swipeScreenInputState.TouchPhaseMoved -= OnSwipeScreenInputStateTouchPhaseMoved;
			swipeScreenInputState.TouchPhaseEnded -= OnSwipeScreenInputStateTouchPhaseEnded;
			eventChannel.RemoveAllListeners();
			this.InteractionStateChanged = null;
			this.ObjectSelected = null;
			this.ObjectDeselected = null;
			this.BeforeDragPosition = null;
			this.NewObjectAdded = null;
			this.ObjectBeforeDelete = null;
			this.SwipedScreen = null;
			this.DragStateStationary = null;
			this.DragStateMoved = null;
			CoroutineRunner.StopAllForOwner(this);
		}

		public void LateUpdate()
		{
			if (!InputProcessingPaused && !(EventSystem.current.currentSelectedGameObject != null))
			{
				if (SkipOneFrame)
				{
					SkipOneFrame = false;
				}
				else
				{
					currentState.Update();
				}
			}
		}

		private void OnActiveSelectionStateTouchPhaseMoved(GameObject obj, Vector2 touchPosition)
		{
			CurrentState.ExitState();
			ObjectManipulator x = null;
			if (obj != null)
			{
				x = obj.GetComponentInParent<ObjectManipulator>();
				if (NaturalDragFromTouchPosition)
				{
					Vector2 a = Camera.main.WorldToScreenPoint(obj.transform.position);
					touchOffset = a - touchPosition;
				}
			}
			if (x == null)
			{
				CurrentState = swipeScreenInputState;
			}
			else
			{
				dragItemInputInteractionState.CurrentlySelectedObject = obj;
				dragItemInputInteractionState.TouchOffset = touchOffset;
				CurrentState = dragItemInputInteractionState;
			}
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
		}

		public void SelectNewObject(GameObject obj, SelectState setSelectStateTo = SelectState.Drag)
		{
			if (!(obj != null))
			{
				return;
			}
			ResetSelected(false, false);
			CurrentState.ExitState();
			addNewSelectedObject(obj);
			InputProcessingPaused = false;
			if (this.NewObjectAdded != null && CurrentlySelectedObject != null)
			{
				ManipulatableObject component = CurrentlySelectedObject.GetComponent<ManipulatableObject>();
				if (component != null)
				{
					this.NewObjectAdded.InvokeSafe(component);
				}
			}
			switch (setSelectStateTo)
			{
			case SelectState.Active:
				CurrentState = activeSelectionState;
				break;
			case SelectState.Drag:
				CurrentState = dragItemInputInteractionState;
				break;
			}
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
		}

		private void OnActiveSelectionStateTouchPhaseEnded(GameObject obj)
		{
			ResetSelected(false, false);
			CurrentState.ExitState();
			ManipulatableObject manipulatableObject = null;
			if (obj != null)
			{
				manipulatableObject = obj.GetComponentInParent<ManipulatableObject>();
			}
			if (manipulatableObject == null)
			{
				CurrentState = noSelectionState;
			}
			else
			{
				CurrentlySelectedObject = manipulatableObject.gameObject;
				activeSelectionState.CurrentlySelectedObject = manipulatableObject.gameObject;
				if (SelectCurrentSelectedObject(manipulatableObject.gameObject))
				{
					CurrentState = activeSelectionState;
				}
				else
				{
					CurrentState = noSelectionState;
				}
			}
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
		}

		private void OnDragItemInputInteractionStateTouchPhaseEnded(RaycastHit topHit, TouchEquivalent touch)
		{
			CurrentState.ExitState();
			CurrentState = activeSelectionState;
			bool flag = false;
			if (CurrentlySelectedObject != null)
			{
				if (topHit.Equals(default(RaycastHit)))
				{
					CurrentlySelectedObject.transform.position = new Vector3(0f, 1000f, 0f);
					DeleteSelectedItem(true);
					InputProcessingPaused = false;
					CurrentState = noSelectionState;
					CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
					return;
				}
				PositionDragItem(topHit, touch);
				ObjectManipulator i = CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
				if (i != null && i.IsAllowed)
				{
					if (BeforeDragComplete != null)
					{
						flag = true;
						BeforeDragComplete.InvokeSafe(i, delegate(bool success)
						{
							if (success)
							{
								storeOldObject();
								CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
							}
							else
							{
								i.BaseLocationIsValid = false;
							}
						});
					}
					else
					{
						storeOldObject();
					}
				}
			}
			if (!flag)
			{
				CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
			}
		}

		private void OnDragItemInputInteractionStateTouchPhaseMoved(RaycastHit topHit, TouchEquivalent touch)
		{
			PositionDragItem(topHit, touch);
			if (this.DragStateMoved != null)
			{
				ObjectManipulator componentInParent = CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
				this.DragStateMoved(componentInParent.gameObject, touch);
			}
		}

		private void OnDragItemInputInteractionStateTouchPhaseStationary(RaycastHit topHit, TouchEquivalent touch)
		{
			PositionDragItem(topHit, touch);
			if (this.DragStateStationary != null)
			{
				ObjectManipulator componentInParent = CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
				this.DragStateStationary(componentInParent.gameObject, touch);
			}
		}

		private void PositionDragItem(RaycastHit topHit, TouchEquivalent touch)
		{
			if (this.BeforeDragPosition != null)
			{
				Cancelable cancelable = new Cancelable();
				this.BeforeDragPosition.InvokeSafe(touch.position, cancelable);
				if (cancelable.isCancelled())
				{
					return;
				}
			}
			ObjectManipulator component = CurrentlySelectedObject.GetComponent<ObjectManipulator>();
			CollidableObject component2 = CurrentlySelectedObject.GetComponent<CollidableObject>();
			if (!(component != null))
			{
				return;
			}
			bool flag = component.transform != Container;
			CollidableObject componentInParent = topHit.transform.gameObject.GetComponentInParent<CollidableObject>();
			if (componentInParent != null)
			{
				switch (GetCollisionRule(componentInParent))
				{
				case CollisionRuleResult.StackXNormal:
					PositionDragItemForStacking(topHit, component, component2, componentInParent, Vector3.right);
					break;
				case CollisionRuleResult.Stack:
					PositionDragItemForStacking(topHit, component, component2, componentInParent, Vector3.up);
					break;
				case CollisionRuleResult.NotAllowed:
				case CollisionRuleResult.Intersect:
				{
					for (int i = 0; i < dragItemInputInteractionState.RaycastHits.Length; i++)
					{
						componentInParent = dragItemInputInteractionState.RaycastHits[i].transform.gameObject.GetComponentInParent<CollidableObject>();
						if (componentInParent == null)
						{
							setPositionAtPointOnTopHit(dragItemInputInteractionState.RaycastHits[i], component);
							break;
						}
						if (componentInParent.transform.IsChildOf(component2.transform))
						{
							continue;
						}
						switch (GetCollisionRule(componentInParent))
						{
						case CollisionRuleResult.Stack:
							PositionDragItemForStacking(dragItemInputInteractionState.RaycastHits[i], component, component2, componentInParent, Vector3.up);
							break;
						case CollisionRuleResult.StackXNormal:
							PositionDragItemForStacking(dragItemInputInteractionState.RaycastHits[i], component, component2, componentInParent, Vector3.right);
							break;
						default:
							continue;
						}
						break;
					}
					break;
				}
				default:
					setPositionAtPointOnTopHit(topHit, component);
					break;
				}
			}
			else
			{
				setPositionAtPointOnTopHit(topHit, component);
			}
			if (NaturalDragFromTouchPosition && !flag)
			{
				Vector2 a = Camera.main.WorldToScreenPoint(component.transform.position);
				dragItemInputInteractionState.TouchOffset = (touchOffset = a - touch.position);
			}
		}

		private void PositionDragItemForStacking(RaycastHit topHit, ObjectManipulator selectedManipulator, CollidableObject selectedCollidableObject, CollidableObject hitCollidableObject, Vector3 localAlignment)
		{
			List<CollidableObject> list = new List<CollidableObject>();
			CollidableObject[] componentsInChildren = hitCollidableObject.gameObject.GetComponentsInChildren<CollidableObject>();
			foreach (CollidableObject collidableObject in componentsInChildren)
			{
				if (collidableObject != selectedCollidableObject && !collidableObject.transform.IsChildOf(selectedCollidableObject.transform))
				{
					list.Add(collidableObject);
				}
			}
			CollidableObject collidableObject2 = null;
			Vector3 vector = Vector3.zero;
			for (int j = 0; j < list.Count; j++)
			{
				CollidableObject collidableObject3 = list[j];
				Vector3 a = collidableObject3.transform.InverseTransformPoint(topHit.point);
				Vector3 b = Vector3.Scale(a, Vector3.one - localAlignment);
				Vector3 localBoundsSize = collidableObject3.GetLocalBoundsSize();
				Vector3 a2 = Vector3.Scale(localBoundsSize, localAlignment);
				a2.x = Mathf.Abs(a2.x);
				a2.y = Mathf.Abs(a2.y);
				a2.z = Mathf.Abs(a2.z);
				vector = collidableObject3.transform.TransformPoint(a2 + b);
				if (j == list.Count - 1)
				{
					collidableObject2 = collidableObject3;
					break;
				}
				bool flag = true;
				for (int k = j + 1; k < list.Count; k++)
				{
					Bounds bounds = new Bounds(vector, selectedCollidableObject.GetBounds().size);
					CollisionRuleResult collisionRule = GetCollisionRule(list[k]);
					if (collisionRule == CollisionRuleResult.Intersect || collisionRule == CollisionRuleResult.NotAllowed)
					{
						flag = true;
						break;
					}
					if (list[k].GetBounds().Intersects(bounds))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					collidableObject2 = collidableObject3;
					break;
				}
			}
			if (collidableObject2 != null)
			{
				selectedManipulator.SetPosition(vector);
				ManipulatableObject componentInParent = collidableObject2.GetComponentInParent<ManipulatableObject>();
				if (componentInParent != null)
				{
					selectedManipulator.SetParent(componentInParent.transform);
				}
				else
				{
					selectedManipulator.SetParent(Container);
				}
				selectedManipulator.AlignWith(collidableObject2.transform.TransformDirection(localAlignment), collidableObject2.transform);
			}
		}

		private void setPositionAtPointOnTopHit(RaycastHit topHit, ObjectManipulator selectedManipulator)
		{
			selectedManipulator.SetParent(Container);
			selectedManipulator.SetPosition(topHit.point);
			if (topHit.transform.gameObject.layer == LayerMask.NameToLayer("TerrainBarrier"))
			{
				selectedManipulator.AlignWith(topHit.normal, topHit.transform);
			}
		}

		private void OnNoSelectionStateTouchPhaseMoved(Vector2 deltaTouchPosition)
		{
			CurrentState.ExitState();
			CurrentState = swipeScreenInputState;
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
			swipeScreenInputState.ProcessMove(deltaTouchPosition);
		}

		private void OnNoSelectionStateTouchPhaseEnded(GameObject obj, Vector2 touchPosition)
		{
			ResetSelected(false, false);
			CurrentState.ExitState();
			touchOffset = Vector2.zero;
			ManipulatableObject manipulatableObject = null;
			if (obj != null)
			{
				manipulatableObject = obj.GetComponentInParent<ManipulatableObject>();
				if (NaturalDragFromTouchPosition)
				{
					Vector2 a = Camera.main.WorldToScreenPoint(obj.transform.position);
					touchOffset = a - touchPosition;
				}
			}
			if (manipulatableObject != null)
			{
				obj = manipulatableObject.gameObject;
				if (SelectCurrentSelectedObject(obj))
				{
					activeSelectionState.CurrentlySelectedObject = obj;
					CurrentState = activeSelectionState;
				}
			}
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
		}

		private void OnSwipeScreenInputStateTouchPhaseEnded()
		{
			CurrentState.ExitState();
			if (CurrentlySelectedObject == null || CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>() == null)
			{
				CurrentState = noSelectionState;
			}
			else
			{
				CurrentState = activeSelectionState;
			}
			CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
		}

		private void OnSwipeScreenInputStateTouchPhaseMoved(Vector2 deltaPosition)
		{
			if (this.SwipedScreen != null)
			{
				this.SwipedScreen(deltaPosition);
			}
		}

		public CollisionRuleResult GetCollisionRule(CollidableObject installedDecoration)
		{
			if (CurrentlySelectedObject != null)
			{
				CollidableObject component = CurrentlySelectedObject.GetComponent<CollidableObject>();
				if (component == null || component.CollisionRuleSet == null || installedDecoration == null || component.CollisionRuleSet == null)
				{
					Log.LogError(this, "Problem: Installed decoration or selected object is not tracked properly and the placement rule cannot be found. Defaulting to stacking");
					return CollisionRuleResult.Stack;
				}
				if (decorationInstallationRulesets.ContainsKey(installedDecoration.CollisionRuleSet.Id))
				{
					CollisionRuleSetDefinition collisionRuleSetDefinition = decorationInstallationRulesets[component.CollisionRuleSet.Id];
					CollisionRuleDefinition[] installedItemRules = collisionRuleSetDefinition.InstalledItemRules;
					foreach (CollisionRuleDefinition collisionRuleDefinition in installedItemRules)
					{
						if (collisionRuleDefinition.InstalledItem.Id == installedDecoration.CollisionRuleSet.Id)
						{
							return collisionRuleDefinition.Result;
						}
					}
					throw new InvalidOperationException(string.Format("No rule was found for the combination {0} and {1} on of the selected object {0} and the installed object", component.CollisionRuleSet.Id, installedDecoration.CollisionRuleSet.Id, CurrentlySelectedObject, installedDecoration.gameObject));
				}
				Log.LogErrorFormatted(this, "Unknown rule: {0}, found for the selected item {1}. Defaulting to stacking", installedDecoration.CollisionRuleSet.Id, CurrentlySelectedObject);
				return CollisionRuleResult.Stack;
			}
			throw new InvalidOperationException("Selected item is null");
		}

		public bool IsSelectedObjectAllowedInCurrentPosition()
		{
			bool result = true;
			foreach (Collider currentCollider in CurrentObjectManipulator.CurrentColliders)
			{
				CollidableObject componentInParent = currentCollider.gameObject.GetComponentInParent<CollidableObject>();
				if (componentInParent != null && GetCollisionRule(componentInParent) == CollisionRuleResult.NotAllowed)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private void addNewSelectedObject(GameObject obj)
		{
			touchOffset = Vector2.zero;
			CurrentlySelectedObject = obj;
			if (!(CurrentlySelectedObject != null))
			{
				return;
			}
			ManipulatableObject component = CurrentlySelectedObject.GetComponent<ManipulatableObject>();
			if (component != null)
			{
				if (oldObject != null)
				{
					UnityEngine.Object.Destroy(oldObject);
				}
				oldObject = null;
				this.ObjectSelected.InvokeSafe(component);
			}
		}

		private bool SelectCurrentSelectedObject(GameObject obj)
		{
			CurrentlySelectedObject = obj;
			if (CurrentlySelectedObject != null)
			{
				ManipulatableObject componentInParent = CurrentlySelectedObject.GetComponentInParent<ManipulatableObject>();
				if (componentInParent != null)
				{
					storeOldObject();
					if (this.ObjectSelected != null)
					{
						this.ObjectSelected.InvokeSafe(componentInParent);
						if (componentInParent.GetComponent<ObjectManipulator>() != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void storeOldObject()
		{
			if (oldObject != null)
			{
				UnityEngine.Object.Destroy(oldObject);
			}
			if (CurrentlySelectedObject != null)
			{
				oldObject = UnityEngine.Object.Instantiate(CurrentlySelectedObject, CurrentlySelectedObject.transform.parent, true);
				oldObject.SetActive(false);
				oldObject.name = CurrentlySelectedObject.name;
				oldParent = CurrentlySelectedObject.transform.parent;
				oldObject.transform.parent = null;
			}
		}

		private static void copyTransform(Transform from, Transform to)
		{
			to.position = from.position;
			to.rotation = from.rotation;
			to.localScale = from.localScale;
		}

		private bool restoreOldObject()
		{
			if (oldObject != null)
			{
				copyTransform(oldObject.transform, CurrentlySelectedObject.transform);
				ManipulatableObject component = CurrentlySelectedObject.GetComponent<ManipulatableObject>();
				component.SetParent(oldParent);
				return true;
			}
			return false;
		}

		private bool ResetSelected(bool objectBeingDeleted, bool deleteChildren)
		{
			bool flag = false;
			if (CurrentlySelectedObject != null)
			{
				ManipulatableObject component = CurrentlySelectedObject.GetComponent<ManipulatableObject>();
				ObjectManipulator componentInParent = CurrentlySelectedObject.GetComponentInParent<ObjectManipulator>();
				if (componentInParent != null)
				{
					if (componentInParent.IsAllowed)
					{
						Transform parent = CurrentlySelectedObject.transform.parent;
						CurrentlySelectedObject.transform.SetParent(oldParentOfSelectedObject, true);
						component.SetParent(parent);
						componentInParent.WasReparented = true;
					}
					else if (!restoreOldObject())
					{
						flag = true;
					}
					if (!objectBeingDeleted && this.ObjectDeselected != null)
					{
						this.ObjectDeselected.InvokeSafe(componentInParent);
					}
					if (flag)
					{
						component.RemoveObject(deleteChildren);
					}
				}
			}
			CurrentlySelectedObject = null;
			return flag;
		}

		private bool OnResetSelectedItem(ObjectManipulationEvents.ResetSelectedItem evt)
		{
			Reset();
			return false;
		}

		private bool OnDeleteSelectedItem(ObjectManipulationEvents.DeleteSelectedItemEvent evt)
		{
			DeleteSelectedItem(evt.DeleteChildren);
			Reset();
			CoroutineRunner.Start(pauseAndResumeInputProcessing(), this, "pauseAndResumeInputProcessing");
			return false;
		}

		private bool OnConfirmPlacementSelectedItemEvent(ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent evt)
		{
			Reset();
			CoroutineRunner.Start(pauseAndResumeInputProcessing(), this, "pauseAndResumeInputProcessing");
			return false;
		}

		private bool OnBeginDragInventoryItem(ObjectManipulationEvents.BeginDragInventoryItem evt)
		{
			Reset();
			InputProcessingPaused = true;
			return false;
		}

		private bool OnEndDragInventoryItem(ObjectManipulationEvents.EndDragInventoryItem evt)
		{
			InputProcessingPaused = false;
			return false;
		}

		private IEnumerator pauseAndResumeInputProcessing()
		{
			InputProcessingPaused = true;
			yield return new WaitForSeconds(0.1f);
			if (!base.gameObject.IsDestroyed())
			{
				InputProcessingPaused = false;
			}
		}

		public void SetPausedState(bool isPaused)
		{
			inputProcessingPaused = isPaused;
		}

		private void DeleteSelectedItem(bool deleteChildren)
		{
			GameObject gameObject = CurrentlySelectedObject;
			UnityEngine.Object.Destroy(oldObject);
			oldObject = null;
			if (!deleteChildren)
			{
				ManipulatableObject component = CurrentlySelectedObject.GetComponent<ManipulatableObject>();
				if (component != null)
				{
					this.ObjectBeforeDelete.InvokeSafe(component);
				}
			}
			bool flag = ResetSelected(true, deleteChildren);
			if (gameObject != null && !flag)
			{
				ManipulatableObject componentInParent = gameObject.GetComponentInParent<ManipulatableObject>();
				if (componentInParent != null)
				{
					componentInParent.RemoveObject(deleteChildren);
				}
			}
		}

		public void Reset()
		{
			if (CurrentState != null)
			{
				ResetSelected(false, false);
				CurrentState.ExitState();
				CurrentState = noSelectionState;
				CurrentState.EnterState(TargetLayerMask, MinTimeToMoveInput);
			}
		}

		public void Abort()
		{
			CurrentlySelectedObject = null;
			Reset();
		}
	}
}
