using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivitiyScreenSaleWidget : AbstractCellPhoneActivityScreenMemberLockableWidget
	{
		public const string TIME_TEXT_TOKEN = "GoGuide.ShopSale.Timer";

		public const string SALE_PERCENT_TOKEN = "GoGuide.ShopSale.Discount";

		public Text TimeRemainingText;

		public Text SalePercentageText;

		private CellPhoneSaleActivityDefinition saleData;

		private DateTime endTime;

		private Localizer localizer;

		private void Awake()
		{
			localizer = Service.Get<Localizer>();
		}

		protected override void setWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneSaleActivityDefinition x = widgetData as CellPhoneSaleActivityDefinition;
			if (x != null)
			{
				saleData = x;
				setEndTime(saleData);
				setSaleDiscount(saleData);
				CoroutineRunner.Start(updateTimer(), this, "updateTimer");
			}
		}

		protected override void onGoButtonClicked()
		{
			goToLocationInZone();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void goToLocationInZone()
		{
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				CellPhoneSaleActivityDefinition.MarketPlaceSaleData marketplaceSaleDateForGoButton = getMarketplaceSaleDateForGoButton();
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.None;
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(marketplaceSaleDateForGoButton.PositionInZone).SceneName(marketplaceSaleDateForGoButton.Scene.SceneName).SpawnedAction(spawnedAction).Build());
				if (Service.Get<SceneTransitionService>().CurrentScene == marketplaceSaleDateForGoButton.Scene.SceneName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
				}
			}
			Service.Get<ICPSwrveService>().Action("activity_tracker", "go", "starting_soon", saleData.name);
		}

		private CellPhoneSaleActivityDefinition.MarketPlaceSaleData getMarketplaceSaleDateForGoButton()
		{
			CellPhoneSaleActivityDefinition.MarketPlaceSaleData result = saleData.MarketPlaceData[0];
			Scene activeScene = SceneManager.GetActiveScene();
			for (int i = 0; i < saleData.MarketPlaceData.Length; i++)
			{
				if (saleData.MarketPlaceData[i].Scene.SceneName == activeScene.name)
				{
					result = saleData.MarketPlaceData[i];
				}
			}
			return result;
		}

		private void setSaleDiscount(CellPhoneSaleActivityDefinition saleData)
		{
			SalePercentageText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GoGuide.ShopSale.Discount"), saleData.DiscountPercentage);
		}

		private void setEndTime(CellPhoneSaleActivityDefinition saleData)
		{
			ScheduledEventDateDefinition availableDates = saleData.AvailableDates;
			endTime = availableDates.Dates.EndDate.Date;
		}

		private IEnumerator updateTimer()
		{
			WaitForSeconds waitForOneSecond = new WaitForSeconds(1f);
			while (true)
			{
				TimeSpan remainingTime = endTime - Service.Get<ContentSchedulerService>().ScheduledEventDate();
				showTimerTime(remainingTime);
				if (remainingTime.TotalSeconds <= 0.0)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				yield return waitForOneSecond;
			}
		}

		private void showTimerTime(TimeSpan time)
		{
			TimeRemainingText.text = string.Format(localizer.GetTokenTranslation("GoGuide.ShopSale.Timer"), Math.Floor((float)time.TotalHours), time.Minutes, time.Seconds);
		}
	}
}
