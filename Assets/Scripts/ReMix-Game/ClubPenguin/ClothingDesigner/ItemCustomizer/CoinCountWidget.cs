using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CoinCountWidget : MonoBehaviour
	{
		[SerializeField]
		private Text coinCountText;

		private CPDataEntityCollection dataEntityCollection;

		private int coinCount;

		private EventChannel customizationEventChannel;

		private void Start()
		{
			setupListeners();
			initializeCoinCount();
			base.gameObject.SetActive(false);
		}

		private void setupListeners()
		{
			customizationEventChannel = new EventChannel(CustomizationContext.EventBus);
			customizationEventChannel.AddListener<CustomizerUIEvents.StartPurchaseMoment>(onStartPurchaseMoment);
			customizationEventChannel.AddListener<CustomizerWidgetEvents.ShowCoinCountWidget>(onShowCoinCountWidget);
			customizationEventChannel.AddListener<CustomizerWidgetEvents.HideCoinCountWidget>(onHideCoinCountWidget);
			customizationEventChannel.AddListener<CustomizerWidgetEvents.ResetCoinCountWidgetCount>(onResetCoinCountWidgetCount);
			customizationEventChannel.AddListener<CustomizerWidgetEvents.RemoveCoinsFromWidget>(onRemoveCoinsAnimated);
		}

		private void initializeCoinCount()
		{
			setCoinAmount(getCoinCount());
		}

		private int getCoinCount()
		{
			int result = 0;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			CoinsData component;
			if (!dataEntityCollection.LocalPlayerHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				result = component.Coins;
			}
			return result;
		}

		private void setCoinAmount(int newCount)
		{
			coinCount = newCount;
			coinCountText.text = coinCount.ToString();
		}

		private bool onShowCoinCountWidget(CustomizerWidgetEvents.ShowCoinCountWidget evt)
		{
			setCoinAmount(getCoinCount());
			base.gameObject.SetActive(true);
			return false;
		}

		private bool onHideCoinCountWidget(CustomizerWidgetEvents.HideCoinCountWidget evt)
		{
			base.gameObject.SetActive(false);
			return false;
		}

		private bool onResetCoinCountWidgetCount(CustomizerWidgetEvents.ResetCoinCountWidgetCount evt)
		{
			initializeCoinCount();
			return false;
		}

		private bool onRemoveCoinsAnimated(CustomizerWidgetEvents.RemoveCoinsFromWidget evt)
		{
			CoroutineRunner.Start(animateCoinCount(evt.Amount), this, "animateCoinCount");
			return false;
		}

		private IEnumerator animateCoinCount(int amount)
		{
			int newCount = coinCount - amount;
			int animatedCount = coinCount;
			int remove = Mathf.Max(1, Mathf.RoundToInt((float)amount / 5f));
			WaitForSeconds wfs = new WaitForSeconds(0.1f);
			while (animatedCount > newCount)
			{
				animatedCount -= remove;
				if (animatedCount > newCount)
				{
					setCoinAmount(animatedCount);
				}
				else
				{
					setCoinAmount(newCount);
				}
				yield return wfs;
			}
			coinCount = newCount;
			setCoinAmount(coinCount);
		}

		private bool onStartPurchaseMoment(CustomizerUIEvents.StartPurchaseMoment evt)
		{
			base.gameObject.SetActive(false);
			return false;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			customizationEventChannel.RemoveAllListeners();
		}
	}
}
