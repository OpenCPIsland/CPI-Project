using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.SceneManipulation;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo
{
	public class IglooRemoveDecorationUIController : MonoBehaviour
	{
		public Button ConfirmButton;

		public Button DuplicateButton;

		public float ButtonTransparencyWhenNotAllowed = 0.4f;

		public RectTransform panelTransform;

		private float minCameraDistance = 1f;

		private Canvas canvas;

		private CanvasScalerExt canvasScalerExt;

		private Image confirmButtonImage;

		private Image duplicateButtonImage;

		private BaseCamera baseCamera;

		private SceneManipulationService sceneManipulationService;

		private ObjectManipulator objectManipulator;

		private ManipulatableObject manipulatableObject;

		private DecorationInventoryService decorationInventoryService;

		private Bounds boundsForCameraTarget;

		private bool isDecorationSet;

		public bool IsInitialized
		{
			get;
			private set;
		}

		public void Awake()
		{
			canvas = GetComponent<Canvas>();
			canvasScalerExt = GetComponent<CanvasScalerExt>();
			manipulatableObject = GetComponentInParent<ManipulatableObject>();
			decorationInventoryService = Service.Get<DecorationInventoryService>();
			confirmButtonImage = ConfirmButton.GetComponent<Image>();
			duplicateButtonImage = DuplicateButton.GetComponent<Image>();
			baseCamera = SceneRefs.Get<BaseCamera>();
			BaseCamera obj = baseCamera;
			obj.Moved = (System.Action)Delegate.Combine(obj.Moved, new System.Action(OnCameraMoved));
			Service.Get<EventDispatcher>().AddListener<InputEvents.ZoomEvent>(onInputZoom);
			IsInitialized = false;
			isDecorationSet = false;
			hide();
		}

		public void OnEnable()
		{
		}

		public void Init(SceneManipulationService sceneManipulationService, Bounds boundsForCameraTarget, float minCameraDistance)
		{
			this.sceneManipulationService = sceneManipulationService;
			this.boundsForCameraTarget = boundsForCameraTarget;
			this.minCameraDistance = minCameraDistance;
			sceneManipulationService.ObjectManipulationInputController.InteractionStateChanged += OnObjectManipulationInputControllerInteractionStateChanged;
			IsInitialized = true;
		}

		public void SetNewDecoration(ManipulatableObject manipulatableObject)
		{
			removeDecorationListeners();
			this.manipulatableObject = manipulatableObject;
			objectManipulator = manipulatableObject.GetComponent<ObjectManipulator>();
			objectManipulator.PositionChanged += OnObjectManipulatorPositionChanged;
			objectManipulator.IsAllowedChanged += OnObjectManipulatorIsAllowedChanged;
			OnObjectManipulatorIsAllowedChanged();
			SetPosition();
			CoroutineRunner.Start(showNewDecoration(), this, "Show New Decoration");
		}

		private IEnumerator showNewDecoration()
		{
			yield return new WaitUntil(() => canvasScalerExt.ScaleFactorSet);
			if (sceneManipulationService.ObjectManipulationInputController.CurrentState.State == InteractionState.ActiveSelectedItem)
			{
				show();
			}
			isDecorationSet = true;
		}

		public void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			BaseCamera obj = baseCamera;
			obj.Moved = (System.Action)Delegate.Remove(obj.Moved, new System.Action(OnCameraMoved));
			removeDecorationListeners();
			Service.Get<EventDispatcher>().RemoveListener<InputEvents.ZoomEvent>(onInputZoom);
			if (sceneManipulationService != null && sceneManipulationService.ObjectManipulationInputController != null)
			{
				sceneManipulationService.ObjectManipulationInputController.InteractionStateChanged -= OnObjectManipulationInputControllerInteractionStateChanged;
			}
		}

		private void removeDecorationListeners()
		{
			if (objectManipulator != null)
			{
				objectManipulator.PositionChanged -= OnObjectManipulatorPositionChanged;
				objectManipulator.IsAllowedChanged -= OnObjectManipulatorIsAllowedChanged;
			}
		}

		private bool onInputZoom(InputEvents.ZoomEvent evt)
		{
			if (isDecorationSet)
			{
				SetPosition();
				show();
			}
			return false;
		}

		private void OnObjectManipulationInputControllerInteractionStateChanged(InteractionState state)
		{
			if (isDecorationSet)
			{
				if (state == InteractionState.ActiveSelectedItem)
				{
					SetPosition();
					show();
				}
				else
				{
					hide();
				}
			}
		}

		private void OnCameraMoved()
		{
			SetPosition();
		}

		private void OnObjectManipulatorPositionChanged(ObjectManipulator obj)
		{
			hide();
			SetPosition();
		}

		private void UpdateDuplicateButton()
		{
			bool flag = true;
			if (manipulatableObject.Type == DecorationLayoutData.DefinitionType.Decoration)
			{
				flag = (sceneManipulationService.GetAvailableDecorationCount(manipulatableObject.DefinitionId) <= 0);
				if (!flag)
				{
					flag = sceneManipulationService.IsLayoutAtMaxItemLimit();
				}
			}
			updateButtonInteractableState(DuplicateButton, duplicateButtonImage, flag);
		}

		private void OnObjectManipulatorIsAllowedChanged()
		{
			updateButtonInteractableState(ConfirmButton, confirmButtonImage);
			UpdateDuplicateButton();
		}

		private void updateButtonInteractableState(Button buttonToUpdate, Image imageToUpdate, bool forceNotAllowed = false)
		{
			float a = 1f;
			Color color = imageToUpdate.color;
			if (forceNotAllowed || !objectManipulator.IsAllowed)
			{
				a = ButtonTransparencyWhenNotAllowed;
				buttonToUpdate.interactable = false;
			}
			else
			{
				buttonToUpdate.interactable = true;
			}
			color.a = a;
			imageToUpdate.color = color;
		}

		private void SetPosition()
		{
			if (isDecorationSet && !(manipulatableObject == null))
			{
				Vector3 vector = IglooTargetUtil.GetBaseOfTargetPoint(manipulatableObject, minCameraDistance);
				if (!boundsForCameraTarget.Contains(vector))
				{
					vector = boundsForCameraTarget.ClosestPoint(vector);
				}
				Vector3 a = Camera.main.WorldToScreenPoint(vector);
				Vector3 v = a * (1f / canvas.scaleFactor);
				v.y += panelTransform.rect.height * 0.25f;
				panelTransform.anchoredPosition = v;
			}
		}

		public void UnsetDecoration()
		{
			isDecorationSet = false;
			hide();
		}

		private void hide()
		{
			panelTransform.gameObject.SetActive(false);
		}

		private void show()
		{
			panelTransform.gameObject.SetActive(true);
		}

		private bool structureHasItems(DecorationLayoutData structureData)
		{
			if (structureData.Type == DecorationLayoutData.DefinitionType.Structure && sceneManipulationService != null && sceneManipulationService.SceneLayoutData != null)
			{
				string fullPath = structureData.Id.GetFullPath();
				foreach (DecorationLayoutData item in sceneManipulationService.SceneLayoutData.GetLayoutEnumerator())
				{
					DecorationLayoutData.ID id = item.Id;
					if (id.ParentPath == fullPath)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void OnDeleteClicked()
		{
			if (manipulatableObject.Type == DecorationLayoutData.DefinitionType.Structure)
			{
				if (sceneManipulationService != null)
				{
					if (sceneManipulationService.SceneLayoutData != null)
					{
						DecorationLayoutData decorationLayoutData = default(DecorationLayoutData);
						decorationLayoutData.Id = DecorationLayoutData.ID.FromFullPath(sceneManipulationService.GetRelativeGameObjectPath(manipulatableObject.gameObject));
						decorationLayoutData.Type = DecorationLayoutData.DefinitionType.Structure;
						DecorationLayoutData structureData = decorationLayoutData;
						if (structureHasItems(structureData))
						{
							Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.RemovingStructureWithItemsEvent));
						}
						else
						{
							Service.Get<EventDispatcher>().DispatchEvent(new ObjectManipulationEvents.DeleteSelectedItemEvent(false));
						}
					}
					else
					{
						UnsetDecoration();
						Log.LogError(this, "sceneManipulationService.layout is null");
					}
				}
				else
				{
					UnsetDecoration();
					Log.LogError(this, "sceneManipulationService is null");
				}
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ObjectManipulationEvents.DeleteSelectedItemEvent(false));
			}
		}

		public void OnOKClicked()
		{
			UnsetDecoration();
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent));
		}

		public void OnDuplicateClicked()
		{
			UnsetDecoration();
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.DuplicateSelectedObject));
			UpdateDuplicateButton();
		}
	}
}
