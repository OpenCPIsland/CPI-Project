using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class EquipmentCustomizationController : MonoBehaviour
	{
		private enum State
		{
			PRE_SELECT,
			SELECT_TEMPLATE,
			TEMPLATE_CHOSEN,
			CUSTOMIZER
		}

		private const string SAVING_SUCCESS_TOKEN = "ClothingDesigner.Customizer.SaveItemSuccess";

		[SerializeField]
		private GameObject TopTrayCategory;

		[SerializeField]
		private GameObject TopTrayChosen;

		[SerializeField]
		private GameObject TopTrayCustomization;

		[SerializeField]
		private GameObject TopTraySave;

		[SerializeField]
		private GameObject TopTraySaving;

		[SerializeField]
		private GameObject TopTraySubmitting;

		[SerializeField]
		private float SaveEffectDuration = 1.5f;

		[SerializeField]
		private float SaveFadeAmount = 0.2f;

		[SerializeField]
		private float SaveFadeTime = 1.2f;

		[SerializeField]
		private float SaveEffectDelay = 0.7f;

		private TemplateSelectionController templateSelectionController;

		private TemplateChosenController templateChosenController;

		private PropertyCustomizationController propertyCustomizationController;

		private CustomizerModel customizerModel;

		private EventChannel eventChannel;

		private State currentState;

		private BackButtonController backButtonController;

		private static readonly DPrompt promptData = new DPrompt("GlobalUI.Prompts.loseProgressTitle", "GlobalUI.Prompts.newItemNoSave", DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.OK);

		private void OnEnable()
		{
			addBackButtonHandler();
		}

		private void addBackButtonHandler()
		{
			if (backButtonController != null)
			{
				backButtonController.Add(onAndroidBackButtonClicked);
			}
		}

		private void OnDisable()
		{
			if (backButtonController != null)
			{
				backButtonController.Remove(onAndroidBackButtonClicked);
			}
		}

		private void onAndroidBackButtonClicked()
		{
			if (backOrReturn())
			{
				addBackButtonHandler();
			}
		}

		public void BackToHomeButton()
		{
			backOrReturn();
		}

		private bool backOrReturn()
		{
			bool result = true;
			switch (currentState)
			{
			case State.SELECT_TEMPLATE:
			case State.TEMPLATE_CHOSEN:
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ChangeStateInventory));
				result = false;
				break;
			case State.CUSTOMIZER:
				returnToSelectTemplate();
				break;
			}
			return result;
		}

		public void SetDependancies(TemplateSelectionController templateSelectionController, TemplateChosenController templateChosenController, PropertyCustomizationController propertyCustomizationController, CustomizerModel customizerModel)
		{
			this.templateSelectionController = templateSelectionController;
			this.templateChosenController = templateChosenController;
			this.propertyCustomizationController = propertyCustomizationController;
			this.customizerModel = customizerModel;
		}

		public void Init()
		{
			changeState(State.SELECT_TEMPLATE);
		}

		private void Awake()
		{
			backButtonController = Service.Get<BackButtonController>();
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerUIEvents.BackButtonClickedEvent>(onBackButtonClicked);
			eventChannel.AddListener<CustomizerUIEvents.SaveItem>(onSaveClothingItemSuccess);
			eventChannel.AddListener<CustomizerUIEvents.SaveItemFailure>(onSaveClothingItemFailure);
			eventChannel.AddListener<CustomizerModelEvents.TemplateChangedEvent>(onTemplateChosen);
			eventChannel.AddListener<CustomizerUIEvents.TemplateConfirmed>(onTemplateConfirmed);
			eventChannel.AddListener<CustomizerUIEvents.SwitchToCustomize>(onSwitchToCustomize);
			eventChannel.AddListener<CustomizerUIEvents.SwitchToSave>(onSwitchToSave);
			eventChannel.AddListener<CustomizerUIEvents.StartPurchaseMoment>(onStartPurchaseMoment);
			eventChannel.AddListener<CustomizerUIEvents.EndPurchaseMoment>(onEndPurchaseMoment);
		}

		private bool onTemplateChosen(CustomizerModelEvents.TemplateChangedEvent evt)
		{
			changeState(State.TEMPLATE_CHOSEN);
			return false;
		}

		private bool onTemplateConfirmed(CustomizerUIEvents.TemplateConfirmed evt)
		{
			changeState(State.CUSTOMIZER);
			return false;
		}

		private void changeState(State newState)
		{
			switch (newState)
			{
			case State.PRE_SELECT:
				templateSelectionController.Hide();
				propertyCustomizationController.gameObject.SetActive(false);
				templateChosenController.gameObject.SetActive(false);
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowCloseButton));
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ResetRotation));
				break;
			case State.SELECT_TEMPLATE:
				templateSelectionController.Show();
				propertyCustomizationController.gameObject.SetActive(false);
				templateChosenController.gameObject.SetActive(false);
				setTopTrayToTemplate();
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowCoinCountWidget));
				CustomizationContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CustomizerTemplateState));
				ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Default, true));
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.HideCloseButton));
				break;
			case State.TEMPLATE_CHOSEN:
				templateChosenController.gameObject.SetActive(true);
				templateSelectionController.Hide();
				propertyCustomizationController.gameObject.SetActive(false);
				setTopTrayToTemplateSelect();
				CustomizationContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CustomizerChosenState));
				break;
			case State.CUSTOMIZER:
				propertyCustomizationController.StartCustomization();
				templateSelectionController.Hide();
				templateChosenController.gameObject.SetActive(false);
				setTopTrayToCustomization();
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.HideCoinCountWidget));
				CustomizationContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CustomizerEditingState));
				break;
			}
			currentState = newState;
		}

		private void setTopTrayToTemplate()
		{
			TopTrayCategory.SetActive(true);
			TopTrayChosen.SetActive(false);
			TopTrayCustomization.SetActive(false);
			TopTraySave.SetActive(false);
			TopTraySaving.SetActive(false);
			TopTraySubmitting.SetActive(false);
		}

		private void setTopTrayToTemplateSelect()
		{
			TopTrayChosen.SetActive(true);
			TopTrayCategory.SetActive(false);
			TopTrayCustomization.SetActive(false);
			TopTraySave.SetActive(false);
			TopTraySaving.SetActive(false);
			TopTraySubmitting.SetActive(false);
		}

		private void setTopTrayToCustomization()
		{
			TopTrayCustomization.SetActive(true);
			TopTrayChosen.SetActive(false);
			TopTrayCategory.SetActive(false);
			TopTraySave.SetActive(false);
			TopTraySaving.SetActive(false);
			TopTraySubmitting.SetActive(false);
		}

		private void setTopTrayToSave()
		{
			TopTraySave.SetActive(true);
			TopTrayChosen.SetActive(false);
			TopTrayCategory.SetActive(false);
			TopTrayCustomization.SetActive(false);
			TopTraySaving.SetActive(false);
			TopTraySubmitting.SetActive(false);
		}

		private void setTopTrayToSaving()
		{
			TopTraySaving.SetActive(true);
			TopTrayChosen.SetActive(false);
			TopTrayCategory.SetActive(false);
			TopTrayCustomization.SetActive(false);
			TopTraySave.SetActive(false);
			TopTraySubmitting.SetActive(false);
		}

		private void setTopTraySubmitting()
		{
			TopTraySubmitting.SetActive(true);
			TopTraySaving.SetActive(false);
			TopTrayChosen.SetActive(false);
			TopTrayCategory.SetActive(false);
			TopTrayCustomization.SetActive(false);
			TopTraySave.SetActive(false);
		}

		private bool onSwitchToCustomize(CustomizerUIEvents.SwitchToCustomize evt)
		{
			setTopTrayToCustomization();
			return false;
		}

		private bool onSwitchToSave(CustomizerUIEvents.SwitchToSave evt)
		{
			setTopTrayToSave();
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ResetRotation));
			return false;
		}

		private bool onStartPurchaseMoment(CustomizerUIEvents.StartPurchaseMoment evt)
		{
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				setTopTraySubmitting();
			}
			else
			{
				setTopTrayToSaving();
			}
			CustomizationContext.EventBus.DispatchEvent(new CustomizerEffectsEvents.FadeBackground(true, SaveFadeAmount, SaveFadeTime));
			CoroutineRunner.Start(sendItemSavingEvent(), this, "SendItemSaveEvent");
			return false;
		}

		private IEnumerator sendItemSavingEvent()
		{
			yield return new WaitForSeconds(SaveEffectDelay);
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerEffectsEvents.ItemSaving));
		}

		private bool onEndPurchaseMoment(CustomizerUIEvents.EndPurchaseMoment evt)
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerEffectsEvents.FadeBackground(false, 1f, SaveFadeTime));
			return false;
		}

		private bool onSaveClothingItemSuccess(CustomizerUIEvents.SaveItem evt)
		{
			CoroutineRunner.Start(DelaySave(), this, "DelaySave");
			return false;
		}

		private IEnumerator DelaySave()
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerEffectsEvents.ItemSaved));
			yield return new WaitForSeconds(SaveEffectDuration);
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowCloseButton));
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ChangeStateInventory));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.EndPurchaseMoment));
			showSaveNotification(Service.Get<Localizer>().GetTokenTranslation("ClothingDesigner.Customizer.SaveItemSuccess"));
		}

		private bool onSaveClothingItemFailure(CustomizerUIEvents.SaveItemFailure evt)
		{
			setTopTrayToSave();
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ResetRotation));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.EndPurchaseMoment));
			return false;
		}

		private void showSaveNotification(string message)
		{
			DNotification dNotification = new DNotification();
			dNotification.ContainsButtons = false;
			dNotification.PopUpDelayTime = 3f;
			dNotification.Message = message;
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
		}

		private bool onBackButtonClicked(CustomizerUIEvents.BackButtonClickedEvent evt)
		{
			if (evt.ShowConfirmation && customizerModel.IsChanged)
			{
				Service.Get<PromptManager>().ShowPrompt(promptData, onLoseProgressPromptDone);
			}
			else
			{
				returnToSelectTemplate();
			}
			return false;
		}

		private void onLoseProgressPromptDone(DPrompt.ButtonFlags button)
		{
			if (button == DPrompt.ButtonFlags.OK)
			{
				returnToSelectTemplate();
			}
		}

		private void returnToSelectTemplate()
		{
			changeState(State.SELECT_TEMPLATE);
			customizerModel.ItemCustomization.Reset();
		}

		private void OnDestroy()
		{
			changeState(State.PRE_SELECT);
			customizerModel.ItemCustomization.Reset();
			eventChannel.RemoveAllListeners();
		}
	}
}
