#define UNITY_ASSERTIONS
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.Tutorial;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Igloo
{
	internal class IglooEditCameraTargetController : MonoBehaviour
	{
		[Tooltip("Center the camera on the target when the item is selected.")]
		public bool CenterCameraOnSelect = true;

		[Tooltip("The target gameobject the camera will follow when in igloo edit mode.")]
		public GameObject EditCameraTarget;

		[Tooltip("The volume the edit camera target is allowed to move in")]
		public Collider BoundsForCameraTarget;

		public CameraController PreviewCameraControllerRail;

		public CameraController EditCameraControllerRail;

		public CameraController StructureCameraControllerRail;

		public CameraController LightingCameraControllerRail;

		[Tooltip("Speed of the swipe screen gesture")]
		public float SwipeSpeedModifier = 0.5f;

		[Tooltip("Min Target distance of camera target when new item added")]
		[Range(1f, 10f)]
		public float MinCameraTargetDistanceOnNewItem = 1f;

		[Range(0.01f, 1f)]
		[Space]
		[Tooltip("The max percentage of the zoom affect on camera speed.")]
		public float MaxZoomAffect = 1f;

		[Tooltip("The min percentage of the zoom affect on camera speed.")]
		[Range(0.01f, 1f)]
		public float MinZoomAffect = 0.25f;

		private Bounds boundsForCameraTarget;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		private CameraController currentRailCameraController;

		private CameraController previousToOverrideRail;

		private bool wasCameraTargetReset;

		private bool listenersSetup = false;

		private float zoomFactor = 0.5f;

		private SceneStateData sceneStateData;

		private DataEventListener sceneStateDataListener;

		private IglooCameraOverrideStateData iglooCameraOverrideData;

		private DataEventListener iglooCameraLotStateListener;

		private ManipulatableObject newlyAddedObject;

		private ObjectManipulationInputController _objectManipulationInputController;

		private Director _director;

		private ScreenEdgeBandCalculator _screenEdgeBandCalculator;

		[Tweakable("Igloo.Camera.EditCameraTarget", Description = "Show/hide edit camera target")]
		public static bool ShowEditCameraTarget
		{
			get
			{
				IglooEditCameraTargetController iglooEditCameraTargetController = Object.FindObjectOfType<IglooEditCameraTargetController>();
				if (iglooEditCameraTargetController != null)
				{
					return iglooEditCameraTargetController.EditCameraTarget.activeInHierarchy;
				}
				return false;
			}
			set
			{
				IglooEditCameraTargetController iglooEditCameraTargetController = Object.FindObjectOfType<IglooEditCameraTargetController>();
				if (iglooEditCameraTargetController != null)
				{
					iglooEditCameraTargetController.EditCameraTarget.SetActive(value);
				}
			}
		}

		[Tweakable("Igloo.Camera.CenterCameraOnSelect", Description = "Centre camera on select")]
		public static bool CenterCameraOnSelectItem
		{
			get
			{
				IglooEditCameraTargetController iglooEditCameraTargetController = Object.FindObjectOfType<IglooEditCameraTargetController>();
				if (iglooEditCameraTargetController != null)
				{
					return iglooEditCameraTargetController.CenterCameraOnSelect;
				}
				return false;
			}
			set
			{
				IglooEditCameraTargetController iglooEditCameraTargetController = Object.FindObjectOfType<IglooEditCameraTargetController>();
				if (iglooEditCameraTargetController != null)
				{
					iglooEditCameraTargetController.CenterCameraOnSelect = value;
				}
			}
		}

		private ObjectManipulationInputController objectManipulationInputController
		{
			get
			{
				if (_objectManipulationInputController == null && SceneRefs.IsSet<ObjectManipulationInputController>())
				{
					_objectManipulationInputController = SceneRefs.Get<ObjectManipulationInputController>();
				}
				return _objectManipulationInputController;
			}
		}

		private Director director
		{
			get
			{
				if (_director == null && SceneRefs.IsSet<Director>())
				{
					_director = SceneRefs.Get<Director>();
				}
				return _director;
			}
		}

		private ScreenEdgeBandCalculator screenEdgeBandCalculator
		{
			get
			{
				if (_screenEdgeBandCalculator == null)
				{
					_screenEdgeBandCalculator = Object.FindObjectOfType<ScreenEdgeBandCalculator>();
				}
				return _screenEdgeBandCalculator;
			}
		}

		private void Awake()
		{
			Assert.IsNotNull(EditCameraTarget, "EditCameraTarget required.");
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<InputEvents.ZoomEvent>(onInputZoom);
			boundsForCameraTarget = BoundsForCameraTarget.bounds;
			BoundsForCameraTarget.enabled = false;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			iglooCameraLotStateListener = dataEntityCollection.Whenever<IglooCameraOverrideStateData>("ActiveSceneData", onOverrideStateDataAdded, onOverrideStateDataRemoved);
			sceneStateDataListener = dataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneDataAdded);
			wasCameraTargetReset = false;
		}

		private void onSceneDataAdded(SceneStateData stateData)
		{
			sceneStateData = stateData;
			sceneStateData.OnStateChanged += onSceneStateDataChanged;
			onSceneStateDataChanged(sceneStateData.State);
		}

		private void onOverrideStateDataAdded(IglooCameraOverrideStateData overrideData)
		{
			iglooCameraOverrideData = overrideData;
			iglooCameraOverrideData.OverrideStateUpdated += onOverrideStateUpdated;
			iglooCameraOverrideData.OverrideToLightingRailUpdated += onOverrideToLightingRailUpdated;
			onOverrideStateUpdated(iglooCameraOverrideData.OverrideState);
		}

		private void onOverrideStateDataRemoved(IglooCameraOverrideStateData overrideData)
		{
			iglooCameraOverrideData.OverrideStateUpdated -= onOverrideStateUpdated;
			iglooCameraOverrideData.OverrideToLightingRailUpdated -= onOverrideToLightingRailUpdated;
			iglooCameraOverrideData = null;
			if (sceneStateData != null)
			{
				onSceneStateDataChanged(sceneStateData.State);
			}
		}

		private void Update()
		{
			if (sceneIsLoaded())
			{
				if (iglooCameraOverrideData == null)
				{
					changeCameraTarget(sceneStateData.State);
				}
				else
				{
					changeCameraTarget(iglooCameraOverrideData.OverrideState);
				}
				base.enabled = false;
			}
			else if (sceneStateData != null && sceneStateData.State == SceneStateData.SceneState.Play)
			{
				base.enabled = false;
			}
		}

		private void OnDestroy()
		{
			eventDispatcher.RemoveListener<InputEvents.ZoomEvent>(onInputZoom);
			removeListeners();
			if (sceneStateData != null)
			{
				sceneStateData.OnStateChanged -= onSceneStateDataChanged;
			}
			if (sceneStateDataListener != null)
			{
				sceneStateDataListener.StopListening();
			}
			if (iglooCameraLotStateListener != null)
			{
				iglooCameraLotStateListener.StopListening();
			}
		}

		private void setupListeners()
		{
			if (!listenersSetup)
			{
				objectManipulationInputController.SwipedScreen += onObjectManipulationInputControllerSwipedScreen;
				objectManipulationInputController.DragStateMoved += onObjectManipulationInputControllerDragStateMoved;
				objectManipulationInputController.DragStateStationary += OnObjectManipulationInputControllerDragStateStationary;
				objectManipulationInputController.NewObjectAdded += OnObjectManipulationInputControllerNewObjectAdded;
				objectManipulationInputController.InteractionStateChanged += OnObjectManipulationInputControllerInteractionStateChanged;
				objectManipulationInputController.ObjectDeselected += onObjectManipulationInputControllerObjectDeselected;
				objectManipulationInputController.ObjectBeforeDelete += onObjectManipulationInputControllerObjectBeforeDelete;
				listenersSetup = true;
			}
		}

		private void removeListeners()
		{
			if (listenersSetup)
			{
				objectManipulationInputController.SwipedScreen -= onObjectManipulationInputControllerSwipedScreen;
				objectManipulationInputController.DragStateMoved -= onObjectManipulationInputControllerDragStateMoved;
				objectManipulationInputController.DragStateStationary -= OnObjectManipulationInputControllerDragStateStationary;
				objectManipulationInputController.NewObjectAdded -= OnObjectManipulationInputControllerNewObjectAdded;
				objectManipulationInputController.InteractionStateChanged -= OnObjectManipulationInputControllerInteractionStateChanged;
				objectManipulationInputController.ObjectDeselected -= onObjectManipulationInputControllerObjectDeselected;
				objectManipulationInputController.ObjectBeforeDelete -= onObjectManipulationInputControllerObjectBeforeDelete;
				listenersSetup = false;
			}
		}

		private bool onInputZoom(InputEvents.ZoomEvent evt)
		{
			zoomFactor = Mathf.Clamp(evt.Factor, MinZoomAffect, MaxZoomAffect);
			return false;
		}

		private void onSceneStateDataChanged(SceneStateData.SceneState state)
		{
			if (iglooCameraOverrideData == null)
			{
				changeCameraTarget(state);
			}
		}

		private void onOverrideToLightingRailUpdated(bool overrideLightRail)
		{
			if (overrideLightRail)
			{
				previousToOverrideRail = currentRailCameraController;
				updateRail(LightingCameraControllerRail);
			}
			else if (previousToOverrideRail != null)
			{
				updateRail(previousToOverrideRail);
				previousToOverrideRail = null;
			}
		}

		private void onOverrideStateUpdated(SceneStateData.SceneState overriddenState)
		{
			if (iglooCameraOverrideData.UpdateTargetAndRail)
			{
				changeCameraTarget(overriddenState);
			}
			else
			{
				changeCameraRail(overriddenState);
			}
		}

		private void onObjectManipulationInputControllerSwipedScreen(Vector2 deltaPosition)
		{
			if (EditCameraTarget != null)
			{
				Vector3 position = EditCameraTarget.transform.position;
				float t = Time.deltaTime * (SwipeSpeedModifier * zoomFactor);
				Vector3 b = EditCameraTarget.transform.position + new Vector3(deltaPosition.x, 0f, deltaPosition.y);
				Vector3 adjustedPosition = Vector3.Lerp(position, b, t);
				updateEditCameraTargetPosition(adjustedPosition);
			}
		}

		private void onObjectManipulationInputControllerDragStateMoved(GameObject obj, TouchEquivalent touch)
		{
			int num = screenEdgeBandCalculator.CalculateBandNumber(touch.position);
			followDragItemWithEditCameraTarget(obj, touch.position, num);
		}

		private void OnObjectManipulationInputControllerDragStateStationary(GameObject obj, TouchEquivalent touch)
		{
			int num = screenEdgeBandCalculator.CalculateBandNumber(touch.position);
			followDragItemWithEditCameraTarget(obj, touch.position, num);
		}

		private void OnObjectManipulationInputControllerNewObjectAdded(ManipulatableObject obj)
		{
			newlyAddedObject = obj;
		}

		private void OnObjectManipulationInputControllerInteractionStateChanged(InteractionState state)
		{
			switch (state)
			{
			case InteractionState.DragItem:
				break;
			case InteractionState.NoSelectedItem:
			case InteractionState.ActiveSelectedItem:
				if (newlyAddedObject != null)
				{
					calculateTargetPositionForSelectedObject(newlyAddedObject);
					newlyAddedObject = null;
				}
				else if (objectManipulationInputController.CurrentlySelectedObject != null)
				{
					notNewlyAddedObjectSelect();
				}
				break;
			case InteractionState.DisabledInput:
			case InteractionState.SwipeScreen:
				newlyAddedObject = null;
				break;
			}
		}

		private void notNewlyAddedObjectSelect()
		{
			ManipulatableObject component = objectManipulationInputController.CurrentlySelectedObject.GetComponent<ManipulatableObject>();
			if (component != null)
			{
				if (CenterCameraOnSelect)
				{
					calculateTargetPositionForSelectedObject(component);
				}
				else
				{
					eventDispatcher.DispatchEvent(new IglooUIEvents.ShowSelectedUIWidget(component, boundsForCameraTarget, MinCameraTargetDistanceOnNewItem));
				}
			}
		}

		private void calculateTargetPositionForSelectedObject(ManipulatableObject mo)
		{
			switch (sceneStateData.State)
			{
			case SceneStateData.SceneState.Edit:
				if (mo.Type == DecorationLayoutData.DefinitionType.Structure)
				{
					return;
				}
				break;
			case SceneStateData.SceneState.StructurePlacement:
				if (mo.Type == DecorationLayoutData.DefinitionType.Decoration)
				{
					return;
				}
				break;
			}
			Vector3 baseOfTargetPoint = IglooTargetUtil.GetBaseOfTargetPoint(mo, MinCameraTargetDistanceOnNewItem);
			updateEditCameraTargetPosition(baseOfTargetPoint);
			CenterCameraOnSelect = false;
			eventDispatcher.DispatchEvent(new IglooUIEvents.ShowSelectedUIWidget(mo, boundsForCameraTarget, MinCameraTargetDistanceOnNewItem));
		}

		private void onObjectManipulationInputControllerObjectDeselected(ObjectManipulator obj)
		{
			CenterCameraOnSelect = true;
		}

		private void onObjectManipulationInputControllerObjectBeforeDelete(ManipulatableObject obj)
		{
			CenterCameraOnSelect = true;
		}

		private bool sceneIsLoaded()
		{
			return sceneStateData != null && director != null && director.IsStartUpComplete && objectManipulationInputController != null;
		}

		private void changeCameraTarget(SceneStateData.SceneState state)
		{
			if (!sceneIsLoaded())
			{
				return;
			}
			switch (state)
			{
			case SceneStateData.SceneState.Play:
			case SceneStateData.SceneState.Preview:
				removeListeners();
				changeCameraRail(state);
				if (director != null)
				{
					director.SoftResetCamera();
				}
				wasCameraTargetReset = false;
				break;
			case SceneStateData.SceneState.StructurePlacement:
				setupEditing();
				changeCameraRail(state);
				eventDispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(EditCameraTarget.transform.transform));
				if (Service.Get<TutorialManager>().IsTutorialRunning())
				{
					centerCamera();
				}
				break;
			case SceneStateData.SceneState.Edit:
				setupEditing();
				changeCameraRail(state);
				eventDispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(EditCameraTarget.transform.transform));
				break;
			}
		}

		private void changeCameraRail(SceneStateData.SceneState state)
		{
			switch (state)
			{
			case SceneStateData.SceneState.Play:
				updateRail(PreviewCameraControllerRail);
				break;
			case SceneStateData.SceneState.Preview:
				eventDispatcher.DispatchEvent(new CinematographyEvents.CameraSnapLockEvent(false, false));
				updateRail(PreviewCameraControllerRail);
				break;
			case SceneStateData.SceneState.StructurePlacement:
				updateRail(StructureCameraControllerRail);
				break;
			case SceneStateData.SceneState.Edit:
				updateRail(EditCameraControllerRail);
				break;
			}
		}

		private void setupEditing()
		{
			if (!wasCameraTargetReset)
			{
				moveEditCameraTargetToDirectorPostion();
				wasCameraTargetReset = true;
			}
			setupListeners();
		}

		private void centerCamera()
		{
			MultiPointLineAttractorLocator multiPointLineAttractorLocator = Object.FindObjectOfType<MultiPointLineAttractorLocator>();
			if (!(multiPointLineAttractorLocator != null))
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			int num = 0;
			for (int i = 0; i < multiPointLineAttractorLocator.AttractorContainers.Length; i++)
			{
				GameObject gameObject = multiPointLineAttractorLocator.AttractorContainers[i];
				if (gameObject != null)
				{
					zero += gameObject.transform.position;
					num++;
				}
			}
			if (num > 0)
			{
				EditCameraTarget.transform.position = zero / num;
			}
		}

		private void followDragItemWithEditCameraTarget(GameObject obj, Vector2 touchPosition, float bandSpeed)
		{
			if (bandSpeed > 0f)
			{
				float num = bandSpeed * Time.deltaTime;
				Vector3 normalized = (obj.transform.position - EditCameraTarget.transform.position).normalized;
				normalized.Scale(new Vector3(num, 0f, num));
				updateEditCameraTargetPosition(EditCameraTarget.transform.position + normalized);
			}
		}

		private void moveEditCameraTargetToDirectorPostion()
		{
			Vector3 position = director.DefaultTarget.position;
			if (boundsForCameraTarget.Contains(position))
			{
				EditCameraTarget.transform.position = position;
			}
			else
			{
				EditCameraTarget.transform.position = boundsForCameraTarget.ClosestPoint(position);
			}
		}

		private void updateEditCameraTargetPosition(Vector3 adjustedPosition)
		{
			if (boundsForCameraTarget.Contains(adjustedPosition))
			{
				EditCameraTarget.transform.position = adjustedPosition;
			}
			else
			{
				EditCameraTarget.transform.position = boundsForCameraTarget.ClosestPoint(adjustedPosition);
			}
		}

		private void updateRail(CameraController newCameraControllerRail)
		{
			if (currentRailCameraController != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = currentRailCameraController;
				eventDispatcher.DispatchEvent(evt);
			}
			CinematographyEvents.CameraLogicChangeEvent evt2 = default(CinematographyEvents.CameraLogicChangeEvent);
			evt2.Controller = newCameraControllerRail;
			eventDispatcher.DispatchEvent(evt2);
			currentRailCameraController = newCameraControllerRail;
		}
	}
}
