using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.ClothingDesigner.ItemCustomizer;
using ClubPenguin.Core;
using ClubPenguin.Igloo.Catalog;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.SceneManipulation;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(Button))]
	public class IglooCustomizationButton : CustomizationButton, IBeginDragHandler, IDragHandler, IEndDragHandler, ICancelHandler, IEventSystemHandler, ILockableButton
	{
		public enum CatalogFilterType
		{
			ALL,
			STRUCTURES,
			CATEGORY
		}

		private const string SHOW_ANIM_PARAM = "Show";

		private const string HIDE_ANIM_PARAM = "Hide";

		private readonly PrefabContentKey IglooCatalogPrefabKey = new PrefabContentKey("UI/IglooCatalog/IglooCatalog");

		public Animator SelectedAnimator;

		[SerializeField]
		[Header("The value checked when dragging the button in the Y direction.")]
		[Tooltip("This value determines if a user has actually attempted to drag an icon or move the scrollrect.")]
		private float dragTolerance = 6f;

		[SerializeField]
		private GameObject progressionBadge;

		[SerializeField]
		private GameObject mascotBadges;

		[SerializeField]
		private GameObject memberlockBadge;

		[SerializeField]
		private GameObject sizeLockBadge;

		[SerializeField]
		private MaterialSelector materialSelector;

		[SerializeField]
		private GameObject itemCountPanel;

		[SerializeField]
		private Text itemCountText;

		[SerializeField]
		private GameObject SizeIconSelector;

		[SerializeField]
		[Header("Small is the first entry, large is the third")]
		private SpriteSelector sizeIconSpriteSelector;

		[SerializeField]
		[Header("Button Transformation")]
		private GameObject itemContentContainer;

		[SerializeField]
		private GameObject catalogButtonContainer;

		[Header("Sounds")]
		[SerializeField]
		private string dragEventName;

		[SerializeField]
		private EventAction dragEventType;

		[SerializeField]
		private string dragEndEventName;

		[SerializeField]
		private EventAction dragEndEventType;

		private Button button;

		private ScrollRect scrollRect;

		private DragContainer dragContainer;

		private bool didDrag;

		private float dragHeight;

		private int index;

		private bool isMemberLocked = false;

		private int count = 0;

		private bool showCountIfZero;

		private bool tintIfZero;

		private PersistentBreadcrumbTypeDefinitionKey breadcrumbType;

		private int breadcrumbId;

		private SceneManipulationService sceneManipulationService;

		private bool didDropItem;

		private CatalogFilterType catalogFilterType;

		private int catalogFilterCategory;

		public int Count
		{
			get
			{
				return count;
			}
		}

		public bool CanSelect
		{
			get;
			set;
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public event Action<IglooCustomizationButton> IglooButtonClicked;

		public event Action<IglooCustomizationButton, Vector2, int> DraggedOffDragArea;

		protected override void Awake()
		{
			base.Awake();
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(false);
			memberlockBadge.SetActive(false);
			itemCountPanel.SetActive(false);
			SizeIconSelector.SetActive(false);
			button = GetComponent<Button>();
			button.onClick.AddListener(onButton);
			sceneManipulationService = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>();
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onTutorialComplete));
		}

		public override void Init(Texture2DContentKey customizationAssetKey, PersistentBreadcrumbTypeDefinitionKey breadcrumbIdType, int definitionId, bool canDrag)
		{
			SizeIconSelector.SetActive(false);
			itemCountPanel.SetActive(false);
			base.Init(customizationAssetKey, breadcrumbIdType, definitionId, canDrag);
			CheckBreadcrumb(breadcrumbIdType, definitionId);
		}

		public void SetupItemButton()
		{
			CanSelect = true;
			SetCanDrag();
			itemContentContainer.SetActive(true);
			catalogButtonContainer.SetActive(false);
		}

		public void SetupCatalogButton(CatalogFilterType filterType = CatalogFilterType.ALL)
		{
			SetCanDrag(false);
			itemContentContainer.SetActive(false);
			catalogButtonContainer.SetActive(true);
			catalogFilterType = filterType;
		}

		public void SetCatalogFilterCategory(int categoryId)
		{
			catalogFilterType = CatalogFilterType.CATEGORY;
			catalogFilterCategory = categoryId;
		}

		public void OnCatalogButtonClicked()
		{
			SceneRefs.FullScreenPopupManager.CreatePopup(IglooCatalogPrefabKey, null, false, OnCatalogPrefabInstantiated);
			Service.Get<ICPSwrveService>().Action("furniture_catalog", "tray_ui");
		}

		public void OnCatalogPrefabInstantiated(PrefabContentKey key, GameObject instance)
		{
			IglooCatalogController component = instance.GetComponent<IglooCatalogController>();
			if (catalogFilterType == CatalogFilterType.STRUCTURES)
			{
				component.SetDefaultFilterToStructures();
			}
			else if (catalogFilterType == CatalogFilterType.CATEGORY)
			{
				component.SetDefaultFilterToCategoryId(catalogFilterCategory);
			}
			else
			{
				component.SetDefaultFilterToAll();
			}
		}

		public void SetDragReferences(ScrollRect scrollRect, int index, float dragHeight, DragContainer dragContainer = null)
		{
			this.scrollRect = scrollRect;
			this.index = index;
			this.dragHeight = dragHeight;
			this.dragContainer = dragContainer;
		}

		public void SetCanDrag(bool drag = true)
		{
			base.CanDrag = drag;
		}

		public void CheckBreadcrumb(PersistentBreadcrumbTypeDefinitionKey breadcrumbIdType, int definitionId)
		{
			breadcrumbType = breadcrumbIdType;
			breadcrumbId = definitionId;
		}

		public void RemoveBreadcrumb()
		{
			Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(breadcrumbType, breadcrumbId.ToString());
		}

		public Sprite GetImage()
		{
			return IconImage.sprite;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			didDropItem = false;
			didDrag = (checkDidDrag(eventData.delta) && base.CanDrag);
			if (didDrag)
			{
				handoffToDragContainer(eventData);
			}
			else
			{
				scrollRect.OnBeginDrag(eventData);
			}
		}

		private void handoffToDragContainer(PointerEventData eventData)
		{
			if (!didDropItem)
			{
				if (sceneManipulationService.IsLayoutAtMaxItemLimit())
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(IglooUIEvents.MaxItemsLimitReached));
					SetItemCount(count + 1, showCountIfZero, tintIfZero);
					EventManager.Instance.PostEvent(dragEndEventName, dragEndEventType, base.gameObject);
				}
				else
				{
					RemoveBreadcrumb();
					EventManager.Instance.PostEvent(dragEventName, dragEventType, base.gameObject);
					dragContainer.transform.position = eventData.position;
					dragContainer.SetImage(GetImage());
					ShowDragContainer();
					SetItemCount(count - 1, showCountIfZero, tintIfZero);
				}
				didDropItem = true;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (didDrag && dragContainer.IsShowing)
			{
				if (eventData.position.y <= dragHeight)
				{
					dragContainer.transform.position = eventData.position;
					return;
				}
				HideDragContainer();
				if (this.DraggedOffDragArea != null)
				{
					this.DraggedOffDragArea(this, eventData.position, index);
				}
			}
			else
			{
				didDrag = (checkDidDrag(eventData.delta) && base.CanDrag);
				if (didDrag)
				{
					handoffToDragContainer(eventData);
				}
				else
				{
					scrollRect.OnDrag(eventData);
				}
			}
		}

		private void ShowDragContainer()
		{
			dragContainer.Show();
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.BeginDragInventoryItem));
		}

		private void HideDragContainer()
		{
			dragContainer.Hide();
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.EndDragInventoryItem));
		}

		private bool checkDidDrag(Vector2 dragDelta)
		{
			float num = dragDelta.y;
			if (num < 0f)
			{
				num *= -1f;
			}
			return num >= dragTolerance && num > Mathf.Abs(dragDelta.x);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			didDropItem = false;
			if (didDrag)
			{
				if (eventData.position.y <= dragHeight)
				{
					SetItemCount(count + 1, showCountIfZero, tintIfZero);
				}
				HideDragContainer();
				EventManager.Instance.PostEvent(dragEndEventName, dragEndEventType, base.gameObject);
			}
			else
			{
				scrollRect.OnEndDrag(eventData);
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.EndDragInventoryItem));
		}

		private void onButton()
		{
			if (CanSelect)
			{
				RemoveBreadcrumb();
				if (this.IglooButtonClicked != null)
				{
					this.IglooButtonClicked(this);
				}
			}
			else if (isMemberLocked)
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("igloo_item");
			}
		}

		private void onTutorialComplete(TutorialDefinition definition)
		{
			if (definition.Id == IglooButtonUtils.IGLOO_TUTORIAL_ID)
			{
				TutorialManager tutorialManager = Service.Get<TutorialManager>();
				tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onTutorialComplete));
				if (isMemberLocked)
				{
					button.enabled = true;
				}
			}
		}

		public void SetUnlocked()
		{
			CanSelect = true;
			materialSelector.SelectMaterial(0);
			mascotBadges.SetActive(false);
			progressionBadge.SetActive(false);
			memberlockBadge.SetActive(false);
			if (sizeLockBadge != null)
			{
				sizeLockBadge.SetActive(false);
			}
		}

		public void SetLevelLocked(int level)
		{
			CanSelect = false;
			SetCanDrag(false);
			materialSelector.SelectMaterial(1);
			itemCountPanel.SetActive(false);
			mascotBadges.SetActive(false);
			progressionBadge.SetActive(true);
			memberlockBadge.SetActive(false);
			if (sizeLockBadge != null)
			{
				sizeLockBadge.SetActive(false);
			}
			progressionBadge.GetComponentInChildren<Text>().text = level.ToString();
		}

		public void SetProgressionLocked(string mascotName)
		{
			CanSelect = false;
			SetCanDrag(false);
			materialSelector.SelectMaterial(1);
			itemCountPanel.SetActive(false);
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(true);
			memberlockBadge.SetActive(false);
			if (sizeLockBadge != null)
			{
				sizeLockBadge.SetActive(false);
			}
			IList<Transform> children = mascotBadges.GetChildren();
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].name.Equals(mascotName))
				{
					children[i].gameObject.SetActive(true);
				}
				else
				{
					children[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetMemberLocked()
		{
			CanSelect = false;
			SetCanDrag(false);
			isMemberLocked = true;
			materialSelector.SelectMaterial(1);
			itemCountPanel.SetActive(false);
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(false);
			memberlockBadge.SetActive(true);
			if (sizeLockBadge != null)
			{
				sizeLockBadge.SetActive(false);
			}
			if (Service.Get<TutorialManager>().IsTutorialRunning())
			{
				button.enabled = false;
			}
		}

		public void SetSizeLocked()
		{
			CanSelect = false;
			SetCanDrag(false);
			materialSelector.SelectMaterial(1);
			itemCountPanel.SetActive(true);
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(false);
			memberlockBadge.SetActive(false);
			if (sizeLockBadge != null)
			{
				sizeLockBadge.SetActive(true);
			}
		}

		public void SetItemCount(int value, bool showIfZero, bool tintIfZero)
		{
			showCountIfZero = showIfZero;
			if (count >= 0)
			{
				count = value;
				itemCountPanel.SetActive(count > 0 || showIfZero);
				itemCountText.text = count.ToString();
			}
			this.tintIfZero = tintIfZero;
			materialSelector.SelectMaterial((tintIfZero && count <= 0) ? 1 : 0);
			base.CanDrag = (count > 0);
		}

		public void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				SelectedAnimator.ResetTrigger("Hide");
				SelectedAnimator.SetTrigger("Show");
			}
			else
			{
				SelectedAnimator.ResetTrigger("Show");
				SelectedAnimator.SetTrigger("Hide");
			}
		}

		public void SetSizeIconSprite(int spriteIndex)
		{
			sizeIconSpriteSelector.SelectSprite(spriteIndex);
			SizeIconSelector.SetActive(true);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onTutorialComplete));
			button.onClick.RemoveListener(onButton);
			this.IglooButtonClicked = null;
		}
	}
}
