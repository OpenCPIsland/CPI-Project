using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class PropertyCustomizationController : MonoBehaviour
	{
		private const string HIDDEN_BOOL_ANIM = "IsHidden";

		private const string SHOW_ANIM = "Show";

		private const string HIDE_ANIM = "Hide";

		[SerializeField]
		private GameObject fabricScreenGrouped;

		[SerializeField]
		private GameObject fabricScreenSingles;

		[SerializeField]
		private GameObject decalScreenGrouped;

		[SerializeField]
		private GameObject decalScreenSingles;

		[SerializeField]
		private GameObject saveScreen;

		[SerializeField]
		private GameObject designSubmission;

		[SerializeField]
		private GameObject saveButtonGameObject;

		[SerializeField]
		private GameObject contentButtons;

		[SerializeField]
		private float FadeAmount = 0.6f;

		[SerializeField]
		private float FadeTime = 0.5f;

		private DItemCustomization itemModel;

		private CustomizerModel customizerModel;

		private CustomizerState prevState;

		private CustomizerState currentState;

		private EventChannel eventChannel;

		private GameObject currentSubScreen;

		private bool hasLoggedScaleAnalytics;

		private ClothingDesignerOutliner clothingOutliner = null;

		private bool _isInputActive = false;

		private bool _isManipulatorActive = false;

		private bool _isChannelActive = false;

		private bool _isInited = false;

		public void Start()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerUIEvents.SelectChannelEvent>(onChannelSelected);
			eventChannel.AddListener<CustomizerModelEvents.ChannelChangedEvent>(onChangedChannel);
			eventChannel.AddListener<CustomizerUIEvents.InputStateChange>(OnInputStateChange);
			eventChannel.AddListener<CustomizerUIEvents.InputOverChannel>(OnInputOverChannel);
			eventChannel.AddListener<CustomizerUIEvents.SaveCancel>(onSaveCancelButton);
			eventChannel.AddListener<CustomizerWidgetEvents.RotationWidgetRotated>(onRotationWidgetRotated);
			eventChannel.AddListener<CustomizerWidgetEvents.TileValueChanged>(onTilingToggled);
			eventChannel.AddListener<CustomizerWidgetEvents.SliderWidgetValueChanged>(onScaleChanged);
			eventChannel.AddListener<CustomizerActiveSwatchEvents.ToggleActiveSwatch>(onToggleActiveSwatch);
		}

		public void StartCustomization()
		{
			_isInited = true;
			base.gameObject.SetActive(true);
			itemModel.ResetChannels();
			itemModel.SetSelectionColors(clothingOutliner.DarkenedSwatchColor);
			changeState(CustomizerState.FABRIC);
			itemModel.SelectChannel(itemModel.RedChannel);
			_isInputActive = false;
			_isManipulatorActive = false;
			_isChannelActive = false;
			updateProperties();
		}

		private void OnDisable()
		{
			changeState(CustomizerState.TEMPLATE);
			Object.Destroy(currentSubScreen);
		}

		public void SetModel(CustomizerModel customizerModel)
		{
			this.customizerModel = customizerModel;
			itemModel = customizerModel.ItemCustomization;
		}

		public void SetOutliner(ClothingDesignerOutliner clothingOutliner)
		{
			this.clothingOutliner = clothingOutliner;
			ClothingDesignerCameraController clothingDesignerCameraController = SceneRefs.ClothingDesignerCameraController;
			if (clothingDesignerCameraController != null)
			{
				Camera component = clothingDesignerCameraController.GetComponent<Camera>();
				if (component != null)
				{
					clothingOutliner.Init(component);
				}
			}
			clothingOutliner.enabled = false;
		}

		public void OnBackButton()
		{
			if (currentState != CustomizerState.SAVE && currentState != CustomizerState.SAVE_CATALOG)
			{
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.BackButtonClickedEvent(true));
			}
		}

		public void OnSaveButton()
		{
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				changeState(CustomizerState.SAVE_CATALOG);
			}
			else
			{
				changeState(CustomizerState.SAVE);
			}
		}

		public void OnFabricButton()
		{
			changeState(CustomizerState.FABRIC);
		}

		public void OnDecalButton()
		{
			changeState(CustomizerState.DECAL);
		}

		private bool onToggleActiveSwatch(CustomizerActiveSwatchEvents.ToggleActiveSwatch evt)
		{
			CustomizerState state = customizerModel.State;
			CustomizerState customizerState = state;
			switch (state)
			{
			case CustomizerState.DECAL:
				customizerState = CustomizerState.FABRIC;
				break;
			case CustomizerState.FABRIC:
				customizerState = CustomizerState.DECAL;
				break;
			}
			if (customizerState != state)
			{
				changeState(customizerState);
			}
			return false;
		}

		private bool onChannelSelected(CustomizerUIEvents.SelectChannelEvent evt)
		{
			itemModel.SelectChannel(evt.NewChannel);
			updateProperties();
			return false;
		}

		private bool onChangedChannel(CustomizerModelEvents.ChannelChangedEvent evt)
		{
			clothingOutliner.SelectedChannel = (int)evt.NewChannel.Mask;
			updateProperties();
			return false;
		}

		private bool OnInputStateChange(CustomizerUIEvents.InputStateChange evt)
		{
			if (evt.IsManipulator)
			{
				_isManipulatorActive = evt.IsDown;
			}
			else
			{
				_isInputActive = evt.IsDown;
			}
			updateProperties();
			return false;
		}

		private bool OnInputOverChannel(CustomizerUIEvents.InputOverChannel evt)
		{
			_isChannelActive = (evt.Channel != CustomizationChannel.NONE);
			updateProperties();
			return false;
		}

		private bool onScaleChanged(CustomizerWidgetEvents.SliderWidgetValueChanged evt)
		{
			float value = evt.Value;
			float num = convertSliderScale(value, true);
			switch (customizerModel.State)
			{
			case CustomizerState.FABRIC:
				itemModel.CurrentChannel.FabricScale = num;
				break;
			case CustomizerState.DECAL:
				itemModel.CurrentChannel.DecalScale = num;
				break;
			}
			if (!hasLoggedScaleAnalytics)
			{
				Service.Get<ICPSwrveService>().Action("game.template_adjust", "scale");
				hasLoggedScaleAnalytics = true;
			}
			return false;
		}

		private bool onRotationWidgetRotated(CustomizerWidgetEvents.RotationWidgetRotated evt)
		{
			float totalRotationDegrees = evt.TotalRotationDegrees;
			switch (customizerModel.State)
			{
			case CustomizerState.FABRIC:
				itemModel.CurrentChannel.FabricRotation = totalRotationDegrees;
				break;
			case CustomizerState.DECAL:
				itemModel.CurrentChannel.DecalRotation = totalRotationDegrees;
				break;
			}
			return false;
		}

		private bool onTilingToggled(CustomizerWidgetEvents.TileValueChanged evt)
		{
			CustomizerState state = customizerModel.State;
			if (state == CustomizerState.DECAL)
			{
				itemModel.CurrentChannel.IsDecalTiled = evt.Value;
			}
			return false;
		}

		private bool onSaveCancelButton(CustomizerUIEvents.SaveCancel evt)
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.HideCoinCountWidget));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SwitchToCustomize));
			changeState(prevState);
			return false;
		}

		private void changeState(CustomizerState newState)
		{
			bool isFabric = false;
			bool flag = false;
			prevState = currentState;
			currentState = newState;
			switch (newState)
			{
			case CustomizerState.FABRIC:
				customizerModel.State = CustomizerState.FABRIC;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.EnableDragAreaControllerUpdates));
				isFabric = true;
				flag = true;
				if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
				{
					changeSubScreen(fabricScreenSingles);
				}
				else
				{
					changeSubScreen(fabricScreenGrouped);
				}
				currentSubScreen.GetComponent<FabricScreenController>().Init(customizerModel);
				break;
			case CustomizerState.DECAL:
				customizerModel.State = CustomizerState.DECAL;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.EnableDragAreaControllerUpdates));
				flag = true;
				if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
				{
					changeSubScreen(decalScreenSingles);
				}
				else
				{
					changeSubScreen(decalScreenGrouped);
				}
				currentSubScreen.GetComponent<DecalScreenController>().Init(customizerModel);
				break;
			case CustomizerState.SAVE:
				customizerModel.State = CustomizerState.SAVE;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.DisableDragAreaControllerUpdates));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowCoinCountWidget));
				changeSubScreen(saveScreen);
				currentSubScreen.GetComponent<SaveScreenController>().SetSaveScreenInformation(customizerModel.ItemCustomization.TemplateSprite, customizerModel.ItemCustomization.TemplateDefinition.Cost);
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SwitchToSave));
				break;
			case CustomizerState.SAVE_CATALOG:
				customizerModel.State = CustomizerState.SAVE_CATALOG;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.DisableDragAreaControllerUpdates));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowCoinCountWidget));
				changeSubScreen(designSubmission);
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SwitchToSave));
				break;
			}
			if (_isInited)
			{
				saveButtonGameObject.SetActive(flag);
				contentButtons.SetActive(flag);
				CustomizationContext.EventBus.DispatchEvent(new CustomizerActiveSwatchEvents.SetIsVisible(flag));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerActiveSwatchEvents.SetIsFabric(isFabric));
				updateProperties();
			}
		}

		private void changeSubScreen(GameObject screenToInstantiate)
		{
			if (currentSubScreen != null)
			{
				Object.Destroy(currentSubScreen);
			}
			currentSubScreen = Object.Instantiate(screenToInstantiate);
			RectTransform rectTransform = (RectTransform)currentSubScreen.transform;
			rectTransform.SetParent(base.transform, false);
			rectTransform.localPosition = Vector3.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
		}

		private void updateProperties()
		{
			updateLighting();
			updateSwatch();
			updateToolSettings();
		}

		private void updateLighting()
		{
			bool flag = false;
			if (_isInputActive || _isManipulatorActive)
			{
				flag = true;
			}
			bool flag2 = false;
			if (flag && (_isChannelActive || _isManipulatorActive))
			{
				flag2 = true;
			}
			CustomizationContext.EventBus.DispatchEvent(new CustomizerEffectsEvents.FadeBackground(flag, FadeAmount, FadeTime));
			itemModel.SetCanFade(flag2);
			if (clothingOutliner != null)
			{
				clothingOutliner.enabled = flag2;
			}
		}

		private void updateSwatch()
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerActiveSwatchEvents.SetSwatch(itemModel.CurrentChannel.Fabric, itemModel.CurrentChannel.Decal));
		}

		private void updateToolSettings()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			float value = 0f;
			float scale = 1f;
			bool value2 = false;
			switch (currentState)
			{
			case CustomizerState.FABRIC:
				flag = true;
				flag3 = true;
				flag2 = (itemModel.CurrentChannel.hasFabric() && itemModel.CurrentChannel.canRotateAndScaleFabric());
				value = itemModel.CurrentChannel.FabricRotation;
				scale = itemModel.CurrentChannel.FabricScale;
				break;
			case CustomizerState.DECAL:
				flag = true;
				flag3 = true;
				flag4 = true;
				flag2 = itemModel.CurrentChannel.hasDecal();
				value = itemModel.CurrentChannel.DecalRotation;
				scale = itemModel.CurrentChannel.DecalScale;
				value2 = itemModel.CurrentChannel.IsDecalTiled;
				break;
			}
			bool flag5 = _isInputActive || _isManipulatorActive;
			if (flag5)
			{
				flag2 = false;
			}
			if (flag)
			{
				CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.RotationWidgetSetValue(value));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.SetSliderWidgetValue(convertSliderScale(scale, false)));
				CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.SetTileValue(value2));
			}
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.RotationWdigetSetIsInteractable(flag2));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.SetIsSliderWidgetInteractable(flag2));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.SetIsTileInteractable(flag2));
			CustomizationContext.EventBus.DispatchEvent(new CustomizerActiveSwatchEvents.SetIsInteractable(!flag5));
			if (flag3)
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowScaleAndRotateWidget));
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.HideScaleAndRotateWidget));
			}
			if (flag4)
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowTileWidget));
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.HideTileWidget));
			}
		}

		private float convertSliderScale(float scale, bool fromSlider)
		{
			float num = 50f;
			float num2 = 2f;
			float num3 = 0f;
			if (fromSlider)
			{
				return scale * scale / num + num2;
			}
			return Mathf.Sqrt((scale - num2) * num);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
