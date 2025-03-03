using ClubPenguin.Breadcrumbs;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class SaveScreenController : MonoBehaviour
	{
		private enum SaveStatus
		{
			NOT_STARTED,
			WAITING,
			DONE_WAITING,
			SUCCESS,
			FAIL,
			COMPLETE
		}

		[SerializeField]
		private Image TemplateImage;

		[SerializeField]
		private Text TemplateCostText;

		[SerializeField]
		private GameObject disabledCoverHandle = null;

		[SerializeField]
		private float MinSaveTime = 1f;

		public StaticBreadcrumbDefinitionKey Breadcrumb;

		private EventChannel eventChannel;

		private int templateCost;

		private bool isItemSaving;

		private bool _isInteractable = true;

		private SaveStatus saveStatus;

		public void OnCancelButton()
		{
			if (!isItemSaving)
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveCancel));
			}
		}

		public void OnBuyItemButton()
		{
			if (!isItemSaving)
			{
				isItemSaving = true;
				savingItem();
			}
		}

		private void Start()
		{
			setupListeners();
			SetInteractable(true);
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerUIEvents.SaveClothingItemSuccess>(onSaveClothingItemSuccess);
			eventChannel.AddListener<CustomizerUIEvents.SaveClothingItemError>(onSaveClothingError);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		public void SetSaveScreenInformation(Sprite templateSprite, int cost)
		{
			templateCost = cost;
			TemplateImage.sprite = templateSprite;
			TemplateCostText.text = templateCost.ToString();
		}

		private void savingItem()
		{
			SetInteractable(false);
			saveStatus = SaveStatus.WAITING;
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ConfirmSaveClickedEvent));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.StartPurchaseMoment));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Save, true));
			CoroutineRunner.Start(performSaveWait(), this, "SaveWait");
		}

		private IEnumerator performSaveWait()
		{
			yield return new WaitForSeconds(MinSaveTime);
			if (saveStatus == SaveStatus.SUCCESS)
			{
				saveSuccess();
			}
			else if (saveStatus == SaveStatus.FAIL)
			{
				saveError();
			}
			else if (saveStatus == SaveStatus.WAITING)
			{
				saveStatus = SaveStatus.DONE_WAITING;
			}
		}

		private bool onSaveClothingItemSuccess(CustomizerUIEvents.SaveClothingItemSuccess evt)
		{
			if (saveStatus == SaveStatus.DONE_WAITING)
			{
				saveSuccess();
			}
			else
			{
				saveStatus = SaveStatus.SUCCESS;
			}
			return false;
		}

		private void saveSuccess()
		{
			saveStatus = SaveStatus.COMPLETE;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.RemoveCoinsFromWidget(templateCost));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveItem));
			isItemSaving = false;
			Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(Breadcrumb);
		}

		private bool onSaveClothingError(CustomizerUIEvents.SaveClothingItemError evt)
		{
			if (saveStatus == SaveStatus.DONE_WAITING)
			{
				saveError();
			}
			else
			{
				saveStatus = SaveStatus.FAIL;
			}
			return false;
		}

		private void saveError()
		{
			saveStatus = SaveStatus.COMPLETE;
			SetInteractable(true);
			Service.Get<PromptManager>().ShowPrompt("SaveEquipmentFailPrompt", onFailPromptClose);
			isItemSaving = false;
		}

		private void onFailPromptClose(DPrompt.ButtonFlags flags)
		{
			saveStatus = SaveStatus.FAIL;
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ResetCoinCountWidgetCount));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowCoinCountWidget));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveItemFailure));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Default, true));
		}

		private void SetInteractable(bool isInteractable)
		{
			_isInteractable = isInteractable;
			if (disabledCoverHandle != null)
			{
				disabledCoverHandle.SetActive(!_isInteractable);
			}
		}
	}
}
