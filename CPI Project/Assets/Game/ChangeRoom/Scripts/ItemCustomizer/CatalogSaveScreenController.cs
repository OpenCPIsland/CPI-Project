using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CatalogSaveScreenController : MonoBehaviour
	{
		public const string SUBMIT_ERROR_PROMPT_ID = "CatalogSubmittionErrorPrompt";

		public Text DescriptionText;

		private EventChannel eventChannel;

		private void Start()
		{
			setupListeners();
			CoroutineRunner.Start(waitAFrame(), this, "waitAFrame");
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CatalogServiceEvents.ItemSubmissionCompleteEvent>(onItemSubmissionCompleteEvent);
			eventChannel.AddListener<CatalogServiceEvents.ItemSubmissionErrorEvent>(onItemsSubmissionErrorEvent);
			eventChannel.AddListener<CustomizerUIEvents.SubmitClothingItemError>(onSubmitClothingItemError);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private bool onItemSubmissionCompleteEvent(CatalogServiceEvents.ItemSubmissionCompleteEvent evt)
		{
			CoroutineRunner.Start(delayedSubmitSuccess(evt), this, "delayedSubmitSuccess");
			return false;
		}

		private bool onItemsSubmissionErrorEvent(CatalogServiceEvents.ItemSubmissionErrorEvent evt)
		{
			OnError();
			return false;
		}

		private bool onSubmitClothingItemError(CustomizerUIEvents.SubmitClothingItemError evt)
		{
			OnError();
			return false;
		}

		private void OnError()
		{
			Service.Get<PromptManager>().ShowPrompt("CatalogSubmittionErrorPrompt", onFailPromptClose);
		}

		private void onFailPromptClose(DPrompt.ButtonFlags flags)
		{
			CoroutineRunner.Start(waitAFrame(), this, "waitAFrame");
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SwitchToSave));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ResetRotation));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.EndPurchaseMoment));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerEffectsEvents.ItemSaved));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Default, true));
		}

		private IEnumerator delayedSubmitSuccess(CatalogServiceEvents.ItemSubmissionCompleteEvent evt)
		{
			yield return new WaitForSeconds(4f);
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.EndPurchaseMoment));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.ShowSubmittedInCatalog(evt.Response.clothingCatalogItemId));
			int coins2 = Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
			if (evt.Response.newCoinTotal > coins2)
			{
				coins2 = (int)evt.Response.newCoinTotal - coins2;
				CatalogThemeDefinition catalogTheme = Service.Get<CatalogServiceProxy>().GetCatalogTheme();
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(catalogTheme.Title);
				Service.Get<ICPSwrveService>().CoinsGiven(coins2, "rewarded", tokenTranslation, "clothing_catalog_challenge");
				Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "submit_outfit", catalogTheme.ToString(), tokenTranslation);
			}
		}

		private IEnumerator waitAFrame()
		{
			yield return new WaitForSeconds(0.1f);
			if (DescriptionText != null)
			{
				long submissionRewardCoinAmount = Service.Get<CatalogServiceProxy>().currentThemeData.submissionRewardCoinAmount;
				DescriptionText.text = DescriptionText.text.Replace("{0}", submissionRewardCoinAmount.ToString());
			}
		}

		public void OnSubmitItem()
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.ConfirmSubmitClickedEvent));
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.StartPurchaseMoment));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.CatalogSave, true));
		}

		public void OnCancelButton()
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveCancel));
		}
	}
}
